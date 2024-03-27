using Tgs.GameWagers.Stat.Check.Core.JobApplicationServices;

using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core;

internal class HangfireUtensil(
	ICommandService<JobExecuteCommand> jobExecuteService,
	ICommandService<JobExecuteStatCheckHourlyCommand> jobExecuteStatCheckHourlyService,
	ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensil
{
	public Task JobExecuteAsync(JobExecuteCommand source)
		=> jobExecuteService.ExecuteAsync(source).AsTask();

	public Task JobExecuteAsync(JobExecuteStatCheckHourlyCommand source)
		=> jobExecuteStatCheckHourlyService.ExecuteAsync(source).AsTask();

	public Task JobInitAsync()
		=> jobInitservice.ExecuteAsync(new()).AsTask();
}
