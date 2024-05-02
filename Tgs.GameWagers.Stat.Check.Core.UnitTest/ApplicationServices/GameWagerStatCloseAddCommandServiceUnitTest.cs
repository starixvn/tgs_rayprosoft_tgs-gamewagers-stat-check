using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;

public class GameWagerStatCloseAddCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<GameWagerStatCloseAddCommandService>.Instance;
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameWagerStatCloseAddCommand(
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

		var brand = new BrandInfo(
			ResellerId: command.ResellerId,
			ResellerCode: "Demo",
			LicenseeId: command.LicenseeId,
			LicenseeCode: "TGS",
			BrandId: command.BrandId,
			BrandCode: "GPI",
			TimeSpanMinutes: 480);
		_ = fakeBrandRepository.GetAsync(
			id: Arg.Is(command.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(brand);

		var sut = new GameWagerStatCloseAddCommandService(
			faceLogger,
			fakeBrandRepository,
			fakeGameWagerStatCloseRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		await fakeGameWagerStatCloseRepository
			.Received(1)
			.AddAsync(
			resellerId: Arg.Is(command.ResellerId),
			licenseeId: Arg.Is(command.LicenseeId),
			brandId: Arg.Is(command.BrandId),
			playerCode: Arg.Is(command.PlayerCode),
			gameProviderId: Arg.Is(command.GameProviderId),
			gameId: Arg.Is(command.GameId),
			statOn: Arg.Is(command.StatOn),
			count: Arg.Is(command.Count),
			bet: Arg.Is(command.Bet),
			validBet: Arg.Is(command.ValidBet),
			settle: Arg.Is(command.Settle),
			commission: Arg.Is(command.Commission),
			turnover: Arg.Is(command.Turnover),
			winLoss: Arg.Is(command.WinLoss),
			cancellationToken: Arg.Any<CancellationToken>());

		await fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameWagersStatHourlyNatsConst.StatHourlyClose),
			data: Arg.Is(new GameWagersStatHourlyMessage
			{
				ResellerId = brand.ResellerId,
				ResellerCode = brand.ResellerCode,
				LicenseeId = brand.LicenseeId,
				LicenseeCode = brand.LicenseeCode,
				BrandId = brand.BrandId,
				BrandCode = brand.BrandCode,
				TimeSpanMinutes = brand.TimeSpanMinutes,
				StartOn = command.StatOn.AddMinutes(-5).ToTimestamp(),
				EndOn = command.StatOn.AddMinutes(+5).ToTimestamp()
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
