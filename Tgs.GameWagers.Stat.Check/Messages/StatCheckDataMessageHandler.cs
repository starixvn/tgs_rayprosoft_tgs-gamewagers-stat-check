using Tgs.GameWagers.Stat.Check.Common.Protos;

using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

using Tgs.ServiceUtility;

using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class StatCheckDataMessageHandler(
	ILogger<StatCheckDataMessageHandler> logger,
	ICommandService<StatCheckDataCommand> service) : IMessageHandler<GameWagersStatCheckCheckDataMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatCheckCheckDataMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new StatCheckDataCommand(
				PlayerCode: data.PlayerCode,
				StatAt: data.StatAt.ToDateTimeOffset(),
				StartAt: data.StartAt.ToDateTimeOffset(),
				EndAt: data.EndAt.ToDateTimeOffset(),
				TriggerAt: data.TriggerAt.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
