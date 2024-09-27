using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core;

public interface IGameBetCloseRepository
{
	IAsyncEnumerable<GameWagerStatInfo> QueryAsync(DateTimeOffset startOn, DateTimeOffset endOn, string playerCode, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameWagerStatInfo> QueryAsync(DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameWagerStatInfo> QueryAsync(DateTimeOffset startOn, DateTimeOffset endOn, int offset = 0, int limit = 20, CancellationToken cancellationToken = default);
}
