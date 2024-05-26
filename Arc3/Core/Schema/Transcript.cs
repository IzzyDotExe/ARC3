
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class Transcript {

  [BsonId]
  [BsonRepresentation(BsonType.String)]
  public string Id { get; set; }

  [BsonElement("modmailId")]
  public string ModMailId { get; set; }

  [BsonElement("sendersnowflake")]
  public long SenderSnowfake { get; set; }

  [BsonElement("attachments")]
  public string[] AttachmentURls { get; set; }

  [BsonElement("createdat")]
  public DateTime CreatedAt { get; set; }

  [BsonElement("GuildSnowflake")]
  public long GuildSnowflake { get; set; }

  [BsonElement("messagecontent")]
  public string MessageContent { get; set; }

  [BsonElement("transcripttype")]
  public string TranscriptType { get; set; }

  [BsonElement("comment")]
  public bool Comment {get; set;} = false;

}