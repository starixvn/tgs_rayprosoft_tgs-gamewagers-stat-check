using System.Runtime.CompilerServices;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class GameWagerStatQueryService(
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerRepository repository) : IQueryService<GameWagerStatQuery, GameWagerStatResult>
{
	public async IAsyncEnumerable<GameWagerStatResult> QueryAsync(GameWagerStatQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		(DateTimeOffset StartOn, DateTimeOffset EndOn) = statisticalTimeUtnsil.GetStatInterval(query.StartOn, query.EndOn);
		var infos = await repository.QueryAsync(
			startOn: StartOn,
			endOn: EndOn,
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerStatInfo>();

		if (!infos.Any())
		{
			yield break;
		}

		foreach (var info in infos)
		{
			yield return new GameWagerStatResult(
				ResellerId: info.ResellerId,
				LicenseeId: info.LicenseeId,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				GameProviderId: info.GameProviderId,
				GameId: info.GameId,
				StatOn: info.StatOn,
				Count: info.Count,
				Bet: info.Bet,
				ValidBet: info.ValidBet,
				Settle: info.Settle,
				Commission: info.Commission,
				Turnover: info.Turnover,
				WinLoss: info.WinLoss);
		}
	}
}
