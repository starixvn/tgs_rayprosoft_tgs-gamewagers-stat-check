using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameWagers.Stat.Check.Common.Protos;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.VipLevels.Level.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;

public class StatCheckDataBacklogCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var fakeGameWagerRepository = Substitute.For<IGameWagerStatCheckDataBacklogRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new StatCheckDataBacklogCommand(
			TriggerAt: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)));

		var infos = new GameWagerStatCheckDataBacklogInfo[] {new(
			"4ucmqplxb029",
			StatAt:DateTimeOffset.Parse("2024-02-27T08:20:00.000Z"),
			StartAt:DateTimeOffset.Parse("2024-02-27T08:20:00.000Z"),
			EndAt:DateTimeOffset.Parse("2024-02-27T08:25:00.000Z"),
			TriggerAt: DateTimeOffset.Parse("2024-02-27T08:30:13.645Z"),
			Count:172)
		};

		fakeGameWagerRepository.QueryAsync(
			Arg.Is(command.TriggerAt),
			offset: Arg.Any<int>(),
			limit: Arg.Is(100),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(infos.ToAsyncEnumerable(), Enumerable.Empty<GameWagerStatCheckDataBacklogInfo>().ToAsyncEnumerable());

		var sut = new StatCheckDataBacklogCommandService(
			fakeGameWagerRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameWagersStatCheckNatsConst.StatCheckData),
			data: Arg.Is(new GameWagersStatCheckCheckDataMessage
			{
				PlayerCode = infos.First().PlayerCode,
				StatAt = infos.First().StatAt.ToTimestamp(),
				StartAt = infos.First().StartAt.ToTimestamp(),
				EndAt = infos.First().EndAt.ToTimestamp(),
				TriggerAt = infos.First().TriggerAt.ToTimestamp(),
			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerRepository
			.Received(1)
			.UpdateExpireAtAsync(
			Arg.Is(infos.First().PlayerCode),
			Arg.Is(infos.First().StatAt),
			Arg.Is(infos.First().StartAt),
			Arg.Is(infos.First().EndAt),
			Arg.Is(infos.First().TriggerAt),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
