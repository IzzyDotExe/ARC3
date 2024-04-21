

using System.Threading.Channels;
using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Services;


public class ChannelStatus
{
  public bool Locked { get; set; } = false;
  public ulong AdminSnowflake { get; set; } = 0;
}

public class DefaultDict<TKey, TValue>(TValue defaultValue) : Dictionary<TKey, TValue>
  where TKey : notnull
{
  public TValue this[TKey i] => base.ContainsKey(i) ? base[i] : defaultValue;
}

public class KaraokeService : ArcService {
  
  private readonly DbService _dbService;
  private readonly Random _random;

  public DefaultDict<ulong, ChannelStatus> ChannelCache { get; set; }
  public KaraokeService(DiscordSocketClient clientInstance, InteractionService interactionService,
    DbService dbService)
    : base(clientInstance, interactionService, "KARAOKE") {
      _random = new Random();
      _dbService = dbService;
      
      ChannelCache = new DefaultDict<ulong, ChannelStatus>(new ChannelStatus());
      _clientInstance.UserVoiceStateUpdated += ClientInstanceOnUserVoiceStateUpdated;
  }

  private Task ClientInstanceOnUserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
  {
    
    // Console.WriteLine("Voice");

    // If the before state has a channel
    if (before.VoiceChannel != null)
    {
      // if the user is owner, remove them as owner
      if (ChannelCache[before.VoiceChannel.Id].AdminSnowflake == user.Id)
      {
        ChannelCache[before.VoiceChannel.Id].AdminSnowflake = 0;
      }
      // If there is anyone else in the channel, pick someone at random to be the nw owner
      if (before.VoiceChannel.ConnectedUsers.Count > 0)
      {
        ChannelCache[before.VoiceChannel.Id].AdminSnowflake = before.VoiceChannel.ConnectedUsers.ToList()[_random.Next(before.VoiceChannel.ConnectedUsers.Count)].Id;
      }
        
      // Send feedback message?
      // Not now
      
    }

    // If the new state has a channel
    if (after.VoiceChannel != null)
    {
      // If the channel is empty Make this user the admin of that channel.
      if (after.VoiceChannel.ConnectedUsers.Count == 1)
      {
        ChannelCache[after.VoiceChannel.Id].AdminSnowflake = user.Id;
      }
    }

    return Task.CompletedTask;
  }
}
