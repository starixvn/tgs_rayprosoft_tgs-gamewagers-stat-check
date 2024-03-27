using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core;

public interface IGameWagerStatCheckDataBacklogRepository
{
	ValueTask AddAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, DateTimeOffset triggerAt, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameWagerStatCheckDataBacklogInfo> QueryAsync(DateTimeOffset endOn, int offset = 0, int limit = 20, CancellationToken cancellationToken = default);

	ValueTask DeleteOneAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, DateTimeOffset triggerAt, CancellationToken cancellationToken = default);
}
