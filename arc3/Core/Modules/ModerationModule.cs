
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
  }

  #region User Notes
  
  private async Task ButtonInteractionCreated(SocketMessageComponent ctx) {
    if (ctx.Data.CustomId.StartsWith("viewnotes."))
      await ViewNotes(ctx);
    
    if (ctx.Data.CustomId.StartsWith("addnote."))
      await AddUserNote(ctx);

    if (ctx.Data.CustomId.StartsWith("usernote.delete."))
      await DeleteUserNote(ctx);
  }

  private async Task AddUserNote(SocketMessageComponent ctx) {

  }

  private async Task DeleteUserNote(SocketMessageComponent ctx) {

  }

  private async Task ViewNotes(SocketMessageComponent ctx) {
    
    await ctx.DeferAsync(true);

    ulong userSnowflake = ulong.Parse(ctx.Data.CustomId.Split('.')[1]);

    List<UserNote> notes = await DbService.GetUserNotes(userSnowflake, ctx.GuildId??0);
    
    List<Page> pages=  new List<Page>();
    foreach (var note in notes) {
      var embed = note.CreateEmbed(_clientInstance);
      var page = new Page(embed:embed);
      var comp = new ActionRowBuilder()
        .WithButton(new ButtonBuilder()
          .WithStyle(ButtonStyle.Danger)
          .WithCustomId($"usernote.delete.{note.Id}")
          .WithLabel("Delete")
          .WithEmote(new Emoji("🗑️"))).Build();
      page.Components = new List<ActionRowComponent>() { comp };
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


}