using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

using Tgs.ServiceUtility;

using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class TriggertCheckWorkerMessageHandler(
	ILogger<TriggertCheckWorkerMessageHandler> logger,
	ICommandService<TriggertCheckWorkerCommand> service) : IMessageHandler<GameBets.Worker.Common.Protos.GameBetsWorkerPublish>
{
	public ValueTask HandleAsync(string subject, GameBets.Worker.Common.Protos.GameBetsWorkerPublish data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			new
			{
				GameBetId = data.GameBetId.ToGuid(),
				GameProviderId = data.GameProviderId.ToGuid(),
				data.GameId,
				ResellerId = data.ResellerId.ToGuid(),
				LicenseeId = data.LicenseeId.ToGuid(),
				BrandId = data.BrandId.ToGuid(),
				data.PlayerCode,
				data.CurrencyCode,
				WalletId = data.WalletId.ToGuid(),
				data.ExternalRoundId,
				data.ExternalBetId,
				ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset()
			});

		return service.ExecuteAsync(
			command: new TriggertCheckWorkerCommand(
				PlayerCode: data.PlayerCode,
				ClosedOn: data.ClosedOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
