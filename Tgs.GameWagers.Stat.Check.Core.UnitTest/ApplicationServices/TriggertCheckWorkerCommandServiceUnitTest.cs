using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameBets.Client;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;

public class TriggertCheckWorkerCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<TriggertCheckWorkerCommandService>.Instance;
		var fakeGameBetsClient = Substitute.For<IGameBetsClient>();
		var fakeRepository = Substitute.For<IGameWagerStatCheckWorkerBacklogRepository>();

		var command = new TriggertCheckWorkerCommand(
			GameBetId: Guid.NewGuid(),
			GameProviderId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			LicenseeId: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: Guid.NewGuid().ToString(),
			ExternalBetId: Guid.NewGuid().ToString(),
			ExternalCreatedOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)));

		var info = new GameBets.Common.ViewModels.GameBetInfoViewModel(
			Id: command.GameBetId,
			GameProviderId: command.GameProviderId,
			GameProviderConfigurationId: Guid.NewGuid(),
			GameId: 1,
			ResellerId: command.ResellerId,
			LicenseeId: command.LicenseeId,
			BrandId: command.BrandId,
			PlayerCode: "test",
			CurrencyCode: "CNY",
			WalletId: command.WalletId,
			ExternalRoundId: command.ExternalRoundId,
			ExternalBetId: command.ExternalBetId,
			ExternalCreatedOn: command.ExternalCreatedOn,
			ExternalClosedOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)),
			ExternalPayoffOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.Common.Enums.BetState.Loss,
			BetType: GameBets.Common.Enums.BetType.Normal,
			FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			GambleState: GameBets.Common.Enums.GambleState.None,
			CashoutState: GameBets.Common.Enums.CashoutState.None,
			ResettleState: GameBets.Common.Enums.ResettleState.None,
			JackpotState: GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1,
			Bet: 100,
			ValidBet: 50,
			Settle: 0.0m,
			Commission: 0,
			Turnover: 50,
			WinLoss: -50m,
			Jackpot: 0.0m,
			CreatedOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)),
			ClosedOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)),
			UpdatedOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)),
			ResetState: GameBets.Common.Enums.ResetState.None,
			ResetOn: DateTimeOffset.UnixEpoch,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = command.ExternalRoundId,
				["ExternalBetId"] = command.ExternalBetId
			});

		_ = fakeGameBetsClient.GameBetGetByExternalBetAsync(
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			walletId: Arg.Is(command.WalletId),
			externalBetId: Arg.Is(command.ExternalBetId),
			externalCreatedOn: Arg.Is(command.ExternalCreatedOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(info);

		var sut = new TriggertCheckWorkerCommandService(
			fakeLog,
			fakeGameBetsClient,
			fakeRepository);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			playerCode: Arg.Is(command.PlayerCode),
			triggerAt: Arg.Is(new DateTimeOffset(2024, 02, 27, 16, 39, 00, 00, TimeSpan.FromHours(8))),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
