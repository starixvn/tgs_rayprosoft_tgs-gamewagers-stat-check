using NSubstitute;

using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core.UnitTest.ApplicationServices;
public class GameWagerStatQueryServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
        var fakeGameWagerRepository = Substitute.For<IGameWagerRepository>();

        var query = new GameWagerStatQuery(
            PlayerCode: "4ucmqplxb029",
            StartOn: new DateTimeOffset(2024, 02, 27, 13, 00, 15, 00, TimeSpan.FromHours(8)),
            EndOn: new DateTimeOffset(2024, 02, 27, 13, 00, 20, 00, TimeSpan.FromHours(8)));

        (DateTimeOffset StartOn, DateTimeOffset EndOn) = (
            new DateTimeOffset(2024, 02, 27, 13, 00, 15, 00, TimeSpan.FromHours(8)),
            new DateTimeOffset(2024, 02, 27, 13, 00, 20, 00, TimeSpan.FromHours(8)));

        _ = fakeStatisticalTimeUtnsil.GetInterval(
            Arg.Is(query.StartOn),
            Arg.Is(query.EndOn))
            .Returns((StartOn, EndOn));

        var gameWagerStatInfo = new GameWagerStatInfo[]
        {
            new() {
                ResellerId=Guid.NewGuid(),
                LicenseeId=Guid.NewGuid(),
                BrandId=Guid.NewGuid(),
                PlayerCode=query.PlayerCode,
                GameProviderId=Guid.NewGuid(),
                GameId=1,
                StatOn=new DateTimeOffset(2024, 02, 27, 13, 00, 15, 00, TimeSpan.FromHours(8)),
                Count=100,
                Bet=1000.00m,
                ValidBet=1000.00m,
                Settle=500.00m,
                Commission=0.00m,
                Turnover=1000.00m,
                WinLoss=-500.00m
            }
        };
        _ = fakeGameWagerRepository.QueryAsync(
            playerCode: Arg.Is(query.PlayerCode),
            startOn: Arg.Is(StartOn),
            endOn: Arg.Is(EndOn),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameWagerStatInfo.ToAsyncEnumerable());
        var sut = new GameWagerStatQueryService(
            fakeStatisticalTimeUtnsil,
            fakeGameWagerRepository);

        var actual = await sut.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerStatResult>();

        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.Single(actual);

    }
}
