using System.Net.Mime;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GameWagers.Stat.Check;
using Tgs.GameWagers.Stat.Check.Common;
using Tgs.GameWagers.Stat.Check.Messages;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Worker.Common;
using Tgs.MessageQueue.Nats.Serializers;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.AddOpenTelemetry(options =>
	{
		options.IncludeFormattedMessage = true;
		options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
			fluentOptions.UseMessagePack();
		});
	});

// Add services to the container.
builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	foreach (var xmlFileName in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml"))
		c.IncludeXmlComments(xmlFileName);
});

// Learn more about configuring Hangfire
builder
	.Services
	.AddHangfire((sp, config) =>
	{
		var systemConfig = sp.GetRequiredService<IConfiguration>();
		var connectionString = builder.Configuration.GetConnectionString("Redis")!;
		var options = new RedisStorageOptions
		{
			Prefix = "gamewagers-stat-check-schedule:hangfire:"
		};
		_ = config
		.UseRedisStorage(connectionString, options)
		.WithJobExpirationTimeout(TimeSpan.FromMinutes(30));
	})
	.AddHangfireServer();

builder.Services.AddGameWagersStatCheck(
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.ConnectionString = configuration.GetConnectionString("Redis")!;
	},
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.ConnectionString = configuration.GetConnectionString("Mongo")!;
	},
	(sp, dbBuilder) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		_ = dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"));
	})
	.AddGameBetsClient(httpClientBuilder => httpClientBuilder.ConfigureHttpClient((sp, http) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		http.BaseAddress = config.GetValue<Uri>("GameWagersStatCheckConnectionStrings:GameBetsClientBaseUrl");
	}));

builder.Services
	.AddKeyedSingleton("Nats", (sp, _) => new NatsConnection(NatsOpts.Default with
	{
		Url = builder.Configuration.GetConnectionString("Nats")!,
		SerializerRegistry = TgsNatsSerializerRegistration.Default
	}))
	.AddMessageQueue()
	.AddNatsMessageQueue(config => config
	.ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
	.AddHandler<TriggertCheckDataMessageHandler>(Tgs.GameBets.Common.GameBetsNatsConst.GameBetAddPublish, typeof(TriggertCheckDataMessageHandler).FullName!.ToLower())
	.AddHandler<TriggertCheckWorkerMessageHandler>(Tgs.GameBets.Common.GameBetsNatsConst.GameBetAddPublish, typeof(TriggertCheckWorkerMessageHandler).FullName!.ToLower())
	.AddHandler<StatCheckDataBacklogMessageHandler>(GameWagersStatCheckNatsConst.StatBacklog, typeof(StatCheckDataBacklogMessageHandler).FullName!.ToLower())
	.AddHandler<StatCheckDataMessageHandler>(GameWagersStatCheckNatsConst.StatCheckData, typeof(StatCheckDataMessageHandler).FullName!.ToLower())
	.AddHandler<StatCheckWorkerBacklogMessageHandler>(GameWagersStatCheckNatsConst.StatBacklog, typeof(StatCheckWorkerBacklogMessageHandler).FullName!.ToLower())
	.AddHandler<StatCheckWorkerMessageHandler>(GameWagersStatCheckNatsConst.StatCheckWorker, typeof(StatCheckWorkerMessageHandler).FullName!.ToLower())
	.AddHandler<StatCheckDataHourlyMessageHandler>(GameWagersStatCheckNatsConst.StatCheckHourlyData, typeof(StatCheckDataHourlyMessageHandler).FullName!.ToLower()))
	.ClearAllExchanges()
	.AddNatsGlobPatternExchange(GameWagersStatCheckNatsConst.StatBacklog)
	.AddNatsGlobPatternExchange(GameWagersStatCheckNatsConst.StatCheckData)
	.AddNatsGlobPatternExchange(GameWagersStatCheckNatsConst.StatCheckHourlyData)
	.AddNatsGlobPatternExchange(GameWagersStatCheckNatsConst.StatCheckWorker)
	.AddNatsGlobPatternExchange(Tgs.GameBets.Common.GameBetsNatsConst.GameBetAddPublish)
	.AddNatsGlobPatternExchange(GameWagersStatHourlyNatsConst.StatHourlyClose)
	.AddNatsGlobPatternExchange(GameWagersStatWorkerNatsConst.Check);

builder.Services
	.AddOpenTelemetry()
	.WithTracing(builder => builder
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddSource("Valhalla.*")
		.AddAspNetCoreInstrumentation(options => options.Filter =
		httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/healthz")
			&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
			&& !httpContext.Request.Path.StartsWithSegments("/hangfire")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
		.AddHttpClientInstrumentation()
		.AddGrpcClientInstrumentation()
		.AddGrpcCoreInstrumentation()
		.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
		.AddOtlpExporter())
	.WithMetrics(meterBuiler => meterBuiler
	.SetResourceBuilder(ResourceBuilder.CreateDefault()
		.AddEnvironmentVariableDetector())
		.AddRuntimeInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter())
	.Services
	.AddInterceptionService()
	.AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

app.UseHangfireDashboard(
	"/hangfire",
	new DashboardOptions()
	{
		Authorization =
		new[]
		{
			new IgnoreHangfireAuthorizationFilter()
		}
	});

_ = app.MapControllers();
_ = app.MapSwagger();

app.UseHealthChecks("/healthz");

_ = app.UseHangfireRegisterAsync();

app.Run();
