using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;
public class StatDataCheckCommandServiceUnitTest
{
	[Fact]
	public async void This_new_game_wager_stat_close_information()
	{
		var fakeLog = NullLogger<StatDataCheckCommandService>.Instance;
		var fakeGameWagerRepository = Substitute.For<IGameWagerRepository>();
		var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();
		var fakeAddService = Substitute.For<ICommandService<GameWagerStatCloseAddCommand>>();
		var fakeUpdateService = Substitute.For<ICommandService<GameWagerStatCloseUpdateCommand>>();

		var command = new StatDataCheckCommand(
			"4ucmqplxb029",
			StatAt: DateTimeOffset.Parse("2024-02-27T08:20:00.000Z"),
			StartAt: DateTimeOffset.Parse("2024-02-27T08:20:00.000Z"),
			EndAt: DateTimeOffset.Parse("2024-02-27T08:25:00.000Z"),
			TriggerAt: DateTimeOffset.Parse("2024-02-27T08:30:00.000Z"));

		var gameWagerStatInfos = new GameWagerStatInfo[]
		{
			new() {
				ResellerId=Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
				LicenseeId=Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
				BrandId=Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
				PlayerCode="4ucmqplxb029",
				GameProviderId=Guid.Parse("6214260a-cf3c-4fb5-ba78-9214fe5608bf"),
				GameId=3500042,
				StatOn= DateTimeOffset.Parse("2024-02-27T08:20:00+00:00"),
				Count=171,
				Bet=85.5m,
				ValidBet=85.5m,
				Settle=41.6m,
				Commission=0,
				Turnover=85.5m,
				WinLoss=-43.9m
			}
		};

		_ = fakeGameWagerRepository.QueryAsync(
			playerCode: Arg.Is(command.PlayerCode),
			startOn: Arg.Is(command.StartAt),
			endOn: Arg.Is(command.EndAt),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameWagerStatInfos.ToAsyncEnumerable());

		_ = fakeGameWagerStatCloseRepository.GetAsync(
			playerCode: Arg.Is(gameWagerStatInfos.First().PlayerCode),
			gameProviderId: Arg.Is(gameWagerStatInfos.First().GameProviderId),
			gameId: Arg.Is(gameWagerStatInfos.First().GameId),
			statOn: Arg.Is(gameWagerStatInfos.First().StatOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((GameWagerStatCloseInfo?)null);

		var sut = new StatDataCheckCommandService(
			fakeLog,
			fakeGameWagerRepository,
			fakeGameWagerStatCloseRepository,
			fakeAddService,
			fakeUpdateService);

		await sut.ExecuteAsync(command);

		_ = fakeAddService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Any<GameWagerStatCloseAddCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeUpdateService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerStatCloseUpdateCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_updates_game_wager_stat_close_information()
	{
		var fakeLog = NullLogger<StatDataCheckCommandService>.Instance;
		var fakeGameWagerRepository = Substitute.For<IGameWagerRepository>();
		var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();
		var fakeAddService = Substitute.For<ICommandService<GameWagerStatCloseAddCommand>>();
		var fakeUpdateService = Substitute.For<ICommandService<GameWagerStatCloseUpdateCommand>>();

		var command = new StatDataCheckCommand(
			"4ucmqplxb029",
			StatAt: DateTimeOffset.Parse("2024-02-27T08:20:00.000Z"),
			StartAt: DateTimeOffset.Parse("2024-02-27T08:20:00.000Z"),
			EndAt: DateTimeOffset.Parse("2024-02-27T08:25:00.000Z"),
			TriggerAt: DateTimeOffset.Parse("2024-02-27T08:30:00.000Z"));

		var gameWagerStatInfos = new GameWagerStatInfo[]
		{
			new() {
				ResellerId=Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
				LicenseeId=Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
				BrandId=Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
				PlayerCode="4ucmqplxb029",
				GameProviderId=Guid.Parse("6214260a-cf3c-4fb5-ba78-9214fe5608bf"),
				GameId=3500042,
				StatOn= DateTimeOffset.Parse("2024-02-27T08:20:00+00:00"),
				Count=171,
				Bet=85.5m,
				ValidBet=85.5m,
				Settle=41.6m,
				Commission=0,
				Turnover=85.5m,
				WinLoss=-43.9m
			}
		};

		_ = fakeGameWagerRepository.QueryAsync(
			playerCode: Arg.Is(command.PlayerCode),
			startOn: Arg.Is(command.StartAt),
			endOn: Arg.Is(command.EndAt),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameWagerStatInfos.ToAsyncEnumerable());

		var gameWagerStatCloseInfo = new GameWagerStatCloseInfo(
			ResellerId: Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
			LicenseeId: Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
			BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: "4ucmqplxb029",
			GameProviderId: Guid.Parse("6214260a-cf3c-4fb5-ba78-9214fe5608bf"),
			GameId: 3500042,
			StatOn: DateTimeOffset.Parse("2024-02-27T08:20:00+00:00"),
			Count: 170,
			Bet: 85.0m,
			ValidBet: 85.0m,
			Settle: 41.6m,
			Commission: 0,
			Turnover: 85.0m,
			WinLoss: -43.4m);
		_ = fakeGameWagerStatCloseRepository.GetAsync(
			playerCode: Arg.Is(gameWagerStatInfos.First().PlayerCode),
			gameProviderId: Arg.Is(gameWagerStatInfos.First().GameProviderId),
			gameId: Arg.Is(gameWagerStatInfos.First().GameId),
			statOn: Arg.Is(gameWagerStatInfos.First().StatOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameWagerStatCloseInfo);

		var sut = new StatDataCheckCommandService(
			fakeLog,
			fakeGameWagerRepository,
			fakeGameWagerStatCloseRepository,
			fakeAddService,
			fakeUpdateService);

		await sut.ExecuteAsync(command);

		_ = fakeAddService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerStatCloseAddCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeUpdateService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Any<GameWagerStatCloseUpdateCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void No_changes()
	{
		var fakeLog = NullLogger<StatDataCheckCommandService>.Instance;
		var fakeGameWagerRepository = Substitute.For<IGameWagerRepository>();
		var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();
		var fakeAddService = Substitute.For<ICommandService<GameWagerStatCloseAddCommand>>();
		var fakeUpdateService = Substitute.For<ICommandService<GameWagerStatCloseUpdateCommand>>();

		var command = new StatDataCheckCommand(
			"4ucmqplxb029",
			StatAt: DateTimeOffset.Parse("2024-02-27T08:20:00.000Z"),
			StartAt: DateTimeOffset.Parse("2024-02-27T08:20:00.000Z"),
			EndAt: DateTimeOffset.Parse("2024-02-27T08:25:00.000Z"),
			TriggerAt: DateTimeOffset.Parse("2024-02-27T08:30:00.000Z"));

		var gameWagerStatInfos = new GameWagerStatInfo[]
		{
			new() {
				ResellerId=Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
				LicenseeId=Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
				BrandId=Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
				PlayerCode="4ucmqplxb029",
				GameProviderId=Guid.Parse("6214260a-cf3c-4fb5-ba78-9214fe5608bf"),
				GameId=3500042,
				StatOn= DateTimeOffset.Parse("2024-02-27T08:20:00+00:00"),
				Count=171,
				Bet=85.5m,
				ValidBet=85.5m,
				Settle=41.6m,
				Commission=0,
				Turnover=85.5m,
				WinLoss=-43.9m
			}
		};

		_ = fakeGameWagerRepository.QueryAsync(
			playerCode: Arg.Is(command.PlayerCode),
			startOn: Arg.Is(command.StartAt),
			endOn: Arg.Is(command.EndAt),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameWagerStatInfos.ToAsyncEnumerable());

		var gameWagerStatCloseInfo = new GameWagerStatCloseInfo(
			ResellerId: Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
			LicenseeId: Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
			BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
			PlayerCode: "4ucmqplxb029",
			GameProviderId: Guid.Parse("6214260a-cf3c-4fb5-ba78-9214fe5608bf"),
			GameId: 3500042,
			StatOn: DateTimeOffset.Parse("2024-02-27T08:20:00+00:00"),
			Count: 171,
			Bet: 85.5m,
			ValidBet: 85.5m,
			Settle: 41.6m,
			Commission: 0,
			Turnover: 85.5m,
			WinLoss: -43.9m);
		_ = fakeGameWagerStatCloseRepository.GetAsync(
			playerCode: Arg.Is(gameWagerStatInfos.First().PlayerCode),
			gameProviderId: Arg.Is(gameWagerStatInfos.First().GameProviderId),
			gameId: Arg.Is(gameWagerStatInfos.First().GameId),
			statOn: Arg.Is(gameWagerStatInfos.First().StatOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameWagerStatCloseInfo);

		var sut = new StatDataCheckCommandService(
			fakeLog,
			fakeGameWagerRepository,
			fakeGameWagerStatCloseRepository,
			fakeAddService,
			fakeUpdateService);

		await sut.ExecuteAsync(command);

		_ = fakeAddService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerStatCloseAddCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeUpdateService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameWagerStatCloseUpdateCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
