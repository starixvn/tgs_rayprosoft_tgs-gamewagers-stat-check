using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tgs.GameWagers.Stat.Check.Core;
using Tgs.GameWagers.Stat.Check.Core.Models;

namespace Tgs.GameWagers.Stat.Check.Repositories;
internal class GameWagerRepository(
	IDbConnection connection) : IGameWagerRepository
{
	public async IAsyncEnumerable<GameWagerStatInfo> QueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		DynamicParameters parameters = new DynamicParameters();
		parameters.Add("@createdStartOn", DateTimeOffset.UtcNow.AddMonths(-3).UtcDateTime, DbType.DateTimeOffset, ParameterDirection.Input);
		parameters.Add("@createdEndOn", DateTimeOffset.UtcNow.UtcDateTime, DbType.DateTimeOffset, ParameterDirection.Input);
		parameters.Add("@startOn", startOn.UtcDateTime, DbType.DateTimeOffset, ParameterDirection.Input);
		parameters.Add("@endOn", endOn.UtcDateTime, DbType.DateTimeOffset, ParameterDirection.Input);
		parameters.Add("@playerCode", playerCode, DbType.String, ParameterDirection.Input);

		var sqlQuery = @"
        select
	        reseller_id as ""reseller_id"",
	        licensee_id as ""licensee_id"",
	        brand_id as ""brand_id"",
	        player_code as ""player_code"",
	        game_provider_id as ""game_provider_id"",
            game_id as ""game_id"",
	        date_trunc('hour',
	        x.closed_on) + date_part('minute',
	        x.closed_on)::int / 5 * interval '5 min' as ""stat_on"" ,
	        count(1) as ""count"",
	        sum(x.bet) as ""bet"",
	        sum(x.valid_bet) as ""valid_bet"",
	        sum(x.settle) as ""settle"",
	        sum(x.commission) as ""commission"",
	        sum(x.turnover) as ""turnover"",
	        sum(x.win_loss) as ""win_loss""
        from
	        dbo.game_wagers x
        where
	        x.created_on >= @createdStartOn
	        and x.created_on < @createdEndOn
	        and x.closed_on >= @startOn
	        and x.closed_on < @endOn
	        and x.player_code = @playerCode
        group by
	        reseller_id ,
	        licensee_id ,
	        brand_id ,
	        player_code,
	        game_provider_id,
            game_id,
	        7;";

		var command = new CommandDefinition(
			commandText: sqlQuery,
			parameters: parameters,
			commandTimeout: (int)TimeSpan.FromMinutes(10).TotalSeconds,
			cancellationToken: cancellationToken);

		var infos = await connection.QueryAsync<GameWagerStatInfo>(
			command: command)
			.ConfigureAwait(false);

		if (!infos.Any())
		{
			yield break;
		}

		foreach (var info in infos)
		{
			yield return info;
		}
	}
}
