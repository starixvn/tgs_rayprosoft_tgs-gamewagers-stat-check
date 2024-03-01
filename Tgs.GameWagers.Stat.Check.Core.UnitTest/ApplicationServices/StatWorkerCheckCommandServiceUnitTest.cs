using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameWagers.Stat.Check.Common.Protos;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.VipLevels.Level.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;
public class StatWorkerCheckCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var fakeRepository = Substitute.For<IGameWagerStatCheckWorkerBacklogRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new StatWorkerCheckBacklogCommand(
			TriggerAt: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)));

		var infos = new GameWagerStatCheckWorkerBacklogInfo[] {new(
			"4ucmqplxb029",
			TriggerAt: DateTimeOffset.Parse("2024-02-27T08:30:13.645Z"),
			Count:172)
		};

		fakeRepository.QueryAsync(
			Arg.Is(command.TriggerAt),
			offset: Arg.Any<int>(),
			limit: Arg.Is(100),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(infos.ToAsyncEnumerable(), Enumerable.Empty<GameWagerStatCheckWorkerBacklogInfo>().ToAsyncEnumerable());

		var sut = new StatWorkerCheckBacklogCommandService(
			fakeRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameWagersStatCheckNatsConst.StatWorkerCheck),
			data: Arg.Is(new GameWagersStatCheckCheckWorkerMessage
			{
				PlayerCode = infos.First().PlayerCode

			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeRepository
			.Received(1)
			.UpdateExpireAtAsync(
			Arg.Is(infos.First().PlayerCode),
			Arg.Is(infos.First().TriggerAt),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
