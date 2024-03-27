using Tgs.GameBets.First.Worker.Common.Protos;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

using Tgs.ServiceUtility;

using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class TriggertCheckDataMessageHandler(
	ILogger<TriggertCheckDataMessageHandler> logger,
	ICommandService<TriggertCheckDataCommand> service) : IMessageHandler<GameBetsFirstWorkerStatMessage>
{
	public ValueTask HandleAsync(string subject, GameBetsFirstWorkerStatMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			new
			{
				GameProviderId = data.GameProviderId.ToGuid(),
				data.GameId,
				data.PlayerCode,
				WalletId = data.WalletId.ToGuid(),
				data.ExternalRoundId,
				data.ExternalBetId,
				ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset(),
				ClosedOn = data.ClosedOn.ToDateTimeOffset()
			});

		return service.ExecuteAsync(
			command: new TriggertCheckDataCommand(
				PlayerCode: data.PlayerCode,
				ClosedOn: data.ClosedOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
