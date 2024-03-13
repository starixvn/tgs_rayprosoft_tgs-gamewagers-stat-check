using Google.Protobuf.WellKnownTypes;
using Tgs.GameWagers.Stat.Check.Common.Protos;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;
using Tgs.GameWagers.Stat.Check.Common;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class StatCheckDataBacklogCommandService(
	IGameWagerStatCheckDataBacklogRepository repository,
	IMessageSender messageSender) : ICommandService<StatCheckDataBacklogCommand>
{
	public async ValueTask ExecuteAsync(StatCheckDataBacklogCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<GameWagerStatCheckDataBacklogInfo>? infos;
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
				?? Enumerable.Empty<GameWagerStatCheckDataBacklogInfo>();

			foreach (var info in infos)
			{
				await messageSender.PublishAsync(
					GameWagersStatCheckNatsConst.StatCheckData,
					data: new GameWagersStatCheckCheckDataMessage
					{
						PlayerCode = info.PlayerCode,
						StatAt = info.StatAt.ToTimestamp(),
						StartAt = info.StartAt.ToTimestamp(),
						EndAt = info.EndAt.ToTimestamp(),
						TriggerAt = info.TriggerAt.ToTimestamp()
					},
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				await repository.DeleteAsync(
					playerCode: info.PlayerCode,
					statAt: info.StatAt,
					startAt: info.StartAt,
					endAt: info.EndAt,
					triggerAt: info.TriggerAt,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}

			offset += 1;
		} while (infos.Any());
	}
}
