namespace Tgs.GameWagers.Stat.Check.ModelViews;

public record GameWagerStatBacklogAddModelView
{
	public string PlayerCode { get; init; } = default!;
	public DateTimeOffset StatAt { get; init; }
	public DateTimeOffset StartAt { get; init; }
	public DateTimeOffset EndAt { get; init; }
	public DateTimeOffset TriggerAt { get; init; }
}
