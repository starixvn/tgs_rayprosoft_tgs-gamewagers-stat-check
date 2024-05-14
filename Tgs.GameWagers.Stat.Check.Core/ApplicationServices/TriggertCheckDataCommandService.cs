using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class TriggertCheckDataCommandService(
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatCheckDataBacklogRepository repository) : ICommandService<TriggertCheckDataCommand>
{
	public async ValueTask ExecuteAsync(TriggertCheckDataCommand command, CancellationToken cancellationToken = default)
	{
		var statOn = statisticalTimeUtnsil.GetStatOn(command.ClosedOn);

		await repository.AddAsync(
			playerCode: command.PlayerCode,
			statAt: statOn,
			startAt: statOn,
			endAt: statOn.AddMinutes(+5),
			triggerAt: statOn.AddMinutes(30),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
