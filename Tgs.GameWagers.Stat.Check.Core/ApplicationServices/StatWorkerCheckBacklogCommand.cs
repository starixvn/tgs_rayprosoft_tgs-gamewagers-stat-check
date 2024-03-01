namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record StatWorkerCheckBacklogCommand(
	DateTimeOffset TriggerAt);
