using System.Runtime.InteropServices.ComTypes;

using System.Diagnostics;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DnsClient.Protocol;
using Discord.Rest;

namespace Arc3.Core.Services;

public class PaginationEmojis {
  public static IEmote Left 
    => new Emoji("◀");
  public static IEmote Right
    => new Emoji("▶");
  public static IEmote SkipLeft
    => new Emoji("⏮");
  public static IEmote SkipRight
    => new Emoji("⏭");
  public static IEmote Stop
    => new Emoji("⏹");
}

public class Page {

  public string Content { get; set; }
  public Embed Embed { get; set; }
  public List<ActionRowComponent> Components { get; set; }

  public Page(string content = "", EmbedBuilder? embed = null, List<ActionRowComponent>? components = null) {
    this.Content = content;
    this.Embed = embed?.Build();
    this.Components = components ?? new List<ActionRowComponent>();
  }

}

public class PaginationSession {
  private SocketMessageComponent _interactionContext;

  private DiscordSocketClient _clientInstance;

  private RestInteractionMessage _message;

  private List<Page> _pages = new List<Page>();

  private int _pageIndex;

  private MessageComponent _paginationButtons {
    get {
      return new ComponentBuilder()
        .WithRows(new List<ActionRowBuilder>() {
          new ActionRowBuilder()
            .WithComponents(new List<IMessageComponent>() {
              new ButtonBuilder()
                .WithStyle(ButtonStyle.Primary)
                .WithCustomId("pagination.previous")
                .WithLabel(" ")
                .WithEmote(PaginationEmojis.Left)
                .WithDisabled(!(_pages.Count > 1)).Build(),
              new ButtonBuilder()
                .WithStyle(ButtonStyle.Primary)
                .WithCustomId("pagination.stop")
                .WithLabel(" ")
                .WithEmote(PaginationEmojis.Stop)
                .WithDisabled(false).Build(),
              new ButtonBuilder()
                .WithStyle(ButtonStyle.Primary)
                .WithCustomId("pagination.next")
                .WithLabel(" ")
                .WithEmote(PaginationEmojis.Right)
                .WithDisabled(!(_pages.Count > 1)).Build()
            })
        }).Build();
    }
  }

  public PaginationSession(SocketMessageComponent interactionContext, List<Page> pages, DiscordSocketClient clientInstance) {
    _interactionContext = interactionContext;
    _pages = pages;
    _clientInstance = clientInstance;

    if (_pages.Count < 1)
      _pages.Add(new Page(embed:new EmbedBuilder()
        .WithTitle("No pages")
        .WithDescription("```No Pages```")
      ));

    Task.Run(async () => {
      await Task.Delay(60000);
      await Stop();
    });
    ;
  }

  public async Task Start() {
    _clientInstance.ButtonExecuted += PaginationInteractionCreated;
    _message = await _interactionContext.GetOriginalResponseAsync();
    await Update();
  }

  private async Task Stop() {
    await Update();
    _clientInstance.ButtonExecuted -= PaginationInteractionCreated;

    await _interactionContext.ModifyOriginalResponseAsync(x=> {
      x.Embed = _pages[_pageIndex].Embed;
      x.Components = null;
      x.Content = _pages[_pageIndex].Content;
    }); 
  }

  private async Task Update() {
    await _interactionContext.ModifyOriginalResponseAsync(x => {
      x.Embed = _pages[_pageIndex].Embed;
      // x.Components = _pages[_pageIndex].Components;
      x.Components = _paginationButtons;
      x.Content = _pages[_pageIndex].Content;
    });
  }

  public void IncIndex()
  {
    if (_pageIndex >= _pages.Count - 1)
      _pageIndex = _pageIndex - _pages.Count;

    _pageIndex++;
  }

  public void DecIndex()
  {
    if (_pageIndex <= 0)
      _pageIndex = _pages.Count;
    _pageIndex--;
  }

  private async Task PaginationInteractionCreated(SocketMessageComponent ctx) {
    if (ctx.Message.Id != _message.Id)
      return;

    await ctx.DeferAsync();

    switch (ctx.Data.CustomId) {
      case "pagination.previous":
        DecIndex();
        await Update();
        break;

      case "pagination.stop":
        await Stop();
        break;

      case "pagination.next":
        IncIndex();
        await Update();
        break;
    }

    if (ctx.Data.CustomId.Contains("delete")) {

      var currentPage = _pages[_pageIndex];
      var comp = currentPage.Components.First(
        // Get the first row that has any component taht is a delete component.
        x => x.Components.Any(x=> x.CustomId.Contains("delete"))
      );
      currentPage.Components.Remove(comp);
      currentPage.Embed = new EmbedBuilder().WithDescription("```Deleted```").Build();
      
      await Update();

    }

  }
  
  
}

public class PaginationService : ArcService {

  public PaginationService(DiscordSocketClient clientInstance, InteractionService interactionService)
    : base(clientInstance, interactionService, "Pagination") {

    }

    public async Task CreatePaginationResponse(List<Page> pages, SocketMessageComponent interactionCtx) {
      PaginationSession session = new PaginationSession(interactionCtx, pages, _clientInstance);
      await session.Start();
    }

}