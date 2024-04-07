using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Services;

public abstract class ArcService 
{
  
  protected readonly DiscordSocketClient _clientInstance;
  protected readonly InteractionService _interactionService;

  protected ArcService(DiscordSocketClient clientInstance, InteractionService interactionService, string serviceName) 
  {

    _clientInstance = clientInstance;
    _interactionService = interactionService;

    Console.WriteLine("LOADED SERVICE: " + serviceName);
    
  }
}