using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class StatCheckHourlyDataCommandService(
	ILogger<StatCheckHourlyDataCommandService> logger,
	IGameBetCloseRepository gameWagerRepository,
	IGameWagerStatCloseRepository gameWagerStatCloseRepository,
	ICommandService<GameWagerStatCloseAddCommand> addService,
	ICommandService<GameWagerStatCloseUpdateCommand> updateService) : ICommandService<StatCheckHourlyDataCommand>
{
	public async ValueTask ExecuteAsync(StatCheckHourlyDataCommand command, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Mesaage:This operation begins.({startAt}-{endAt})",
			DateTime.Now.ToString("O"),
			command.StartAt,
			command.EndAt);

		int offset = 0;
		int limit = 20;
		IEnumerable<GameWagerStatInfo> gameWagerStatInfos;
		do
		{
			gameWagerStatInfos = await gameWagerRepository.QueryAsync(
			   startOn: command.StartAt,
			   endOn: command.EndAt,
			   offset: offset,
			   limit: limit,
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
						DateTime.Now.ToString("O"),
						info);

					await addService.ExecuteAsync(
						command: new GameWagerStatCloseAddCommand(
							ResellerId: info.ResellerId,
							LicenseeId: info.LicenseeId,
							BrandId: info.BrandId,
							PlayerCode: info.PlayerCode,
							GameProviderId: info.GameProviderId,
							GameId: info.GameId,
							StatOn: info.StatOn,
							Count: info.Count,
							Bet: info.Bet,
							ValidBet: info.ValidBet,
							Settle: info.Settle,
							Commission: info.Commission,
							Turnover: info.Turnover,
							WinLoss: info.WinLoss),
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
							DateTime.Now.ToString("O"),
							JsonSerializer.Serialize(gameWagerStatCloseInfo),
							JsonSerializer.Serialize(info));

						await updateService.ExecuteAsync(
							command: new GameWagerStatCloseUpdateCommand(
								ResellerId: info.ResellerId,
								LicenseeId: info.LicenseeId,
								BrandId: info.BrandId,
								PlayerCode: info.PlayerCode,
								GameProviderId: info.GameProviderId,
								GameId: info.GameId,
								StatOn: info.StatOn,
								Count: info.Count,
								Bet: info.Bet,
								ValidBet: info.ValidBet,
								Settle: info.Settle,
								Commission: info.Commission,
								Turnover: info.Turnover,
								WinLoss: info.WinLoss),
							cancellationToken: cancellationToken)
							.ConfigureAwait(false);
					}
				}
			}

			offset++;
		} while (gameWagerStatInfos.Any());

		logger.LogInformation("LogOn:{logOn} Mesaage:This operation ends.({startAt}-{endAt})",
			DateTime.Now.ToString("O"),
			command.StartAt,
			command.EndAt);
	}
}
