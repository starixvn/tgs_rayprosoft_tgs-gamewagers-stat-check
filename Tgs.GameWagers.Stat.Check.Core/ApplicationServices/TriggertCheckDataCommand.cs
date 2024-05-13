namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
public record TriggertCheckDataCommand(
	Guid GameBetId,
	Guid GameProviderId,
	int GameId,
	Guid ResellerId,
	Guid LicenseeId,
	Guid BrandId,
	string PlayerCode,
	string CurrencyCode,
	Guid WalletId,
	string ExternalRoundId,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn);
