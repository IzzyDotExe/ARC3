using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class GuildConfig {

  [BsonId]
  [BsonRepresentation(BsonType.String)]
  public string Id { get; set; }

  [BsonElement("guildsnowflake")]
  public long GuildSnowflake { get; set; }

  [BsonElement("configkey")]
  public string ConfigKey { get; set; }

  [BsonElement("configvalue")]
  public string ConfigValue { get; set; }

}
