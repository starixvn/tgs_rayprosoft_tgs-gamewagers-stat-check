using Microsoft.AspNetCore.Mvc;
using Tgs.GameWagers.Stat.Check.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Check.Controllers;

[ApiController]
[Route("[controller]")]
public class GameWagerController : ControllerBase
{

	[HttpGet("{playerCode}", Name = "GetWeatherForecast")]
	public IAsyncEnumerable<GameWagerStatResult> QueryAsync(
		[FromServices] IQueryService<GameWagerStatQuery, GameWagerStatResult> service,
		string playerCode,
		[FromQuery] DateTimeOffset startOn,
		[FromQuery] DateTimeOffset endOn)
	=> service.QueryAsync(
		query: new GameWagerStatQuery(
			PlayerCode: playerCode,
			StartOn: startOn,
			EndOn: endOn),
		cancellationToken: HttpContext.RequestAborted);

}
