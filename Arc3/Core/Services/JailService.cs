

using Arc3.Core.Schema;
using Arc3.Core.Schema.Ext;
using Discord;
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
      clientInstance.UserJoined += ClientInstanceOnUserJoined;
    }

  private async Task ClientInstanceOnUserJoined(SocketGuildUser arg)
  {
    
    // Get all the jails
    var jails = await _dbService.GetJailsAsync();

    // If the user that joined has a jail logged. 
    if (jails.Any(x => x.UserSnowflake == (long)arg.Id))
    {
      
      var jail = jails.First(x => x.UserSnowflake == (long)arg.Id);
      
      // give them the jailed role and make sure the channel has allowed them.
      
      // Get some needed vars 
      var channel = await jail.GetChannel(_clientInstance);
      var user = arg;
      var guildConfig = _dbService.Config[user.Guild.Id];
      var guild = arg.Guild;
      
      // Set the channel perms
      var perms = new OverwritePermissions(viewChannel: PermValue.Allow);
      await channel.AddPermissionOverwriteAsync(user, perms);
      
      // Give the role
      try {
        var jailedRoleSnowflake = ulong.Parse(guildConfig["jailedrole"]);
        var jailRole = guild.GetRole(jailedRoleSnowflake);
        await user.AddRoleAsync(jailRole);
        // await channel.SendMessageAsync("User rejoined! Adding jail role.");
      } catch (Exception e) {
        await channel.SendMessageAsync("Jail role not found or failed to give role to user!");
      }

    }
    
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
  
    var transcript = new Transcript
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