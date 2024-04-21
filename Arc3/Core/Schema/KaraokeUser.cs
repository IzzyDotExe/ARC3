using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace arc3.Core.Schema;

public class KaraokeUser {

  [BsonId]
  [BsonRepresentation(BsonType.String)] 
  public string Id { get; set; }

  [BsonElement("channelsnowflake")]
  public long ChannelSnowflake;

  [BsonElement("usersnowflake")]
  public long UserSnowflake;

  [BsonElement("rank")]
  public int Rank;

}