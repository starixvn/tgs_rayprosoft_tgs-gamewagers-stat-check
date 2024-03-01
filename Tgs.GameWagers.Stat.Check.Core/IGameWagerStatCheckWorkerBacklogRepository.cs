using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core;
public interface IGameWagerStatCheckWorkerBacklogRepository
{
	ValueTask AddAsync(string playerCode, DateTimeOffset triggerAt, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameWagerStatCheckWorkerBacklogInfo> QueryAsync(DateTimeOffset endOn, int offset = 0, int limit = 20, CancellationToken cancellationToken = default);

	ValueTask UpdateExpireAtAsync(string playerCode, DateTimeOffset triggerAt, CancellationToken cancellationToken = default);
}
