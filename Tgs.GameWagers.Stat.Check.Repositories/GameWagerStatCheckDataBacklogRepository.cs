using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly;
using Tgs.GameWagers.Stat.Check.Core;
using Tgs.GameWagers.Stat.Check.Core.Extensions;
using Tgs.GameWagers.Stat.Check.Core.Models;
using Tgs.GameWagers.Stat.Check.Repositories.Models;

namespace Tgs.GameWagers.Stat.Check.Repositories;

internal class GameWagerStatCheckDataBacklogRepository(
	ILogger<GameWagerStatCheckDataBacklogRepository> logger,
	IMongoDatabase mongoDatabase) : IGameWagerStatCheckDataBacklogRepository
{
	public async ValueTask AddAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, DateTimeOffset triggerAt, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.PlayerCode, playerCode)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.StatAt, statAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.StartAt, startAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.EndAt, endAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.TriggerAt, triggerAt.UtcDateTime);

		var update = Builders<GameWagerStatCheckDataBacklog>.Update
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

	public async ValueTask DeleteAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, DateTimeOffset triggerAt, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.PlayerCode, playerCode)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.StatAt, statAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.StartAt, startAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.EndAt, endAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.TriggerAt, triggerAt.UtcDateTime);

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
			() => collection.DeleteOneAsync(
				filter: filter,
				cancellationToken: cancellationToken))
			.ConfigureAwait(false);
	}

	public async IAsyncEnumerable<GameWagerStatCheckDataBacklogInfo> QueryAsync(DateTimeOffset endOn, int offset = 0, int limit = 20, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatCheckDataBacklog>.Filter.Lt(x => x.TriggerAt, endOn.UtcDateTime);

		var sort = Builders<GameWagerStatCheckDataBacklog>
			.Sort
			.Ascending(x => x.Id);

		var options = new FindOptions<GameWagerStatCheckDataBacklog>
		{
			Sort = sort,
			Skip = offset * limit,
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
				yield return new GameWagerStatCheckDataBacklogInfo(
				PlayerCode: current.PlayerCode,
				StatAt: current.StatAt,
				StartAt: current.StartAt,
				EndAt: current.EndAt,
				TriggerAt: current.TriggerAt,
				Count: current.Count);
			}
		}
	}

	public async ValueTask UpdateExpireAtAsync(string playerCode, DateTimeOffset statAt, DateTimeOffset startAt, DateTimeOffset endAt, DateTimeOffset triggerAt, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync(cancellationToken).ConfigureAwait(false);

		var filter = Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.PlayerCode, playerCode)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.StatAt, statAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.StartAt, startAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.EndAt, endAt.UtcDateTime)
			& Builders<GameWagerStatCheckDataBacklog>.Filter.Eq(x => x.TriggerAt, triggerAt.UtcDateTime);

		var update = Builders<GameWagerStatCheckDataBacklog>.Update
			.Set(s => s.PlayerCode, playerCode)
			.Set(s => s.StatAt, statAt.UtcDateTime)
			.Set(s => s.StartAt, startAt.UtcDateTime)
			.Set(s => s.EndAt, endAt.UtcDateTime)
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

	private static Task PrimaryKey_IndexAsync(IMongoCollection<GameWagerStatCheckDataBacklog> collection, CancellationToken cancellationToken = default)
	{
		var index = Builders<GameWagerStatCheckDataBacklog>.IndexKeys
			.Ascending(m => m.PlayerCode)
			.Ascending(m => m.StatAt)
			.Ascending(m => m.StartAt)
			.Ascending(m => m.EndAt)
			.Ascending(m => m.TriggerAt);

		var createIndexOptions = new CreateIndexOptions
		{
			Unique = true,
			Name = "px_index"
		};
		var createIndexModel = new CreateIndexModel<GameWagerStatCheckDataBacklog>(index, createIndexOptions);

		return _ = collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private static Task ExpireOn_IndexAsync(IMongoCollection<GameWagerStatCheckDataBacklog> collection, CancellationToken cancellationToken = default)
	{
		var index = Builders<GameWagerStatCheckDataBacklog>.IndexKeys
			.Ascending(m => m.ExpireAt);

		var createIndexOptions = new CreateIndexOptions
		{
			ExpireAfter = TimeSpan.FromSeconds(1),
			Name = "idx_expire_at"
		};
		var createIndexModel = new CreateIndexModel<GameWagerStatCheckDataBacklog>(index, createIndexOptions);

		return _ = collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private async ValueTask<IMongoCollection<GameWagerStatCheckDataBacklog>> GetCollectionAsync(CancellationToken cancellationToken = default)
	{
		var collection = mongoDatabase.GetCollection<GameWagerStatCheckDataBacklog>(typeof(GameWagerStatCheckDataBacklog).GetTableName());

		if (mongoDatabase.ListCollectionNames(
			options: new ListCollectionNamesOptions
			{
				Filter = new BsonDocument("name", typeof(GameWagerStatCheckDataBacklog).GetTableName())
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
