using Tgs.GameWagers.Stat.Worker.Common;
using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
internal class StatWorkerCheckCommandService(
	IMessageSender messageSender) : ICommandService<StatWorkerCheckCommand>
{
	public ValueTask ExecuteAsync(StatWorkerCheckCommand command, CancellationToken cancellationToken = default)
		=> messageSender.PublishAsync(
			subject: GameWagersStatWorkerNatsConst.Check,
			data: new GameWagersStatWorkerMessage
			{
				PlayerCode = command.PlayerCode
			},
			cancellationToken: cancellationToken);

}
