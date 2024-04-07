using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class Jail {

  [BsonId]
  [BsonRepresentation(BsonType.String)]
  public string Id { get; set; }

  [BsonElement("usersnowflake")]
  public long UserSnowflake { get; set; }

  [BsonElement("channelsnowflake")]
  public long ChannelSnowflake { get; set; }

}
