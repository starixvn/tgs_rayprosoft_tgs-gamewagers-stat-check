using Tgs.GameWagers.Stat.Check.Common.Protos;

using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

using Tgs.ServiceUtility;

using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class StatCheckWorkerBacklogMessageHandler(
	ILogger<StatCheckWorkerBacklogMessageHandler> logger,
	ICommandService<StatCheckWorkerBacklogCommand> service) : IMessageHandler<GameWagersStatCheckTriggerMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatCheckTriggerMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new StatCheckWorkerBacklogCommand(
				TriggerAt: data.TriggerAt.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
