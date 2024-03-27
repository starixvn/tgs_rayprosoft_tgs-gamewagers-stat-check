using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tgs.GameWagers.Stat.Check;

namespace Tgs.GameWagers.Stat.Check;

internal class MongoDBConnectionManager
{
	public IMongoClient MongoClient { get; }

	public IMongoDatabase MongoDatabase { get; }

	public MongoDBConnectionManager(
		IOptions<MongoDBOptions> optionsAccessor)
	{
		var options = optionsAccessor.Value;
		var mongoUrl = new MongoUrl(options.ConnectionString);
		MongoClient = new MongoClient(mongoUrl);
		MongoDatabase = MongoClient.GetDatabase("game_wager_master");
	}
}
