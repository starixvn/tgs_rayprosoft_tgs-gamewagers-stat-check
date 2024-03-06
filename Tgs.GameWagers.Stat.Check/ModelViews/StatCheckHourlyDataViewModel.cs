using System.ComponentModel.DataAnnotations;

namespace Tgs.GameWagers.Stat.Check.ModelViews;

public record StatCheckHourlyDataViewModel
{
	[Required]
	public DateTimeOffset StartAt { get; init; }
	[Required]
	public DateTimeOffset EndAt { get; init; }
}
