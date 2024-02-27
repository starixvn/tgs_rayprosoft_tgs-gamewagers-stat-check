namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record GameWagerStatResult(
	Guid ResellerId,
	Guid LicenseeId,
	Guid BrandId,
	string PlayerCode,
	Guid GameProviderId,
	int GameId,
	DateTimeOffset StatOn,
	int Count,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss);
