using Tgs.ServiceUtility;

using Valhalla.MessageQueue;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Check.Common.Protos;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class CheckPlayerMessageHandler(
	ILogger<CheckPlayerMessageHandler> logger,
	ICommandService<CheckPlayerCommand> service) : IMessageHandler<GameWagersStatCheckCheckPlayerMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatCheckCheckPlayerMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new CheckPlayerCommand(
				PlayerCode: data.PlayerCode,
				StatAt: data.StatAt.ToDateTimeOffset(),
				StartAt: data.StartAt.ToDateTimeOffset(),
				EndAt: data.EndAt.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
