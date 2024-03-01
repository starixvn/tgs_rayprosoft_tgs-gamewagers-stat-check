using Tgs.ServiceUtility;

using Valhalla.MessageQueue;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Worker.Common.Protos;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class TriggertCheckDataMessageHandler(
	ILogger<TriggertCheckDataMessageHandler> logger,
	ICommandService<TriggertCheckDataCommand> service) : IMessageHandler<GameWagersFirstsMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersFirstsMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var loggerInfo = new
		{
			GameProviderId = data.GameProviderId.ToGuid(),
			data.GameId,
			data.PlayerCode,
			WalletId = data.WalletId.ToGuid(),
			data.ExternalWagerId,
			CreatedOn = data.CreatedOn.ToDateTimeOffset()
		};

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			loggerInfo);

		return service.ExecuteAsync(
			command: new TriggertCheckDataCommand(
				PlayerCode: data.PlayerCode,
				ClosedOn: data.ClosedOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
