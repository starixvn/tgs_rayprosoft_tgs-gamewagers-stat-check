using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Tgs.GameWagers.Stat.Check.Repositories.Models;
[Table("game_wager_stat_check_worker_backlogs")]
public class GameWagerStatCheckWorkerBacklog
{
	/// <summary>
	/// The identifier
	/// </summary>
	[BsonId]
	public ObjectId Id { get; set; } = default!;

	/// <summary>
	/// The player code
	/// </summary>
	[BsonElement("player_code")]
	[BsonRepresentation(BsonType.String)]
	public string PlayerCode { get; set; } = default!;
	/// <summary>
	/// The count
	/// </summary>
	[BsonElement("count")]
	[BsonRepresentation(BsonType.Int32)]
	[BsonDefaultValue(0)]
	public int Count { get; set; }
	/// <summary>
	/// The updated on
	/// </summary>
	[BsonElement("trigger_at")]
	public DateTime TriggerAt { get; set; }
	/// <summary>
	/// The expire on
	/// </summary>
	[BsonElement("expire_at")]
	public DateTime ExpireAt { get; set; }
}
