using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.GameBets.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class TriggertCheckDataCommandService(
	ILogger<TriggertCheckDataCommandService> logger,
	IGameBetsClient client,
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatCheckDataBacklogRepository repository) : ICommandService<TriggertCheckDataCommand>
{
	public async ValueTask ExecuteAsync(TriggertCheckDataCommand command, CancellationToken cancellationToken = default)
	{
		var retryCount = 5;
		var retrySeconds = 2;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "LogOn:{logOn} Exception:{exception} RetryTime:{retryTime} RetryCount:{retryCount} Source:{source}",
					DateTime.Now.ToString("o"),
					exception.Message,
				retryTime,
					retryCount,
				new
				{
					command.GameBetId,
					command.GameProviderId,
					command.GameId,
					command.ResellerId,
					command.LicenseeId,
					command.BrandId,
					command.PlayerCode,
					command.CurrencyCode,
					command.WalletId,
					command.ExternalRoundId,
					command.ExternalBetId,
					command.ExternalCreatedOn
				}));

		var info = await retryPolicy.ExecuteAsync(() => client.GameBetGetByExternalBetAsync(
			gameProviderId: command.GameProviderId,
			gameId: command.GameId,
			walletId: command.WalletId,
			externalBetId: command.ExternalBetId,
			externalCreatedOn: command.ExternalCreatedOn,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(JsonSerializer.Serialize(new
			{
				command.GameProviderId,
				command.GameId,
				command.WalletId,
				command.ExternalBetId,
				command.ExternalCreatedOn,
			}));

		var statOn = statisticalTimeUtnsil.GetStatOn(info.ClosedOn);

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
