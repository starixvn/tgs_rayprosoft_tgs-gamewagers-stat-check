using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

using Tgs.GameWagers.Stat.Hourly.Common;

using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

internal class GameWagerStatCloseAddCommandService(
	ILogger<GameWagerStatCloseAddCommandService> logger,
	IBrandRepository brandRepository,
	IGameWagerStatCloseRepository gameWagerStatCloseRepository,
	IMessageSender messageSender) : ICommandService<GameWagerStatCloseAddCommand>
{
	public async ValueTask ExecuteAsync(GameWagerStatCloseAddCommand command, CancellationToken cancellationToken = default)
	{
		var brand = await brandRepository.GetAsync(
			id: command.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await gameWagerStatCloseRepository.AddAsync(
			resellerId: command.ResellerId,
			licenseeId: command.LicenseeId,
			brandId: command.BrandId,
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

		var message = new GameWagersStatHourlyMessage
		{
			ResellerId = brand.ResellerId,
			ResellerCode = brand.ResellerCode,
			LicenseeId = brand.LicenseeId,
			LicenseeCode = brand.LicenseeCode,
			BrandId = brand.BrandId,
			BrandCode = brand.BrandCode,
			TimeSpanMinutes = brand.TimeSpanMinutes,
			StartOn = command.StatOn.AddMinutes(-5).ToTimestamp(),
			EndOn = command.StatOn.AddMinutes(+5).ToTimestamp()
		};

		await messageSender.PublishAsync(
			subject: GameWagersStatHourlyNatsConst.StatHourlyClose,
			data: message,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:The content of this message.({message})",
			DateTime.Now.ToString("u"),
			GameWagersStatHourlyNatsConst.StatHourlyClose,
			message);
	}
}
