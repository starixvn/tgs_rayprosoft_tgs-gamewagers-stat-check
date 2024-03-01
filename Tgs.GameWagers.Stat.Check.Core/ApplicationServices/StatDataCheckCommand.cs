namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record StatDataCheckCommand(
	string PlayerCode,
	DateTimeOffset StatAt,
	DateTimeOffset StartAt,
	DateTimeOffset EndAt,
	DateTimeOffset TriggerAt);
