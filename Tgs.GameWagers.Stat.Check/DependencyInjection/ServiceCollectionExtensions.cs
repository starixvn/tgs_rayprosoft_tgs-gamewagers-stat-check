using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Check;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatCheck(
		this IServiceCollection services,
		Action<RedisOptions, IServiceProvider> redisOptions,
		Action<MongoDBOptions, IServiceProvider> mongoDBconfigure,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisOptions)
		.Services
		.AddOptions<MongoDBOptions>()
		.Configure(mongoDBconfigure)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddSingleton<MongoDBConnectionManager>()
		.AddSingleton(TimeProvider.System)
		.AddGameWagersStatCheckCore()
		.AddGameWagersStatCheckRepositories(
			dbContextOptions: dbContextOptions,
			(sp) => sp.GetRequiredService<MongoDBConnectionManager>().MongoDatabase);
}
