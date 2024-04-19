

using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Ext;


public static class SocketInteractionContextExt {
  public static bool CheckRoleHigher(this SocketInteractionContext ctx, SocketUser target) {
    var guildUser = ctx.Guild.GetUser(target.Id);
    var invoker = ctx.Guild.GetUser(ctx.User.Id);
    var highestRole = guildUser.Roles.OrderBy(x => x.Position).Last();
    var myHigestRole = invoker.Roles.OrderBy(x => x.Position).Last();
    return highestRole.Position > myHigestRole.Position;
  }
}