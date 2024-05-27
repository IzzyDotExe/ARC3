using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class Blacklist
{

  [BsonId]
  [BsonRepresentation(BsonType.String)]
  public ObjectId Id { get; set; }

  [BsonElement("usersnowflake")]
  public long UserSnowflake { get; set; }

  [BsonElement("cmd")]
  public string Command { get; set; }

  [BsonElement("guildsnowflake")]
  public long GuildSnowflake { get; set; }

}