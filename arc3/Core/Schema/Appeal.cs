using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class Appeal {

  [BsonId]
  [BsonRepresentation(BsonType.String)]
  public string Id { get; set; }

  [BsonElement("usersnowflake")]
  public long UserSnowflake { get; set; }

  [BsonElement("nextappeal")]
  public long NextAppeal { get; set; }

}
