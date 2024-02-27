using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameWagers.Stat.Check.Core;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.GameWagers.Stat.Check.ModelViews;

namespace Tgs.GameWagers.Stat.Check.Controllers;
[Route("[controller]")]
[ApiController]
public class GameWagerStatBacklogController : ControllerBase
{
	[HttpGet]
	public IAsyncEnumerable<GameWagerStatBacklogInfo> QueryAsync(
		[FromServices] IGameWagerStatBacklogRepository repository,
		[FromQuery] DateTime endOn,
		[FromQuery] int offset = 0,
		[FromQuery] int limit = 20)
		=> repository.QueryAsync(
			endOn: endOn,
			offset: offset,
			limit: limit,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Adds the asynchronous.
	/// </summary>
	/// <param name="repository">The repository.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "playerCode": "4ucmqplxb029",
	///             "StatAt": "2024-02-27T16:20:00.000+08:00",
	///             "StartAt": "2024-02-27T16:20:00.000+08:00",
	///             "EndAt": "2024-02-27T16:25:00.000+08:00",
	///             "TriggerAt": "2024-02-27T16:30:00.000+08:00"
	///         }
	/// </remarks>
	[HttpPost]
	public ValueTask AddAsync(
		[FromServices] IGameWagerStatBacklogRepository repository,
		[FromBody] GameWagerStatBacklogAddModelView source)
		=> repository.AddAsync(
			playerCode: source.PlayerCode,
			statAt: source.StatAt,
			startAt: source.StartAt,
			endAt: source.EndAt,
			triggerAt: source.TriggerAt,
			cancellationToken: HttpContext.RequestAborted);
	/// <summary>
	/// Posts the asynchronous.
	/// </summary>
	/// <param name="repository">The repository.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Patch /4ucmqplxb029/expireat
	///         {
	///             "StatAt": "2024-02-27T16:20:00.000+08:00",
	///             "StartAt": "2024-02-27T16:20:00.000+08:00",
	///             "EndAt": "2024-02-27T16:25:00.000+08:00"
	///         }
	/// </remarks>
	[HttpPatch("{playerCode}/expireat")]
	public ValueTask PostAsync(
		[FromServices] IGameWagerStatBacklogRepository repository,
		string playerCode,
		[FromBody] GameWagerStatBacklogUpdateExpireModelView source)
		=> repository.UpdateExpireAtAsync(
			playerCode: playerCode,
			statAt: source.StatAt,
			startAt: source.StartAt,
			endAt: source.EndAt,
			cancellationToken: HttpContext.RequestAborted);
}
