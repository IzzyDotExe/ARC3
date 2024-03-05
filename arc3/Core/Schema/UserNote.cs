using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class UserNote {

  [BsonId]
  [BsonRepresentation(BsonType.String)]
  public string Id { get; set; } = string.Empty;

  [BsonElement("usersnowflake")]
  public long UserSnowflake;

  [BsonElement("guildsnowflake")]
  public long GuildSnowflake;

  [BsonElement("note")]
  public string Note { get; set; } = string.Empty;

  [BsonElement("date")]
  public long Date { get; set; }

  [BsonElement("authorsnowflake")]
  public long AuthorSnowflake { get; set; }

}