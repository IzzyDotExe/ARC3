using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using Arc3.Core.Services;
using Discord;
using Discord.API;
using Discord.Interactions;
using Discord.WebSocket;
using ZstdSharp.Unsafe;
using System.Runtime.InteropServices;
using MongoDB.Bson;
using Arc3.Core.Schema;
using System.Data.Common;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Arc3.Core.Ext;
using Arc3.Core.Attributes;

namespace Arc3.Core.Modules;


[RequireOwner]
public class OwnerModule : ArcModule {
  
  public DbService DbService { get; set; }
  public UptimeService UptimeService { get; set; }

  public OwnerModule(DiscordSocketClient clientInstance) : base(clientInstance, "Owner") {

  }

  public override void RegisterListeners() {

  }
  
  [SlashCommand("status", "Change the bot status"),
   RequireUserPermission(GuildPermission.Administrator)]
  public async Task StatusCommand(string name, string url = null, ActivityType type = ActivityType.CustomStatus)
  {
    await _clientInstance.SetGameAsync(name, url, type);
    await Context.Interaction.RespondAsync("Changed!", ephemeral:true);
  }

  [SlashCommand("premium", "Toggle premium mode for this server.")]
  public async Task PremiumCommand()
  {

      var guild = await DbService.GetItemsAsync<GuildInfo>("Guilds");
      var self = guild.First(x => x.GuildSnowflake == Context.Guild.Id.ToString());
      await DbService.UpdatePremium(self.GuildSnowflake, !self.Premium);
      if (self.Premium)
      {
          await Context.Interaction.RespondAsync("Premium is now disabled");
      }
      else
      {
          await Context.Interaction.RespondAsync("Premium is now enabled");
      }
  }
  
}