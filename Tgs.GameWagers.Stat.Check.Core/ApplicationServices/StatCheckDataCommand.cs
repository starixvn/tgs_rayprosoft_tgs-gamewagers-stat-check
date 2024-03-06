namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record StatCheckDataCommand(
	string PlayerCode,
	DateTimeOffset StatAt,
	DateTimeOffset StartAt,
	DateTimeOffset EndAt,
	DateTimeOffset TriggerAt);
