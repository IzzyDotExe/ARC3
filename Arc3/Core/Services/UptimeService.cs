using System.Diagnostics;
using Arc3.Core.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Services;

public class UptimeService : ArcService {

  private readonly Stopwatch _uptime = new Stopwatch();

  public Stopwatch Uptime => _uptime;

  public UptimeService(DiscordSocketClient clientInstance, InteractionService interactionService) 
  : base(clientInstance, interactionService, "Uptime") {
    _uptime.Start();
  }

}
