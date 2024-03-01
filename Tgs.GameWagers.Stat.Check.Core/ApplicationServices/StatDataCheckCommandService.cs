using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
internal class StatDataCheckCommandService(
	ILogger<StatDataCheckCommandService> logger,
	IGameWagerRepository gameWagerRepository,
	IGameWagerStatCloseRepository gameWagerStatCloseRepository) : ICommandService<StatDataCheckCommand>
{
	public async ValueTask ExecuteAsync(StatDataCheckCommand command, CancellationToken cancellationToken = default)
	{
		var gameWagerStatInfos = await gameWagerRepository.QueryAsync(
			command.PlayerCode,
			command.StartAt,
			command.EndAt,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerStatInfo>();

		foreach (var info in gameWagerStatInfos)
		{
			var gameWagerStatCloseInfo = await gameWagerStatCloseRepository.GetAsync(
				playerCode: info.PlayerCode,
				gameProviderId: info.GameProviderId,
				gameId: info.GameId,
				statOn: info.StatOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			if (gameWagerStatCloseInfo is null)
			{
				logger.LogWarning("LogOn:{logOn} Mesaage:This new game wager stat close information. Add Info({info})",
					DateTime.Now.ToString("o"),
					info);

				await gameWagerStatCloseRepository.AddAsync(
					resellerId: info.ResellerId,
					licenseeId: info.LicenseeId,
					brandId: info.BrandId,
					playerCode: info.PlayerCode,
					gameProviderId: info.GameProviderId,
					gameId: info.GameId,
					statOn: info.StatOn,
					count: info.Count,
					bet: info.Bet,
					validBet: info.ValidBet,
					settle: info.Settle,
					commission: info.Commission,
					turnover: info.Turnover,
					winLoss: info.WinLoss,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
			else
			{
				if (info.Count != gameWagerStatCloseInfo.Count ||
					info.Bet != gameWagerStatCloseInfo.Bet ||
					info.ValidBet != gameWagerStatCloseInfo.ValidBet ||
					info.Settle != gameWagerStatCloseInfo.Settle ||
					info.Commission != gameWagerStatCloseInfo.Commission ||
					info.Turnover != gameWagerStatCloseInfo.Turnover ||
					info.WinLoss != gameWagerStatCloseInfo.WinLoss)
				{
					logger.LogWarning("LogOn:{logOn} Mesaage:This updates game wager stat close information. Old Info:{old}  Update Info:{update}",
						DateTime.Now.ToString("o"),
						JsonSerializer.Serialize(gameWagerStatCloseInfo),
						JsonSerializer.Serialize(info));

					await gameWagerStatCloseRepository.UpdateAsync(
						playerCode: info.PlayerCode,
						gameProviderId: info.GameProviderId,
						gameId: info.GameId,
						statOn: info.StatOn,
						count: info.Count,
						bet: info.Bet,
						validBet: info.ValidBet,
						settle: info.Settle,
						commission: info.Commission,
						turnover: info.Turnover,
						winLoss: info.WinLoss,
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
			}
		}
	}
}
