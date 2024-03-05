
using Discord;
using Discord.WebSocket;

namespace Arc3.Core.Schema.Ext;

public static class UserNoteExt {

  public static SocketUser GetUser(this UserNote self, DiscordSocketClient clientInstance) {
    var guild = clientInstance.GetGuild(((ulong)self.GuildSnowflake));
    var user = guild.GetUser(((ulong)self.UserSnowflake));
    return user;
  }

  public static SocketUser GetAuthor(this UserNote self, DiscordSocketClient clientInstance) {
    var guild = clientInstance.GetGuild(((ulong)self.GuildSnowflake));
    var user = guild.GetUser(((ulong)self.AuthorSnowflake));
    return user;
  }

  public static EmbedBuilder CreateEmbed(this UserNote self, DiscordSocketClient clientInstance) {
    var user = self.GetUser(clientInstance);
    var author = self.GetAuthor(clientInstance);
    return new EmbedBuilder()
      .WithAuthor(new EmbedAuthorBuilder()
        .WithName($"{user} Note #{self.Id}")
        .WithIconUrl(user.GetDisplayAvatarUrl(ImageFormat.Auto)))
      .WithDescription($"```{self.Note}```")
      // TODO: Add timestamp
      // .WithTimestamp();
      .WithFooter($"Note added by {author.Username}", author.GetDisplayAvatarUrl(ImageFormat.Auto));
  }
}