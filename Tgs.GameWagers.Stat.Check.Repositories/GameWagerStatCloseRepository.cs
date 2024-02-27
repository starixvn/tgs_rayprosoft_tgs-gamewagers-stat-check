using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Check.Core;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.GameWagers.Stat.Check.Entities;

namespace Tgs.GameWagers.Stat.Check.Repositories;
internal class GameWagerStatCloseRepository(
	TgsContext context) : IGameWagerStatCloseRepository
{
	public async ValueTask AddAsync(Guid resellerId, Guid licenseeId, Guid brandId, string playerCode, Guid gameProviderId, int gameId, DateTimeOffset statOn, int count, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, CancellationToken cancellationToken = default)
	{
		var entity = new GameWagerStatClose
		{
			ResellerId = resellerId,
			LicenseeId = licenseeId,
			BrandId = brandId,
			PlayerCode = playerCode,
			GameProviderId = gameProviderId,
			GameId = gameId,
			StatOn = statOn.UtcDateTime,
			Count = count,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = commission,
			Turnover = turnover,
			WinLoss = winLoss,
			CreatedOn = DateTime.UtcNow,
			UpdatedOn = DateTime.UtcNow
		};

		_ = await context.GameWagerStatCloses.AddAsync(
			entity,
			cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
	public ValueTask<GameWagerStatCloseInfo?> GetAsync(string playerCode, Guid gameProviderId, int gameId, DateTimeOffset statOn, CancellationToken cancellationToken = default)
		=> context.GameWagerStatCloses
		.Where(predicate: close => close.PlayerCode == playerCode &&
		close.GameProviderId == gameProviderId &&
		close.GameId == gameId &&
		close.StatOn == statOn.UtcDateTime)
		.Select(selector: close => new GameWagerStatCloseInfo(
			close.ResellerId,
			close.LicenseeId,
			close.BrandId,
			close.PlayerCode,
			close.GameProviderId,
			close.GameId,
			close.StatOn,
			close.Count,
			close.Bet,
			close.ValidBet,
			close.Settle,
			close.Commission,
			close.Turnover,
			close.WinLoss))
		.ToAsyncEnumerable()
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken);
	public async ValueTask UpdateAsync(string playerCode, Guid gameProviderId, int gameId, DateTimeOffset statOn, int count, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, CancellationToken cancellationToken = default)
	{
		var gameWagerStatClose = await context.GameWagerStatCloses
			.Where(predicate: close => close.PlayerCode == playerCode &&
			close.GameProviderId == gameProviderId &&
			close.GameId == gameId &&
			close.StatOn == statOn.UtcDateTime)
			.SingleAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		gameWagerStatClose.Count = count;
		gameWagerStatClose.Bet = bet;
		gameWagerStatClose.ValidBet = validBet;
		gameWagerStatClose.Settle = settle;
		gameWagerStatClose.Commission = commission;
		gameWagerStatClose.Turnover = turnover;
		gameWagerStatClose.WinLoss = winLoss;

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
