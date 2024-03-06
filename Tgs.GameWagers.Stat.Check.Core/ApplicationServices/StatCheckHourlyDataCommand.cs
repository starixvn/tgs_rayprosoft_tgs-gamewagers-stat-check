namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record StatCheckHourlyDataCommand(
	DateTimeOffset StartAt,
	DateTimeOffset EndAt);
