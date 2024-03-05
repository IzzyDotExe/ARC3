using Discord.Interactions;
using Discord.WebSocket;
using DnsClient.Protocol;

namespace Arc3.Core.Modules;

public abstract class ArcModule : InteractionModuleBase<SocketInteractionContext> {

  private static Dictionary<string, bool> _loadedDict = new Dictionary<string, bool>();
  protected readonly DiscordSocketClient _clientInstance;

  public ArcModule(DiscordSocketClient clientInstance, string moduleName) {

    var loaded = _loadedDict.ContainsKey(moduleName);

    _clientInstance = clientInstance;
    
    if (loaded)
      return;

    RegisterListeners(); 

    Console.WriteLine($"MODULE LOADED: {moduleName}");
    _loadedDict[moduleName] = true;
    
  }

  public abstract void RegisterListeners();

}