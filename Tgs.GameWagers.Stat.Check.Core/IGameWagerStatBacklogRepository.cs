using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core;
public interface IGameWagerStatBacklogRepository
{
	ValueTask AddAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, DateTimeOffset triggerAt, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameWagerStatBacklogInfo> QueryAsync(DateTimeOffset endOn, int offset = 0, int limit = 20, CancellationToken cancellationToken = default);

	ValueTask UpdateExpireOnAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, CancellationToken cancellationToken = default);
}
