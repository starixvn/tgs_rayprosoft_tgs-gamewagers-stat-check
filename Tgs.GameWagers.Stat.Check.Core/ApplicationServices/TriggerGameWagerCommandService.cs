using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class TriggerGameWagerCommandService(
	ILogger<TriggerGameWagerCommandService> logger,
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatBacklogRepository repository) : ICommandService<TriggerGameWagerCommand>
{
	public ValueTask ExecuteAsync(TriggerGameWagerCommand command, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Mesaage:This command information.({command})",
			DateTime.Now.ToString("o"),
			command);

		var statOn = statisticalTimeUtnsil.GetStatOn(command.ClosedOn);

		return repository.AddAsync(
			playerCode: command.PlayerCode,
			statAt: statOn,
			startAt: statOn,
			endAt: statOn.AddMinutes(+5),
			triggerAt: statOn.AddMinutes(+10),
			cancellationToken: cancellationToken);
	}
}
