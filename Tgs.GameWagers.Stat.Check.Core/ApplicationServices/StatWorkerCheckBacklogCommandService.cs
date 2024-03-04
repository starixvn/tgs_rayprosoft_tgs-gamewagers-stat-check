using Tgs.GameWagers.Stat.Check.Common.Protos;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Level.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
internal class StatWorkerCheckBacklogCommandService(
	IGameWagerStatCheckWorkerBacklogRepository repository,
	IMessageSender messageSender) : ICommandService<StatWorkerCheckBacklogCommand>
{
	public async ValueTask ExecuteAsync(StatWorkerCheckBacklogCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<GameWagerStatCheckWorkerBacklogInfo>? infos;
		var offset = 0;
		var limit = 100;

		do
		{
			infos = await repository.QueryAsync(
				endOn: command.TriggerAt,
				offset: offset,
				limit: limit,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameWagerStatCheckWorkerBacklogInfo>();

			foreach (var info in infos)
			{
				await messageSender.PublishAsync(
					GameWagersStatCheckNatsConst.StatWorkerCheck,
					data: new GameWagersStatCheckCheckWorkerMessage
					{
						PlayerCode = info.PlayerCode,
					},
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				await repository.UpdateExpireAtAsync(
					playerCode: info.PlayerCode,
					triggerAt: info.TriggerAt.AddMinutes(+5),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}

			offset += 1;
		} while (infos.Any());
	}
}
