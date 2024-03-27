namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record StatCheckWorkerBacklogCommand(
	DateTimeOffset TriggerAt);
