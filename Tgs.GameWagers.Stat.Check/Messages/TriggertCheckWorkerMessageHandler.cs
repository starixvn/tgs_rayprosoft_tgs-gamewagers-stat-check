using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

using Tgs.ServiceUtility;

using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class TriggertCheckWorkerMessageHandler(
	ILogger<TriggertCheckWorkerMessageHandler> logger,
	ICommandService<TriggertCheckWorkerCommand> service) : IMessageHandler<GameBets.Common.Protos.GameBetPublish>
{
	public ValueTask HandleAsync(string subject, GameBets.Common.Protos.GameBetPublish data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
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
				GameBetId: data.GameBetId.ToGuid(),
				GameProviderId: data.GameProviderId.ToGuid(),
				GameId: data.GameId,
				ResellerId: data.ResellerId.ToGuid(),
				LicenseeId: data.LicenseeId.ToGuid(),
				BrandId: data.BrandId.ToGuid(),
				PlayerCode: data.PlayerCode,
				CurrencyCode: data.CurrencyCode,
				WalletId: data.WalletId.ToGuid(),
				ExternalRoundId: data.ExternalRoundId,
				ExternalBetId: data.ExternalBetId,
				ExternalCreatedOn: data.ExternalCreatedOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
