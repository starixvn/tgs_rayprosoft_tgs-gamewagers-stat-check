using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core;
public interface IGameWagerStatCloseRepository
{
	public ValueTask<GameWagerStatCloseInfo?> GetAsync(string playerCode, Guid gameProviderId, int gameId, DateTimeOffset statOn, CancellationToken cancellationToken = default);

	public ValueTask AddAsync(Guid resellerId, Guid licenseeId, Guid brandId, string playerCode, Guid gameProviderId, int gameId, DateTimeOffset statOn, int count, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, CancellationToken cancellationToken = default);

	public ValueTask UpdateAsync(string playerCode, Guid gameProviderId, int gameId, DateTimeOffset statOn, int count, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, CancellationToken cancellationToken = default);
}
