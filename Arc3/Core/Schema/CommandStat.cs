using System.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Arc3.Core.Schema;

public class CommandStat {

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; }

    [BsonElement("guild_id")]
    public string GuildID { get; set; }

    [BsonElement("args")]
    public BsonDocument Args { get; set; }

    [BsonElement("command_name")]
    public string Name { get; set; }

}