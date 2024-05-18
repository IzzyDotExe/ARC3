

using arc3.Core.Schema;
using Arc3.Core.Attributes;
using Arc3.Core.Ext;
using Arc3.Core.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Modules;

[RequireCommandBlacklist, RequirePremium]
public class KaraokeModule : ArcModule {

  public DbService DbService { get; set ; }
  public KaraokeService KaraokeService { get; set; }

  public KaraokeModule(DiscordSocketClient clientInstance) : base(clientInstance, "Karaoke") {

  }

  public override void RegisterListeners()
  {
        
  }

  private async Task<EmbedBuilder> GetQueueEmbed() {

    var guildUser = Context.Guild.GetUser(Context.User.Id);

    // Get the queue
    var queue = await DbService.GetQueueAsync(guildUser.VoiceChannel.Id);

    var description = "";

    queue.ForEach(x => {

      var user = Context.Guild.GetUser(((ulong)x.UserSnowflake));
    
      description += (x.Rank == 1?"**Current turn:**" : x.Rank + ".") + $" {user.Mention}\n";

    });

    description += $"\n**Admin User: ** <@{KaraokeService.ChannelCache[guildUser.VoiceChannel.Id].AdminSnowflake}>";

    var embed = new EmbedBuilder().WithModMailStyle(_clientInstance, "Karaoke");
    embed.WithTitle($"Queue for {guildUser.VoiceChannel.Name}");
    embed.WithDescription(description);

    return embed;
  }


  #region commands

  [SlashCommand("queuejoin", "Join the queue for the current channel."),
  KaraokePrecondition(false)]
  public async Task QueueJoinCommand() {

    var guildUser = Context.Guild.GetUser(Context.User.Id);
    
    // Join the queue
    var rank = await DbService.AddToQueue(new KaraokeUser {
      Id = Guid.NewGuid().ToString(),
      UserSnowflake = ((long)Context.User.Id),
      ChannelSnowflake = ((long)guildUser.VoiceChannel.Id)
    });

    await Context.Interaction.RespondAsync($"You are in position #{rank}.");


  }

  [SlashCommand("queue", "View the queue!"),
  KaraokePrecondition(false)]
  public async Task QueueCommand() {
    
    var embed = await GetQueueEmbed();

    await Context.Interaction.RespondAsync(embed: embed.Build());

  }

  [SlashCommand("queuenext", "Skip to the next user in the queue."),
  KaraokePrecondition]
  public async Task QueueNextCommand() {

    var guildUser = Context.Guild.GetUser(Context.User.Id);
    await DbService.PopQueue(guildUser.VoiceChannel.Id);
    var queue = await DbService.GetQueueAsync(guildUser.VoiceChannel.Id);

    try {
      var first = queue.First();
      await Context.Interaction.RespondAsync($"Skipped, Next up is <@{first.UserSnowflake}>!");
    } catch (InvalidOperationException ex) {
      await Context.Interaction.RespondAsync($"The queue is now empty!");
    }

  }

  [SlashCommand("queuelock", "Lock the queue so only admins and mods can skip"),
   KaraokePrecondition]
  public async Task QueueLockCommand()
  {
    
    KaraokeService.ChannelCache[Context.Channel.Id].Locked = !KaraokeService.ChannelCache[Context.Channel.Id].Locked;

    if (KaraokeService.ChannelCache[Context.Channel.Id].Locked)
    {
      await Context.Interaction.RespondAsync("Channel is now locked!");
      return;
    }
    
    await Context.Interaction.RespondAsync("Channel is now unlocked!");
    
  }

  #endregion

}