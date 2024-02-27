using System.Reflection;
using Tgs.GameWagers.Stat.Check.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatCheckCore(
		this IServiceCollection services)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton<IStatisticalTimeUtnsil, StatisticalTimeUtnsil>()
		.AddSingleton<IHangfireUtensil, HangfireUtensil>();
}
