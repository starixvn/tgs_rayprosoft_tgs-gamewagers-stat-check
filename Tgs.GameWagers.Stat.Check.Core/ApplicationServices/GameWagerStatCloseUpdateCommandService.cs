using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;
using Google.Protobuf.WellKnownTypes;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
internal class GameWagerStatCloseUpdateCommandService(
	IBrandRepository brandRepository,
	IGameWagerStatCloseRepository gameWagerStatCloseRepository,
	IMessageSender messageSender) : ICommandService<GameWagerStatCloseUpdateCommand>
{
	public async ValueTask ExecuteAsync(GameWagerStatCloseUpdateCommand command, CancellationToken cancellationToken = default)
	{
		var brand = await brandRepository.GetAsync(
			id: command.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await gameWagerStatCloseRepository.UpdateAsync(
			playerCode: command.PlayerCode,
			gameProviderId: command.GameProviderId,
			gameId: command.GameId,
			statOn: command.StatOn,
			count: command.Count,
			bet: command.Bet,
			validBet: command.ValidBet,
			settle: command.Settle,
			commission: command.Commission,
			turnover: command.Turnover,
			winLoss: command.WinLoss,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await messageSender.PublishAsync(
			subject: GameWagersStatHourlyNatsConst.StatHourlyClose,
			data: new GameWagersStatHourlyMessage
			{
				ResellerId = brand.ResellerId,
				ResellerCode = brand.ResellerCode,
				LicenseeId = brand.LicenseeId,
				LicenseeCode = brand.LicenseeCode,
				BrandId = brand.BrandId,
				BrandCode = brand.BrandCode,
				TimeSpanMinutes = brand.TimeSpanMinutes,
				StartOn = command.StatOn.AddHours(-1).ToTimestamp(),
				EndOn = command.StatOn.ToTimestamp()
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
