using System.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class Insight {

  [BsonId]
  [BsonRepresentation(BsonType.String)]
  public string Id { get; set; }

  [BsonElement("type")]
  public string Type { get; set; }

  [BsonElement("date")]
  public long Date { get; set; }

  [BsonElement("tagline")]
  public string Tagline { get; set; }

  [BsonElement("guild_id")]
  public string GuildID { get; set; }

  [BsonElement("data")]
  public BsonDocument  Data { get; set; }

  [BsonElement("url")]
  public string Url { get; set; }

}
