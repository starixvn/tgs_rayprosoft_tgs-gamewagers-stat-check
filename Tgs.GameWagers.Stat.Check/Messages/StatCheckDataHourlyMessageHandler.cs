using Tgs.GameWagers.Stat.Check.Common.Protos;

using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;

using Tgs.ServiceUtility;

using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Messages;

public class StatCheckDataHourlyMessageHandler(
	ILogger<StatCheckDataHourlyMessageHandler> logger,
	ICommandService<StatCheckHourlyDataCommand> service) : IMessageHandler<GameWagersStatCheckDataHourlyMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatCheckDataHourlyMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new StatCheckHourlyDataCommand(
				StartAt: data.StartAt.ToDateTimeOffset(),
				EndAt: data.EndAt.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
