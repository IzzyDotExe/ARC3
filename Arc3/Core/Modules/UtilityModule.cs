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
using System.Runtime.CompilerServices;

namespace Arc3.Core.Modules;


[RequireCommandBlacklist]
public class UtilityModule : ArcModule {      
  
  public DbService DbService { get; set; }
  public UptimeService UptimeService { get; set; }
  
  public ModMailService ModmailService { get; set; }
  

  private static DateTimeOffset lastAlert = new DateTimeOffset(2024, 02, 1, 0, 0, 0, new());

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

  [SlashCommand("modmail", "Select a guild to modmail"), EnabledInDm(true)]
  public async Task ModmailCommand()
  {
    
    // Exit if it is not a dm channel
    if (Context.Channel.GetChannelType() != ChannelType.DM)
    {
      await Context.Interaction.RespondAsync("This command is only available in the bot DMS.", ephemeral: true);
      return;
    }
    
    var mails = await DbService.GetModMails();

    if (mails.Any(x => (ulong)x.UserSnowflake == Context.User.Id))
    {
      await Context.Interaction.RespondAsync("You are already in a modmail.", ephemeral: true);
      return;
    }

    
    var selectMenu = ModmailService.BuildModmailSelectMenu();
    var components = new ComponentBuilder().WithSelectMenu(new SelectMenuBuilder().WithOptions(selectMenu));
    await Context.Interaction.RespondAsync(components: components.Build());

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
    SocketUser? user = null
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

  [SlashCommand("setconfig", "Set a config string for the guild"),
   RequireUserPermission(GuildPermission.ManageGuild),
   RequirePremium]
  public async Task SetConfigCommand(
    string configKey,
    string configValue
  ) {

    var configs = await DbService.GetGuildConfigsAsync();
    var config = configs.Where(c => c.GuildSnowflake == (long)Context.Guild.Id && c.ConfigKey.Equals(configKey));
   
    GuildConfig conf;
   
    
    if (config.Any()) {

      conf = config.First();
      BsonDocument data = new BsonDocument();

      data.Add(new BsonElement[] {
        new BsonElement("key", conf.ConfigKey),
        new BsonElement("oldvalue", conf.ConfigValue),
        new BsonElement("newvalue", configValue)
      });

      var insight = new Insight {
        Id = Guid.NewGuid().ToString(),
        Type = "config",
        Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        Tagline = "Config value was changed",
        GuildID = Context.Guild.Id.ToString(),
        Data = data,
        Url = ""
      }; 

      await DbService.AddAsync<Insight>(insight, "Insights");

      conf.ConfigValue = configValue;

    } else {

      conf = new GuildConfig() {
        Id = (configs.Count + 1).ToString(),
        ConfigKey = configKey,
        ConfigValue = configValue,
        GuildSnowflake = ((long)Context.Guild.Id)
      };

    }

    await DbService.SetGuildConfigAsync(conf);

    var embed = new EmbedBuilder()
      .WithTitle("Config was updated sucessfully")
      .WithDescription($"```{configKey} ---> {configValue}```")
      .Build();

    await Context.Interaction.RespondAsync(embed: embed, ephemeral: true);

  }

  [SlashCommand("getconfig", "Get a config string for the guilds"),
   RequireUserPermission(GuildPermission.ManageGuild),
   RequirePremium]
  public async Task GetConfigCommand(
    string configKey
  ) {

    var configValueExists = DbService.Config[Context.Guild.Id].ContainsKey(configKey);

    string? configValue = null;
    string description;

    if (configValueExists)
      configValue = DbService.Config[Context.Guild.Id][configKey];

    description = $"``{configKey}`` is currently set to ``{configValue}``.";

    if (configValue == null || string.IsNullOrEmpty(configValue) || configValue.ToLower().Equals("null"))
      description = $"``{configKey}`` is not currently set to anything.";

    var embed = new EmbedBuilder()
      .WithTitle($"Config for {Context.Guild.Name}")
      .WithDescription(description)
      .Build();

    await Context.Interaction.RespondAsync(embed: embed, ephemeral: true);

  }

  [SlashCommand("alert", "Send an alert ping to the moderators"), RequirePremium]
  public async Task AlertCommand() {

    var configValueExists = DbService.Config[Context.Guild.Id].ContainsKey("alertrole") || DbService.Config[Context.Guild.Id].ContainsKey("alertcooldown");
    
    if (!configValueExists) {
      await Context.Interaction.RespondAsync("This command is not configured! Please ask the admins to set the alertrole and alertcooldown config values.", ephemeral: true);
      return;
    } 

    string alertRole = DbService.Config[Context.Guild.Id]["alertrole"];
    string alertcooldown = DbService.Config[Context.Guild.Id]["alertcooldown"];

    var alert = lastAlert.AddHours(double.Parse(alertcooldown));

    if (DateTimeOffset.UtcNow > alert) {
      
      await Context.Interaction.RespondAsync($"<@&{alertRole}>", allowedMentions: AllowedMentions.All);

      lastAlert = DateTimeOffset.UtcNow;
      return;
    }

    await Context.Interaction.RespondAsync("Alert command is on cooldown!");
    return;

  }

  [SlashCommand("embedsimple", "Create an embed message"), 
   RequireUserPermission(GuildPermission.Administrator), RequirePremium]
  public async Task EmbedCommand(string title, string description, string color, string? thumbnail = null, string? image = null, SocketTextChannel? channel = null)
  {
  
    // TODO: Add saving.
    //    if (id is not null)
    //    {
    //      
    //    }

    channel ??= (SocketTextChannel)Context.Channel;

    var embed = new EmbedBuilder()
      .WithTitle(title == "none" ? "" : title)
      .WithDescription(description == "none" ? "" : description)
      .WithColor(new Color(Convert.ToUInt32(color, 16)));

    if (image != null)
      embed.WithImageUrl(image);

    if (thumbnail != null)
      embed.WithThumbnailUrl(thumbnail);
    
    await channel.SendMessageAsync(embed:embed.Build());
    await Context.Interaction.RespondAsync("Sent!");
    
    //    if (id is null)
    //   ;
    //    else
    //      await Context.Interaction.RespondAsync("Sent and saved!");
    //   
    
  }

  [SlashCommand("blacklistglobal", "Globally blacklist a user"),
  RequireOwner, RequirePremium]
  public async Task BlacklistIDCommand(
    string id,
    string? cmd = "all"
  ) {
    var ctx = Context.Interaction;
    var blacklists = await DbService.GetItemsAsync<Blacklist>("blacklist");
    var user = await _clientInstance.GetUserAsync(ulong.Parse(id));

    // Guard if the user is already blacklisted.
    if (blacklists.Any(x => x.GuildSnowflake == ((long)Context.Guild.Id) &&  x.UserSnowflake == long.Parse(id) && (x.Command == "all" || x.Command == cmd)) ) {
      await ctx.RespondAsync($"That user is already blacklisted from {cmd}", ephemeral: true);
      return;
    }

    // Guard if the user is the invoker
    if (Context.User.Id == ulong.Parse(id)) {
      await ctx.RespondAsync("You cannot blacklist yourself, silly.", ephemeral: true);
      return;
    }

    var blacklist = new Blacklist() {
      UserSnowflake = long.Parse(id),
      GuildSnowflake = 0,
      Command = cmd
    };

    // Add the blacklist
    await DbService.AddAsync<Blacklist>(blacklist, "blacklist");

    await ctx.RespondAsync($"{user.Mention} was blacklisted from {cmd}", ephemeral: true);

  }

  [SlashCommand("blacklist", "Add a user to the command blacklist"),
  RequireUserPermission(GuildPermission.Administrator), RequirePremium]
  public async Task BlacklistCommand(
    SocketUser user,
    string? cmd = "all"
  ) {

    var ctx = Context.Interaction;
    var blacklists = await DbService.GetItemsAsync<Blacklist>("blacklist");

    // Guard if the user is already blacklisted.
    if (blacklists.Any(x => x.GuildSnowflake == ((long)Context.Guild.Id) &&  x.UserSnowflake == (long)user.Id && (x.Command == "all" || x.Command == cmd)) ) {
      await ctx.RespondAsync($"That user is already blacklisted from {cmd}", ephemeral: true);
      return;
    }

    // Guard if the user has a higher role
    if (Context.CheckRoleHigher(user)) {
      await ctx.RespondAsync($"You do not have permission to blacklist that user!", ephemeral: true);
      return;
    }

    // Guard if the user is the invoker
    if (Context.User.Id == user.Id) {
      await ctx.RespondAsync("You cannot blacklist yourself, silly.", ephemeral: true);
      return;
    }

    var blacklist = new Blacklist() {
      UserSnowflake = ((long)user.Id),
      GuildSnowflake = ((long)Context.Guild.Id),
      Command = cmd
    };

    // Add the blacklist
    await DbService.AddAsync<Blacklist>(blacklist, "blacklist");

    await ctx.RespondAsync($"{user.Mention} was blacklisted from {cmd}", ephemeral: true);

  }

  [SlashCommand("blacklistbyid", "Add a user to the command blacklist by ID"),
  RequireUserPermission(GuildPermission.Administrator), RequirePremium]
  public async Task BlacklistCommand(
    string id,
    string? cmd = "all"
  ) {

    var ctx = Context.Interaction;
    var blacklists = await DbService.GetItemsAsync<Blacklist>("blacklist");
    var user = await _clientInstance.GetUserAsync(ulong.Parse(id));

    // Guard if the user is already blacklisted.
    if (blacklists.Any(x => x.GuildSnowflake == ((long)Context.Guild.Id) &&  x.UserSnowflake == (long)user.Id && (x.Command == "all" || x.Command == cmd)) ) {
      await ctx.RespondAsync($"That user is already blacklisted from {cmd}", ephemeral: true);
      return;
    }

    // Guard if the user is the invoker
    if (Context.User.Id == user.Id) {
      await ctx.RespondAsync("You cannot blacklist yourself, silly.", ephemeral: true);
      return;
    }

    var blacklist = new Blacklist() {
      UserSnowflake = ((long)user.Id),
      GuildSnowflake = ((long)Context.Guild.Id),
      Command = cmd
    };

    // Add the blacklist
    await DbService.AddAsync<Blacklist>(blacklist, "blacklist");

    await ctx.RespondAsync($"{user.Mention} was blacklisted from {cmd}", ephemeral: true);

  }

  [UserCommand("Clear Blacklist"),
  SlashCommand("unblacklist", "Clear a user's blacklist"),
  RequireUserPermission(GuildPermission.Administrator), RequirePremium]
  public async Task UnblacklistCommand(SocketUser user) {

    var ctx = Context.Interaction;
    var blacklists = await DbService.GetItemsAsync<Blacklist>("blacklist");


    // Guard if the user is not already blacklisted.
    if ( !blacklists.Any(x => x.GuildSnowflake == ((long)Context.Guild.Id) &&  x.UserSnowflake == (long)user.Id ) ) {
      await ctx.RespondAsync($"That user is not blacklisted from any command", ephemeral: true);
      return;
    }

    // Guard if the user has a higher role
    if (Context.CheckRoleHigher(user)) {
      await ctx.RespondAsync($"You do not have permission to unblacklist that user!", ephemeral: true);
      return;
    }

    // Guard if the user is the invoker
    if (Context.User.Id == user.Id) {
      await ctx.RespondAsync("You cannot unblacklist yourself, silly.", ephemeral: true);
      return;
    }

    await DbService.ClearBlacklistAsync(((long)user.Id));
    await ctx.RespondAsync($"{user.Mention}'s blacklist was cleared", ephemeral: true);

  }

  [SlashCommand("whitelist", "add a user to the mod whitelist"), RequireUserPermission(GuildPermission.Administrator)]
  public async Task WhiteListCommand(SocketUser user) {
    var guild = await DbService.GetItemsAsync<GuildInfo>("Guilds");
    var self = guild.First(x => x.GuildSnowflake == Context.Guild.Id.ToString());
    var mods = self.Moderators;

    if (mods.Contains(user.Id.ToString())) {
      mods.Remove(user.Id.ToString());
      await Context.Interaction.RespondAsync("User was unwhitelisted");
    } else {
      mods.Add(user.Id.ToString());
      await Context.Interaction.RespondAsync("User was whitelisted");
    }

    await DbService.UpdateModList(Context.Guild.Id.ToString(), mods);

  }
  
}
