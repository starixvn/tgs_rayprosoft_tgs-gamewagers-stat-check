namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record CheckPlayerCommand(
	string PlayerCode,
	DateTimeOffset StatAt,
	DateTimeOffset StartAt,
	DateTimeOffset EndAt,
	DateTimeOffset TriggerAt);
