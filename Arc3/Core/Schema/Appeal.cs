using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class Appeal {

  [BsonId]
  [BsonRepresentation(BsonType.String)]
  public string Id { get; set; } = String.Empty;

  [BsonElement("userSnowflake")]
  public long UserSnowflake { get; set; }

  [BsonElement("nextappeal")]
  public long NextAppeal { get; set; }
  
  [BsonElement("bannedBy")]
  public string BannedBy { get; set; }
  
  [BsonElement("appealContent")]
  public string AppealContent { get; set; }

  [BsonElement("action")]
  public string Action { get; set; }

}
