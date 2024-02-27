using System.Runtime.CompilerServices;
using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core;

public interface IGameWagerRepository
{
	IAsyncEnumerable<GameWagerStatInfo> QueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);
}
