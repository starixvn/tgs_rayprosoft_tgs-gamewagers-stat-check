using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Level.Common;
using Valhalla.MessageQueue;
using Tgs.GameWagers.Stat.Check.Common.Protos;

namespace Tgs.GameWagers.Stat.Check.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	TimeProvider timeProvider,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{

		var message = new GameWagersStatCheckTriggerMessage
		{
			TriggerAt = timeProvider.GetUtcNow().ToTimestamp()
		};

		logger.LogInformation("LogOn:{logOn} Publish Message:{message}", DateTime.Now.ToString("u"), message);

		return messageSender.PublishAsync<GameWagersStatCheckTriggerMessage>(
			subject: GameWagersStatCheckNatsConst.Trigger,
			data: message,
			cancellationToken: cancellationToken);
	}
}
