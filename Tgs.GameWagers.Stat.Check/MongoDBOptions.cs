namespace Tgs.GameWagers.Stat.Check;

public record MongoDBOptions
{
	public string ConnectionString { get; set; } = "mongodb://localhost";
}
