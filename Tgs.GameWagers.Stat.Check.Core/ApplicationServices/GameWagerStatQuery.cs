namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record GameWagerStatQuery(
	string PlayerCode,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn);
