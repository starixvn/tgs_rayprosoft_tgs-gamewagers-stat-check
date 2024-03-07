using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Check.Common.Protos;

using Tgs.ServiceUtility;
using Valhalla.MessageQueue;
using Tgs.GameWagers.Stat.Check.Common;

namespace Tgs.GameWagers.Stat.Check.Core.JobApplicationServices;

internal class JobExecuteStatCheckHourlyCommandService(
	ILogger<JobExecuteStatCheckHourlyCommandService> logger,
	TimeProvider timeProvider,
	IMessageSender messageSender) : ICommandService<JobExecuteStatCheckHourlyCommand>
{
	public ValueTask ExecuteAsync(JobExecuteStatCheckHourlyCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var startAt = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour - 1, 00, 00, currentTime.Offset);
		var endAt = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 00, 00, currentTime.Offset);

		var message = new GameWagersStatCheckDataHourlyMessage
		{
			StartAt = startAt.ToTimestamp(),
			EndAt = endAt.ToTimestamp()
		};

		logger.LogInformation("LogOn:{logOn} Publish Message:{message}", DateTime.Now.ToString("u"), message);

		return messageSender.PublishAsync(
			subject: GameWagersStatCheckNatsConst.StatCheckHourlyData,
			data: message,
			cancellationToken: cancellationToken);
	}
}
