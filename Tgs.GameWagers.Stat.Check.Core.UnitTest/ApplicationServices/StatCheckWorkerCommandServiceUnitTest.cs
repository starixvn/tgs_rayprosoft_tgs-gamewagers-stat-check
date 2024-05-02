using NSubstitute;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Worker.Common;
using Tgs.GameWagers.Stat.Worker.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;

public class StatCheckWorkerCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new StatCheckWorkerCommand(
			  "4ucmqplxb029");

		var sut = new StatCheckWorkerCommandService(
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameWagersStatWorkerNatsConst.Check),
			data: Arg.Is(new GameWagersStatWorkerMessage
			{
				PlayerCode = command.PlayerCode
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
