using System.Data;
using Dapper;

using Dapper.ColumnMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Npgsql;
using Tgs.GameWagers.Stat.Check.Core;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.GameWagers.Stat.Check.Entities;
using Tgs.GameWagers.Stat.Check.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatCheckRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
	{
		_ = services
			.AddDbContext<TgsContext>(dbContextOptions)
			.AddScoped<IDbConnection, NpgsqlConnection>((sp) =>
			{
				var context = sp.GetRequiredService<TgsContext>();
				return (NpgsqlConnection)context.Database.GetDbConnection();
			})
			.AddTransient<IBrandRepository, BrandRepository>()
			.AddTransient<IGameWagerRepository, GameWagerRepository>()
			.AddTransient<IGameWagerStatCloseRepository, GameWagerStatCloseRepository>()
			.AddTransient<IGameWagerStatCheckWorkerBacklogRepository>((sp) => ActivatorUtilities.CreateInstance<GameWagerStatCheckWorkerBacklogRepository>(sp, mongoDatabaseResolver(sp)))
			.AddTransient<IGameWagerStatCheckDataBacklogRepository>((sp) => ActivatorUtilities.CreateInstance<GameWagerStatCheckDataBacklogRepository>(sp, mongoDatabaseResolver(sp)));

		SqlMapper.SetTypeMap(typeof(GameWagerStatInfo), new ColumnTypeMapper(typeof(GameWagerStatInfo)));
		return services;
	}
}
