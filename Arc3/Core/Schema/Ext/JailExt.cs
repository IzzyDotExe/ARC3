


using Arc3.Core.Services;
using Discord;
using Discord.WebSocket;

namespace Arc3.Core.Schema.Ext;

public static class JailExt {

  
  public static async Task<ITextChannel> GetChannel(this Jail self, DiscordSocketClient clientInstance) {
    var channel = await clientInstance.GetChannelAsync((ulong)self.ChannelSnowflake);
    return (ITextChannel)channel;
  }   
  
  public static SocketUser GetUser(this Jail self, DiscordSocketClient clientInstance) {
    var user = clientInstance.GetUser(((ulong)self.UserSnowflake));
    return user;
  }

  
  public static async Task<bool> InitAsync(this Jail self, DiscordSocketClient clientInstance, SocketGuild guild,
    SocketUser user, DbService dbService) 
  {

    self.UserSnowflake = ((long)user.Id);

    if (guild is null)
      return false;

    var guildConfig = dbService.Config[guild.Id];
    var jails = await dbService.GetJailsAsync();

    if (jails.Any(x => x.UserSnowflake == self.UserSnowflake))
      return false;

    if (!guildConfig.ContainsKey("jailchannel"))
      return false;
    
    var jailChannelSnowflake = ulong.Parse(guildConfig["jailchannel"]);
    var jailCategory = guild.GetCategoryChannel(jailChannelSnowflake);

    if (jailCategory.GetChannelType() != ChannelType.Category)
      return false;

    var perms = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, useApplicationCommands: PermValue.Deny);
    var jailChannel = await guild.CreateTextChannelAsync(
      $"Jail-{user.Username}",
      x =>
      {
        x.ChannelType = ChannelType.Text;
        x.CategoryId = jailCategory.Id;
      }
    );
    await jailChannel.AddPermissionOverwriteAsync(user, perms);
    
    await jailChannel.SendMessageAsync($"{user.Mention} you have been jailed!");

    try {
      var jailedRoleSnowflake = ulong.Parse(guildConfig["jailedrole"]);
      var jailRole = guild.GetRole(jailedRoleSnowflake);

      var guser = guild.GetUser(user.Id);
      await guser.AddRoleAsync(jailRole);
    } catch (Exception e) {
      await jailChannel.SendMessageAsync("Jail role not found or failed to give role to user!");
    }

    
    self.Id = Guid.NewGuid().ToString();
    self.ChannelSnowflake = ((long)jailChannel.Id);
    
    await dbService.AddJailAsync(self);

    return true;

  }

  public static async Task DestroyAsync(this Jail self, DiscordSocketClient clientInstance, DbService dbService)
  {

    // Get the channel, User and Guild
    var channel = await self.GetChannel(clientInstance);
    var guild = channel.Guild;
    var user = await guild.GetUserAsync((ulong)self.UserSnowflake);
    
    // Remove the jail from the db
    await dbService.DeleteJailAsync(self);
    
    // Remove the channel
    await channel.DeleteAsync();
      
    // Unjail the user Remove the jailed role
    var guildConfig = dbService.Config[channel.GuildId];
    var jailedRoleSnowflake = ulong.Parse(guildConfig["jailedrole"]);
    
    var jailRole = guild.GetRole(jailedRoleSnowflake);
  

    await user.RemoveRoleAsync(jailRole);

  }
  
}
