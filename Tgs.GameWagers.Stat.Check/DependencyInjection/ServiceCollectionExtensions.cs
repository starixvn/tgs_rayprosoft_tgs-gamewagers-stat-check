using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatCheck(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
		=> services
		.AddSingleton(TimeProvider.System)
		.AddGameWagersStatCheckCore()
		.AddGameWagersStatCheckRepositories(
			dbContextOptions: dbContextOptions);
}
