using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;
public class TriggertCheckWorkerCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRepository = Substitute.For<IGameWagerStatCheckWorkerBacklogRepository>();

		var command = new TriggertCheckWorkerCommand(
			PlayerCode: "4ucmqplxb029",
			ClosedOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)));

		var sut = new TriggertCheckWorkerCommandService(
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
