using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;
public class TriggertCheckDataCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeRepository = Substitute.For<IGameWagerStatCheckDataBacklogRepository>();

		var command = new TriggertCheckDataCommand(
			PlayerCode: "4ucmqplxb029",
			ClosedOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)));

		var statOn = new DateTimeOffset(2024, 02, 27, 16, 30, 00, 00, TimeSpan.FromHours(8));

		_ = fakeStatisticalTimeUtnsil.GetStatOn(
			Arg.Is(command.ClosedOn))
			.Returns(statOn);

		var sut = new TriggertCheckDataCommandService(
			fakeStatisticalTimeUtnsil,
			fakeRepository);

		await sut.ExecuteAsync(command);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			playerCode: Arg.Is(command.PlayerCode),
			statAt: Arg.Is(statOn),
			startAt: Arg.Is(statOn),
			endAt: Arg.Is(statOn.AddMinutes(5)),
			triggerAt: Arg.Is(statOn.AddMinutes(30)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
