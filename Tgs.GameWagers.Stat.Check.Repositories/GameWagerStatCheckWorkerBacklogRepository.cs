using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly;
using Tgs.GameWagers.Stat.Check.Core;
using Tgs.GameWagers.Stat.Check.Core.Extensions;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.GameWagers.Stat.Check.Repositories.Models;

namespace Tgs.GameWagers.Stat.Check.Repositories;
internal class GameWagerStatCheckWorkerBacklogRepository(
	ILogger<GameWagerStatCheckWorkerBacklogRepository> logger,
	IMongoDatabase mongoDatabase) : IGameWagerStatCheckWorkerBacklogRepository
{
	public async ValueTask AddAsync(string playerCode, DateTimeOffset triggerAt, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatCheckWorkerBacklog>.Filter.Eq(x => x.PlayerCode, playerCode);

		var update = Builders<GameWagerStatCheckWorkerBacklog>.Update
			.Set(s => s.PlayerCode, playerCode)
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
			(result, retryTime, context) => logger.LogWarning("LogOn:{logOn}  PlayerCode:{playerCode} TriggerAt:{triggerAt} Result:{result} Delay:{retryTime}",
				DateTime.Now.ToString("o"),
				playerCode,
				triggerAt,
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
	public async IAsyncEnumerable<GameWagerStatCheckWorkerBacklogInfo> QueryAsync(DateTimeOffset endOn, int offset = 0, int limit = 20, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatCheckWorkerBacklog>.Filter.Lt(x => x.TriggerAt, endOn.UtcDateTime);

		var sort = Builders<GameWagerStatCheckWorkerBacklog>
			.Sort
			.Ascending(x => x.Id);

		var options = new FindOptions<GameWagerStatCheckWorkerBacklog>
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
				yield return new GameWagerStatCheckWorkerBacklogInfo(
				PlayerCode: current.PlayerCode,
				TriggerAt: current.TriggerAt,
				Count: current.Count);
			}
		}
	}
	public async ValueTask UpdateExpireAtAsync(string playerCode, DateTimeOffset triggerAt, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatCheckWorkerBacklog>.Filter.Eq(x => x.PlayerCode, playerCode);

		var update = Builders<GameWagerStatCheckWorkerBacklog>.Update
			.Set(s => s.PlayerCode, playerCode)
			.Set(s => s.TriggerAt, triggerAt.UtcDateTime)
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
			(result, retryTime, context) => logger.LogWarning("LogOn:{logOn}  PlayerCode:{playerCode} TriggerAt:{triggerAt} Result:{result} Delay:{retryTime}",
				DateTime.Now.ToString("o"),
				playerCode,
				triggerAt,
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

	private static Task PrimaryKey_IndexAsync(IMongoCollection<GameWagerStatCheckWorkerBacklog> collection, CancellationToken cancellationToken = default)
	{
		var index = Builders<GameWagerStatCheckWorkerBacklog>.IndexKeys
			.Ascending(m => m.PlayerCode)
			.Ascending(m => m.TriggerAt);

		var createIndexOptions = new CreateIndexOptions
		{
			Unique = true,
			Name = "px_index"
		};
		var createIndexModel = new CreateIndexModel<GameWagerStatCheckWorkerBacklog>(index, createIndexOptions);

		return _ = collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private static Task ExpireOn_IndexAsync(IMongoCollection<GameWagerStatCheckWorkerBacklog> collection, CancellationToken cancellationToken = default)
	{
		var index = Builders<GameWagerStatCheckWorkerBacklog>.IndexKeys
			.Ascending(m => m.ExpireAt);

		var createIndexOptions = new CreateIndexOptions
		{
			ExpireAfter = TimeSpan.FromSeconds(1),
			Name = "idx_expire_at"
		};
		var createIndexModel = new CreateIndexModel<GameWagerStatCheckWorkerBacklog>(index, createIndexOptions);

		return _ = collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private async ValueTask<IMongoCollection<GameWagerStatCheckWorkerBacklog>> GetCollectionAsync(CancellationToken cancellationToken = default)
	{
		var collection = mongoDatabase.GetCollection<GameWagerStatCheckWorkerBacklog>(typeof(GameWagerStatCheckWorkerBacklog).GetTableName());

		if (mongoDatabase.ListCollectionNames(
			options: new ListCollectionNamesOptions
			{
				Filter = new BsonDocument("name", typeof(GameWagerStatCheckWorkerBacklog).GetTableName())
			},
			cancellationToken: cancellationToken).Any(cancellationToken: cancellationToken))
		{
			return collection;
		}

		await PrimaryKey_IndexAsync(
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
