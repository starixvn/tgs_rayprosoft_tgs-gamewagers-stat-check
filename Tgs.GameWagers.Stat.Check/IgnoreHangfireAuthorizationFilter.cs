using Hangfire.Dashboard;

namespace Tgs.GameWagers.Stat.Check;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
