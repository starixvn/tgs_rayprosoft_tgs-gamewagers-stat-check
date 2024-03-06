using Tgs.GameWagers.Stat.Check.Core.JobApplicationServices;

namespace Tgs.GameWagers.Stat.Check.Core;

public interface IHangfireUtensil
{
	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobExecuteAsync(JobExecuteStatCheckHourlyCommand source);

	public Task JobInitAsync();
}
