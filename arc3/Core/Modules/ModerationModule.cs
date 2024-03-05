
using Arc3.Core.Schema;
using Arc3.Core.Schema.Ext;
using Arc3.Core.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Modules;


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

  [UserCommand("User Notes")]
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
  
  
  #endregion

}