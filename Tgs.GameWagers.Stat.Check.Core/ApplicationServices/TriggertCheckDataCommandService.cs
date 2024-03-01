using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class TriggertCheckDataCommandService(
	ILogger<TriggertCheckDataCommandService> logger,
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatCheckDataBacklogRepository repository) : ICommandService<TriggertCheckDataCommand>
{
	public ValueTask ExecuteAsync(TriggertCheckDataCommand command, CancellationToken cancellationToken = default)
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
			triggerAt: statOn.AddHours(1),
			cancellationToken: cancellationToken);
	}
}
