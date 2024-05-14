namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record TriggertCheckWorkerCommand(
	string PlayerCode,
	DateTimeOffset ClosedOn);
