namespace Tgs.GameWagers.Stat.Check.ModelViews;

public record GameWagerStatBacklogUpdateExpireModelView
{
	public DateTimeOffset StatAt { get; init; }
	public DateTimeOffset StartAt { get; init; }
	public DateTimeOffset EndAt { get; init; }
}
