using Discord.Interactions;

namespace Arc3.Core.Modules;

public abstract class ArcModule : InteractionModuleBase<SocketInteractionContext> {

  protected static bool _loaded = false;

  public ArcModule() {

    if (!_loaded)
      RegisterListeners();
    
    _loaded = true;

  }

  public abstract void RegisterListeners();

}