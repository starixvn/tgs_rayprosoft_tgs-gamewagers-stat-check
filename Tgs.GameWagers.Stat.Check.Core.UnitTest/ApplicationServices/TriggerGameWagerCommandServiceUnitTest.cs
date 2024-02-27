using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;
public class TriggerGameWagerCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var fakeLog = NullLogger<TriggerGameWagerCommandService>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerRepository = Substitute.For<IGameWagerStatBacklogRepository>();

		var command = new TriggerGameWagerCommand(
			PlayerCode: "4ucmqplxb029",
			ClosedOn: new DateTimeOffset(2024, 02, 27, 16, 34, 38, 00, TimeSpan.FromHours(8)));

		var statOn = new DateTimeOffset(2024, 02, 27, 16, 30, 00, 00, TimeSpan.FromHours(8));

		_ = fakeStatisticalTimeUtnsil.GetStatOn(
			Arg.Is(command.ClosedOn))
			.Returns(statOn);

		var sut = new TriggerGameWagerCommandService(
			fakeLog,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerRepository);

		await sut.ExecuteAsync(command);

		_ = fakeGameWagerRepository
			.Received(1)
			.AddAsync(
			Arg.Is(command.PlayerCode),
			Arg.Is(statOn),
			Arg.Is(statOn),
			Arg.Is(statOn.AddMinutes(5)),
			Arg.Is(statOn.AddMinutes(10)),
			Arg.Any<CancellationToken>());
	}
}
