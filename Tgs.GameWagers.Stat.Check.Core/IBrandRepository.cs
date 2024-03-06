using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Core;

public interface IBrandRepository
{
	ValueTask<BrandInfo> GetAsync(string code, CancellationToken cancellationToken = default);

	ValueTask<BrandInfo> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
