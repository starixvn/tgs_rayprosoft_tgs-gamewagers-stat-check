using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class TriggertCheckWorkerCommandService(
	ILogger<TriggertCheckWorkerCommandService> logger,
	IGameWagerStatCheckWorkerBacklogRepository repository) : ICommandService<TriggertCheckWorkerCommand>
{
	public ValueTask ExecuteAsync(TriggertCheckWorkerCommand command, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Mesaage:This command information.({command})",
			DateTime.Now.ToString("o"),
			command);

		var triggerAt = DateTimeOffset.Parse(command.ClosedOn.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:00+00:00"));

		return repository.AddAsync(
			playerCode: command.PlayerCode,
			triggerAt: triggerAt.AddMinutes(+5),
			cancellationToken: cancellationToken);
	}
}
