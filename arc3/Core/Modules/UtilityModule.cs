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

namespace Arc3.Core.Modules;

public class UtilityModule : ArcModule {
  
  public DbService DbService { get; set; }
  public UptimeService UptimeService { get; set; }

  public UtilityModule(DiscordSocketClient clientInstance) : base(clientInstance, "Utility") {
    
  }

  public override void RegisterListeners() {
    _clientInstance.SelectMenuExecuted += OnInteractionCreated;
  }

  private async Task OnInteractionCreated(SocketMessageComponent interaction) {
    
    var data = interaction.Data;
    
    if (!data.CustomId.Equals("avatar_component"))
      return;

    string response = "w";
    var guild = _clientInstance.GetGuild(interaction.GuildId?? 0);
    var user = guild.GetUser(ulong.Parse(data.Values.First().Split('.')[1]));

    if (data.Values.First().Split('.')[0] == "global") {
      response = user.GetAvatarUrl(ImageFormat.Auto);
    }

    if (data.Values.First().Split('.')[0] == "server") {
      response = user.GetGuildAvatarUrl(ImageFormat.Auto);
    }
    

    if (ulong.Parse(data.Values.First().Split('.')[2]) == interaction.User.Id) {
      await interaction.UpdateAsync(x => {
        if (!String.IsNullOrEmpty(response))
          x.Content = response;
      });
    } else {
      await interaction.RespondAsync("Not Your command", ephemeral: true);
    }

  }
  

  [SlashCommand("uptime", "Get the bot's uptime")]
  public async Task UptimeCommand() {
    
    // TODO: Change strings when localization is added.
    string uptimeMsg = "Uptime";
    string uptimeDays = "Days";
    string uptimeHours = "Hrs";
    string uptimeMinutes = "Mins";
    string uptimeSeconds = "Sec";

    var uptime = UptimeService.Uptime.Elapsed;
    
    var embed = new EmbedBuilder()
      .WithAuthor(new EmbedAuthorBuilder()
        .WithName(_clientInstance.CurrentUser.Username)
        .WithIconUrl(_clientInstance.CurrentUser.GetAvatarUrl(ImageFormat.Auto)))
      .WithColor(Color.DarkBlue)
      .WithDescription($"**{uptimeMsg}:** ``{uptime.Days}{uptimeDays} {uptime.Hours}{uptimeHours} {uptime.Minutes}{uptimeMinutes} {uptime.Seconds}{uptimeSeconds}``")
      .Build();

    await Context.Interaction.RespondAsync(embed: embed);

  }

  [SlashCommand("ping", "Get the bot's latency")]
  public async Task PingCommand() {

    Stopwatch timer = new Stopwatch();
    timer.Start();
    await Context.Channel.TriggerTypingAsync();
    var msg = await Context.Channel.SendMessageAsync(".");
    timer.Stop();
    await msg.DeleteAsync();

    string wsText = "Websocket latency";
    string rtText = "Roundtrip latency";
    var wsPing = Context.Client.Latency;
    var rtPing = timer.ElapsedMilliseconds.ToString();

    var embed = new EmbedBuilder()
      .WithColor(Color.DarkBlue)
      .WithDescription($"🌐 **{wsText}:** ``{wsPing}ms``\n💬 **{rtText}:** ``{rtPing}ms``")
      .Build();
    await Context.Interaction.RespondAsync(embed:embed);

  }

  [SlashCommand("avatar", "Get the avatar of the user")]
  public async Task AvatarCommand(
    SocketUser? user
  ) {

    if (user is null) {
      user = Context.User;
    }

    var selectOptions = new SelectMenuBuilder()
      .WithCustomId("avatar_component")
      .WithOptions(new List<SelectMenuOptionBuilder> {
        new SelectMenuOptionBuilder()
          .WithLabel("🌐  Global Avatar")
          .WithDescription("Get the user's global avatar")
          .WithDefault(true)
          .WithValue($"global.{user.Id}.{Context.User.Id}"),
        new SelectMenuOptionBuilder()
          .WithLabel("🖥️  Server Avatar")
          .WithDescription("Get the user's server avatar")
          .WithDefault(false)
          .WithValue($"server.{user.Id}.{Context.User.Id}")
    }).Build();
    
    await Context.Interaction.RespondAsync(
      text: user.GetAvatarUrl(ImageFormat.Auto, 2048),
      components: new ComponentBuilder().AddRow(new ActionRowBuilder().AddComponent(selectOptions)).Build()
    );

  }

}