using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameWagers.Stat.Check.Common;

using Tgs.GameWagers.Stat.Check.Common.Protos;
using Tgs.GameWagers.Stat.Check.ModelViews;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Check.Controllers;

[Route("stat-check-hourly-data")]
[ApiController]
public class StatCheckHourlyDataController : ControllerBase
{
	/// <summary>
	/// Executes the asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "startAt": "2024-03-05T20:00:00.000+08:00",
	///             "endAt": "2024-03-05T21:00:00.000+08:00"
	///         }
	/// </remarks>
	[HttpPost(Name = "StatCheckHourlyData_Execute")]
	public ValueTask ExecuteAsync(
		[FromServices] IMessageSender messageSender,
		StatCheckHourlyDataViewModel source)
		=> messageSender.PublishAsync(
			subject: GameWagersStatCheckNatsConst.StatCheckHourlyData,
			data: new GameWagersStatCheckDataHourlyMessage
			{
				StartAt = source.StartAt.ToTimestamp(),
				EndAt = source.EndAt.ToTimestamp()
			},
			cancellationToken: HttpContext.RequestAborted);
}
