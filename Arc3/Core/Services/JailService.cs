

using Arc3.Core.Schema;
using Arc3.Core.Schema.Ext;
using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Services;

public class JailService : ArcService {
  private readonly DbService _dbService;

  public JailService(DiscordSocketClient clientInstance, InteractionService interactionService,
    DbService dbService)
    : base(clientInstance, interactionService, "JAIL") {
      _dbService = dbService;
      clientInstance.MessageReceived += ClientInstanceOnMessageReceived;
    }

  private async Task ClientInstanceOnMessageReceived(SocketMessage arg)
  {
    
    var jails = await _dbService.GetJailsAsync();
    
    if (jails.All(x => x.ChannelSnowflake != (long)arg.Channel.Id))
    {
      return;
    }

    var jail = jails.First(x => x.ChannelSnowflake == (long)arg.Channel.Id);
    var channel = await jail.GetChannel(_clientInstance);

    var transcript = new Transcript()
    {
      Id = arg.Id.ToString(),
      ModMailId = jail.Id,
      SenderSnowfake = (long)arg.Author.Id,
      AttachmentURls = arg.Attachments.Select(x => x.ProxyUrl).ToArray(),
      CreatedAt = arg.CreatedAt.UtcDateTime,
      GuildSnowflake = (long)channel.GuildId,
      MessageContent = arg.Content,
      TranscriptType = "Jail"
    };

    await _dbService.AddTranscriptAsync(transcript);
    
  }
}