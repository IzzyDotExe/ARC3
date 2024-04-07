

using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Services;

public class JailService : ArcService {
  private readonly DbService _dbService;

  public JailService(DiscordSocketClient clientInstance, InteractionService interactionService,
    DbService dbService)
    : base(clientInstance, interactionService, "JAIL") {
      _dbService = dbService;
      
    }

}