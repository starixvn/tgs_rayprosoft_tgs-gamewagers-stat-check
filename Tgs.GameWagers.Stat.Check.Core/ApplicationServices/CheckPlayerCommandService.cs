using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
internal class CheckPlayerCommandService(
	IGameWagerRepository gameWagerRepository,
	IGameWagerStatCloseRepository gameWagerStatCloseRepository,
	IGameWagerStatBacklogRepository gameWagerStatBacklogRepository) : ICommandService<CheckPlayerCommand>
{
	public async ValueTask ExecuteAsync(CheckPlayerCommand command, CancellationToken cancellationToken = default)
	{
		var infos = await gameWagerRepository.QueryAsync(
			command.PlayerCode,
			command.StartAt,
			command.EndAt,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerStatInfo>();

		foreach (var info in infos)
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

		await gameWagerStatBacklogRepository.UpdateExpireOnAsync(
			command.PlayerCode,
			command.StatAt,
			command.StartAt,
			command.EndAt,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
