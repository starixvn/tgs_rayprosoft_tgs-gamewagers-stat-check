using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.ColumnMapper;

namespace Tgs.GameWagers.Stat.Check.Core.Models;
public record GameWagerStatInfo
{
	[ColumnMapping(columnName: "reseller_id")]
	public Guid ResellerId { get; set; }

	[ColumnMapping(columnName: "licensee_id")]
	public Guid LicenseeId { get; set; }

	[ColumnMapping(columnName: "brand_id")]
	public Guid BrandId { get; set; }

	[ColumnMapping(columnName: "player_code")]
	public string PlayerCode { get; set; } = default!;

	[ColumnMapping(columnName: "game_provider_id")]
	public Guid GameProviderId { get; set; }

	[ColumnMapping(columnName: "game_id")]
	public int GameId { get; set; }

	[ColumnMapping(columnName: "stat_on")]
	public DateTimeOffset StatOn { get; set; }

	[ColumnMapping(columnName: "count")]
	public int Count { get; set; }

	[ColumnMapping(columnName: "bet")]
	public decimal Bet { get; set; }

	[ColumnMapping(columnName: "valid_bet")]
	public decimal ValidBet { get; set; }

	[ColumnMapping(columnName: "settle")]
	public decimal Settle { get; set; }

	[ColumnMapping(columnName: "commission")]
	public decimal Commission { get; set; }

	[ColumnMapping(columnName: "turnover")]
	public decimal Turnover { get; set; }

	[ColumnMapping(columnName: "win_loss")]
	public decimal WinLoss { get; set; }
}
