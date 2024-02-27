using System.Runtime.CompilerServices;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly;
using Tgs.GameWagers.Stat.Check.Core;
using Tgs.GameWagers.Stat.Check.Core.Extensions;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.GameWagers.Stat.Check.Repositories.Models;

namespace Tgs.GameWagers.Stat.Check.Repositories;
internal class GameWagerStatBacklogRepository(
	ILogger<GameWagerStatBacklogRepository> logger,
	IMongoDatabase mongoDatabase) : IGameWagerStatBacklogRepository
{
	public async ValueTask AddAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, DateTimeOffset triggerAt, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatBacklog>.Filter.Eq(x => x.PlayerCode, playerCode)
			& Builders<GameWagerStatBacklog>.Filter.Eq(x => x.StatAt, statAt.UtcDateTime)
			& Builders<GameWagerStatBacklog>.Filter.Eq(x => x.StartAt, startAt.UtcDateTime)
			& Builders<GameWagerStatBacklog>.Filter.Eq(x => x.EndAt, endAt.UtcDateTime);

		var update = Builders<GameWagerStatBacklog>.Update
			.Set(s => s.PlayerCode, playerCode)
			.Set(s => s.StatAt, statAt.UtcDateTime)
			.Set(s => s.StartAt, startAt.UtcDateTime)
			.Set(s => s.EndAt, endAt.UtcDateTime)
			.Set(s => s.TriggerAt, triggerAt.UtcDateTime)
			.Set(s => s.ExpireAt, DateTime.UtcNow.AddDays(1))
			.Inc(s => s.Count, 1);

		var options = new UpdateOptions
		{
			IsUpsert = true
		};

		var retryPolicy = Policy
		   .Handle<MongoBulkWriteException>()
		   .WaitAndRetryAsync(
			retryCount: 3,
			retryAttempt => TimeSpan.FromSeconds(5),
			(result, retryTime, context) => logger.LogWarning("LogOn:{logOn}  PlayerCode:{playerCode} TriggerAt:{statAt} Result:{result} Delay:{retryTime}",
				DateTime.Now.ToString("o"),
				playerCode,
				statAt,
				result,
				retryTime));

		_ = await retryPolicy.ExecuteAsync(
			() => collection.UpdateOneAsync(
				filter: filter,
				update: update,
				options: options,
				cancellationToken: cancellationToken))
			.ConfigureAwait(false);
	}
	public async IAsyncEnumerable<GameWagerStatBacklogInfo> QueryAsync(DateTimeOffset endOn, int offset = 0, int limit = 20, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatBacklog>.Filter.Lt(x => x.TriggerAt, endOn.UtcDateTime);

		var sort = Builders<GameWagerStatBacklog>
			.Sort
			.Ascending(x => x.Id);

		var options = new FindOptions<GameWagerStatBacklog>
		{
			Sort = sort,
			Skip = offset,
			Limit = limit,
		};

		var asyncCursor = await collection.FindAsync(
			filter: filter,
			options: options,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		while (await asyncCursor
			.MoveNextAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			foreach (var current in asyncCursor.Current)
			{
				yield return new GameWagerStatBacklogInfo(
				PlayerCode: current.PlayerCode,
				StatAt: current.StatAt,
				StartAt: current.StartAt,
				EndAt: current.EndAt,
				TriggerAt: current.TriggerAt,
				Count: current.Count);
			}
		}
	}
	public async ValueTask UpdateExpireOnAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatBacklog>.Filter.Eq(x => x.PlayerCode, playerCode)
			& Builders<GameWagerStatBacklog>.Filter.Eq(x => x.StatAt, statAt.UtcDateTime)
			& Builders<GameWagerStatBacklog>.Filter.Eq(x => x.StartAt, startAt.UtcDateTime)
			& Builders<GameWagerStatBacklog>.Filter.Eq(x => x.EndAt, endAt.UtcDateTime);

		var update = Builders<GameWagerStatBacklog>.Update
			.Set(s => s.PlayerCode, playerCode)
			.Set(s => s.StatAt, statAt.UtcDateTime)
			.Set(s => s.StartAt, startAt.UtcDateTime)
			.Set(s => s.EndAt, endAt.UtcDateTime)
			.Set(s => s.ExpireAt, DateTime.UtcNow.AddSeconds(1))
			.Inc(s => s.Count, 1);

		var options = new UpdateOptions
		{
			IsUpsert = true
		};

		var retryPolicy = Policy
		   .Handle<MongoBulkWriteException>()
		   .WaitAndRetryAsync(
			retryCount: 3,
			retryAttempt => TimeSpan.FromSeconds(5),
			(result, retryTime, context) => logger.LogWarning("LogOn:{logOn}  PlayerCode:{playerCode} TriggerAt:{statAt} Result:{result} Delay:{retryTime}",
				DateTime.Now.ToString("o"),
				playerCode,
				statAt,
				result,
				retryTime));

		_ = await retryPolicy.ExecuteAsync(
			() => collection.UpdateOneAsync(
				filter: filter,
				update: update,
				options: options,
				cancellationToken: cancellationToken))
			.ConfigureAwait(false);
	}

	private static Task PlayerVipLevelBacklog_IndexAsync(
		IMongoCollection<GameWagerStatBacklog> collection,
		CancellationToken cancellationToken = default)
	{
		var index = Builders<GameWagerStatBacklog>.IndexKeys
			.Ascending(m => m.PlayerCode)
			.Ascending(m => m.StatAt)
			.Ascending(m => m.StartAt)
			.Ascending(m => m.EndAt);

		var createIndexOptions = new CreateIndexOptions
		{
			Unique = true,
			Name = "px_game_wager_stat_backlogs"
		};
		var createIndexModel = new CreateIndexModel<GameWagerStatBacklog>(index, createIndexOptions);

		return _ = collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private static Task ExpireOn_IndexAsync(
		IMongoCollection<GameWagerStatBacklog> collection,
		CancellationToken cancellationToken = default)
	{
		var index = Builders<GameWagerStatBacklog>.IndexKeys
			.Ascending(m => m.ExpireAt);

		var createIndexOptions = new CreateIndexOptions
		{
			ExpireAfter = TimeSpan.FromSeconds(1),
			Name = "idx_expire_at"
		};
		var createIndexModel = new CreateIndexModel<GameWagerStatBacklog>(index, createIndexOptions);

		return _ = collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private async ValueTask<IMongoCollection<GameWagerStatBacklog>> GetCollectionAsync(CancellationToken cancellationToken = default)
	{
		var collection = mongoDatabase.GetCollection<GameWagerStatBacklog>(typeof(GameWagerStatBacklog).GetTableName());

		if (mongoDatabase.ListCollectionNames(
			options: new ListCollectionNamesOptions
			{
				Filter = new BsonDocument("name", typeof(GameWagerStatBacklog).GetTableName())
			},
			cancellationToken: cancellationToken).Any(cancellationToken: cancellationToken))
		{
			return collection;
		}

		await PlayerVipLevelBacklog_IndexAsync(
			collection: collection,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await ExpireOn_IndexAsync(
			collection: collection,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return collection;
	}
}
