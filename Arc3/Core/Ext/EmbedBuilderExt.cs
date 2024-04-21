using Discord;
using Discord.WebSocket;

namespace Arc3.Core.Ext;

public static class EmbedBuilderExt
{
    public static EmbedBuilder WithModMailStyle(this EmbedBuilder builder, DiscordSocketClient clientInstance, string footer = "Modmail")
    {
        return builder
            .WithColor(Color.DarkRed)
            .WithTimestamp(DateTimeOffset.UtcNow)
            .WithFooter($"ARC v{Arc3.ArcVersion} - {footer}", clientInstance.CurrentUser.GetAvatarUrl());
    }
}