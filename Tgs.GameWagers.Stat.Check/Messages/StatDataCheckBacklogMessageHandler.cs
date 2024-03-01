using Tgs.ServiceUtility;

using Valhalla.MessageQueue;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Check.Common.Protos;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class StatDataCheckBacklogMessageHandler(
	ILogger<StatDataCheckBacklogMessageHandler> logger,
	ICommandService<StatDataCheckBacklogCommand> service) : IMessageHandler<GameWagersStatCheckTriggerMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatCheckTriggerMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new StatDataCheckBacklogCommand(
				TriggerAt: data.TriggerAt.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
