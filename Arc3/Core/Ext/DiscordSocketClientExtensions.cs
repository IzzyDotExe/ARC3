
using Discord.WebSocket;

namespace Arc3.Core.Ext;

public static class DiscordSocketClientExtensions {
  public static (string, string)? GetEventAction(this DiscordSocketClient client, string eventId) {
    
    var indx = eventId.LastIndexOf('.');
    if (indx == -1) return null;
    
    var eventAction = eventId[..indx];
    var eventIdentifier = eventId.Substring(indx+1, Math.Abs(indx+1 - eventId.Length));
    return (eventAction, eventIdentifier);
    
  }
}