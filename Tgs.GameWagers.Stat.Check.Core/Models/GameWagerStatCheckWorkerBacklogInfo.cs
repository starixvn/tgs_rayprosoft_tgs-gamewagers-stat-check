namespace Tgs.GameWagers.Stat.Check.Core.Models;
public record GameWagerStatCheckWorkerBacklogInfo(
	string PlayerCode,
	DateTimeOffset TriggerAt,
	int Count);
