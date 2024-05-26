
using Arc3.Core.Attributes;
using Arc3.Core.Ext;
using Arc3.Core.Schema;
using Arc3.Core.Schema.Ext;
using Arc3.Core.Services;
using Discord;

using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using MongoDB.Bson;

namespace Arc3.Core.Modules;

[RequireCommandBlacklist]
public class ModerationModule : ArcModule
{

  public DbService DbService { get; set; }
  public InteractionService  InteractionService{ get; set; }
  public PaginationService PaginationService { get; set; }
  public ModerationModule(DiscordSocketClient clientInstance) : base(clientInstance, "Moderation") {

  }

  public override void RegisterListeners()
  {
    // Register listeners
    _clientInstance.ButtonExecuted += ButtonInteractionCreated;
    _clientInstance.ModalSubmitted += ClientInstanceOnModalSubmitted;
  }

  private async Task ClientInstanceOnModalSubmitted(SocketModal arg)
  {
    
    // await arg.DeferAsync();
    
    if (arg.Data.CustomId.StartsWith("addnote"))
    {
      ulong userSnowflake = ulong.Parse(arg.Data.CustomId.Split('.')[1]);
      var author = arg.User;
      // var user = await _clientInstance.GetUserAsync(userSnowflake);

      String content = arg.Data.Components.First(x => x.CustomId == "usernote.content").Value;

      await DbService.AddUserNote(new UserNote
      {
        AuthorSnowflake = (long)author.Id,
        Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        GuildSnowflake = (long)arg.GuildId!,
        Note = content,
        UserSnowflake = (long)userSnowflake
      });

      await arg.RespondAsync("User note was added!", ephemeral: true);

    }
  }

  #region User Notes
  
  private async Task ButtonInteractionCreated(SocketMessageComponent ctx) {
    if (ctx.Data.CustomId.StartsWith("viewnotes."))
      await ViewNotes(ctx);
    
    if (ctx.Data.CustomId.StartsWith("addnote."))
      await AddUserNote(ctx);
  }

  private async Task AddUserNote(SocketMessageComponent ctx) {

    ulong userId = ulong.Parse(ctx.Data.CustomId.Split('.')[1]);
    var user = await _clientInstance.GetUserAsync(userId);

    var modal = new ModalBuilder()
      .WithTitle($"Add note to {user.Username}")
      .WithCustomId($"addnote.{userId}")
      .AddComponents(new List<IMessageComponent>() {
        new TextInputBuilder()
          .WithLabel("Note")
          .WithCustomId("usernote.content")
          .WithPlaceholder("Enter a note")
          .WithRequired(true)
          .WithStyle(TextInputStyle.Paragraph).Build()
      }, 0)
    .Build();

    await ctx.RespondWithModalAsync(modal);

  }
  
  private async Task ViewNotes(SocketMessageComponent ctx) {
    
    await ctx.DeferAsync(true);

    ulong userSnowflake = ulong.Parse(ctx.Data.CustomId.Split('.')[1]);

    List<UserNote> notes = await DbService.GetUserNotes(userSnowflake, ctx.GuildId??0);
    
    List<Page> pages =  new List<Page>();
    foreach (var note in notes) {
      var embed = note.CreateEmbed(_clientInstance);
      var page = new Page(embed:embed);
      var comp = new ActionRowBuilder()
        .WithButton(new ButtonBuilder()
          .WithStyle(ButtonStyle.Danger)
          .WithCustomId($"usernote.delete.{note.Id}")
          .WithLabel("Delete")
          .WithEmote(new Emoji("🗑️")));
      page.Components = new List<ActionRowBuilder>() { comp };
      pages.Add(page);
    }

    await PaginationService.CreatePaginationResponse(pages, ctx);

  }

  [UserCommand("User Notes"),
   RequireUserPermission(GuildPermission.ManageMessages), RequirePremium]
  public async Task UserNotes(SocketUser user) {


    var ctx = (SocketUserCommand)Context.Interaction;


    List<UserNote> notes = await DbService.GetUserNotes(ctx.Data.Member.Id, ctx.GuildId??0);

    var embed = new EmbedBuilder()
      .WithAuthor(new EmbedAuthorBuilder()
        .WithName($"{ctx.Data.Member} Notes")
        .WithIconUrl(ctx.Data.Member.GetAvatarUrl(ImageFormat.Auto)))
      .WithColor(Color.DarkBlue).Build();
    
    var components = new ComponentBuilder()
      .WithRows(new List<ActionRowBuilder>() {
        new ActionRowBuilder()
          .WithComponents(new List<IMessageComponent>() {
            new ButtonBuilder()
              .WithStyle(ButtonStyle.Primary)
              .WithCustomId($"addnote.{ctx.Data.Member.Id}")
              .WithLabel("Add Note")
              .WithEmote(new Emoji("📝")).Build(),
            new ButtonBuilder()
              .WithStyle(ButtonStyle.Primary)
              .WithCustomId($"viewnotes.{ctx.Data.Member.Id}")
              .WithLabel($"View {notes.Count} Note(s)")
              .WithEmote(new Emoji("📜")).Build()
          })
      })
      .Build();

    await ctx.RespondAsync(embed: embed, components: components, ephemeral: true);

  }

  #endregion
  
  # region Jail
  
  [UserCommand("Jail User"), 
   SlashCommand("jail", "Send a user to jail"),
   RequireUserPermission(GuildPermission.MuteMembers),
   RequirePremium]
  public async Task JailUser(SocketUser user) {
  
    // Fetch the interaction and defer it.
    var ctx = Context.Interaction; 
    await ctx.DeferAsync(true);
    
    // Get all the jails.
    var jails = await DbService.GetJailsAsync();

    /* Guard so that the user is not already jailed */
    if (jails.Any(x => x.UserSnowflake == (long)user.Id)) {
      await ctx.FollowupAsync("Could not jail that user! They might be already jailed.", ephemeral: true);
      return;
    }
    /* Guard */
    
    // Create your jail and initialize it.
    var jail = new Jail();
    await jail.InitAsync(_clientInstance, Context.Guild, user, DbService);
    
    // Send feedback.
    await ctx.FollowupAsync($"{user.Mention} was jailed!", ephemeral: true);

  }

    [UserCommand("Jail Unute"),
  SlashCommand("jailunmute", "Unute a user inside of their jail channel"),
  RequireUserPermission(GuildPermission.MuteMembers),
  RequirePremium]
  public async Task JailMute(SocketUser user) {

    // Fetch the interaction and defer it.
    var ctx = Context.Interaction; 
    await ctx.DeferAsync(true);
    
    // Get all the jails.
    var jails = await DbService.GetJailsAsync();
    
    /* Guard so that the user is actually jailed */
    if (jails.All(x => x.UserSnowflake != (long)user.Id))
    {
      // If they are not the send the error
      await ctx.FollowupAsync("Could not jail mute that user! They might not be jailed.", ephemeral: true);
      return;
    }
       
    // get the jail
    var jail = jails.First(x => x.UserSnowflake == (long)user.Id);

    // Get the jail channel
    var channel = await jail.GetChannel(_clientInstance);

    // remove the user's permission to speak
    var perm =  new OverwritePermissions(sendMessages: PermValue.Allow);
    await channel.AddPermissionOverwriteAsync(user, perm);

  }

  [UserCommand("Jail Mute"),
  SlashCommand("jailmute", "Mute a user inside of their jail channel"),
  RequireUserPermission(GuildPermission.MuteMembers),
  RequirePremium]
  public async Task JailMute(SocketUser user) {

    // Fetch the interaction and defer it.
    var ctx = Context.Interaction; 
    await ctx.DeferAsync(true);
    
    // Get all the jails.
    var jails = await DbService.GetJailsAsync();
    
    /* Guard so that the user is actually jailed */
    if (jails.All(x => x.UserSnowflake != (long)user.Id))
    {
      // If they are not the send the error
      await ctx.FollowupAsync("Could not jail mute that user! They might not be jailed.", ephemeral: true);
      return;
    }
       
    // get the jail
    var jail = jails.First(x => x.UserSnowflake == (long)user.Id);

    // Get the jail channel
    var channel = await jail.GetChannel(_clientInstance);

    // remove the user's permission to speak
    var perm =  new OverwritePermissions(sendMessages: PermValue.Deny);
    await channel.AddPermissionOverwriteAsync(user, perm);

  }

  
  [UserCommand("Unjail User"), 
   SlashCommand("unjail", "Take a user out of jail"),
   RequireUserPermission(GuildPermission.MuteMembers),
   RequirePremium]
  public async Task UnjailUser(SocketUser user)
  {
    
    // Fetch the interaction and defer it.
    var ctx = Context.Interaction; 
    await ctx.DeferAsync(true);
    
    // Get all the jails.
    var jails = await DbService.GetJailsAsync();
    
    /* Guard so that the user is actually jailed */
    if (jails.All(x => x.UserSnowflake != (long)user.Id))
    {
      // If they are not the send the error
      await ctx.FollowupAsync("Could not unjail that user! They might not be jailed.", ephemeral: true);
      return;
    }
    
    // Delete the jail
    var jail = jails.First(x => x.UserSnowflake == (long)user.Id);
    await jail.DestroyAsync(_clientInstance, DbService);
    
    try
    {
      await ctx.FollowupAsync($"{user.Mention} was unjailed!", ephemeral: true);
    } catch (HttpException e)
    {
      await ctx.User.SendMessageAsync($"{user.Mention} was unjailed!");
    }
   
  }

  
  #endregion

  [SlashCommand("diagnose", "Diagnose a modmail channel"),
  RequireUserPermission(GuildPermission.ManageMessages),
  RequirePremium]
  public async Task DiagnoseModmail()
  {
    var mails = await DbService.GetModMails();

    if (!mails.Any(x => (ulong)x.ChannelSnowflake == Context.Channel.Id))
    {
      await Context.Interaction.RespondAsync("This channel is not a modmail channel");
      return;
    }

    var mail = mails.First(x => (ulong)x.ChannelSnowflake == Context.Channel.Id);

    var embed = new EmbedBuilder().WithModMailStyle(_clientInstance);
    var channel = await mail.GetChannel(_clientInstance);
    var user = await mail.GetUser(_clientInstance);

    var candm = true;
    IUserMessage msg = null;
    
    try
    {
      msg = await user.SendMessageAsync(".");
    }
    catch (HttpException ex)
    {
      candm = false;
    }
    finally
    {
      await msg?.DeleteAsync()!;
    }
      
    embed.WithTitle("Modmail Diagnostics");
    embed.AddField("ID", mail.Id);
    embed.AddField("Can DM user", candm.ToString());

    await Context.Interaction.RespondAsync(embed:embed.Build());

  }


  [SlashCommand("dispose", "Dispose a modmail channel"),
  RequireUserPermission(GuildPermission.ManageChannels),
  RequirePremium]
  public async Task DisposeModmail()
  {
    var mails = await DbService.GetModMails();

    if (!mails.Any(x => (ulong)x.ChannelSnowflake == Context.Channel.Id))
    {
      await Context.Interaction.RespondAsync("This channel is not a modmail channel");
      return;
    }

    var mail = mails.First(x => (ulong)x.ChannelSnowflake == Context.Channel.Id);

    try
    {
      await mail.CloseAsync(_clientInstance, DbService);
    }
    catch (Exception ex)
    {
      var chan = await mail.GetChannel(clientInstance: _clientInstance);
      await DbService.RemoveModMail(mail.Id);
      await chan.DeleteAsync();
    }

    
  }
  
}