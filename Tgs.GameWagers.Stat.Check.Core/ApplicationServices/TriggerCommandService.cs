using Google.Protobuf.WellKnownTypes;
using Tgs.GameWagers.Stat.Check.Common.Protos;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Level.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
internal class TriggerCommandService(
	IGameWagerStatBacklogRepository repository,
	IMessageSender messageSender) : ICommandService<TriggerCommand>
{
	public async ValueTask ExecuteAsync(TriggerCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<GameWagerStatBacklogInfo>? infos;
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
				?? Enumerable.Empty<GameWagerStatBacklogInfo>();

			foreach (var info in infos)
			{
				await messageSender.PublishAsync(
					GameWagersStatCheckNatsConst.Check,
					data: new GameWagersStatCheckCheckPlayerMessage
					{
						PlayerCode = info.PlayerCode,
						StatAt = info.StatAt.ToTimestamp(),
						StartAt = info.StartAt.ToTimestamp(),
						EndAt = info.EndAt.ToTimestamp(),
						TriggerAt = info.TriggerAt.ToTimestamp()
					},
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}

			offset += 1;
		} while (infos.Any());
	}
}
