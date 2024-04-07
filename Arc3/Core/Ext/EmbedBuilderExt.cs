using Discord;
using Discord.WebSocket;

namespace Arc3.Core.Ext;

public static class EmbedBuilderExt
{
    public static EmbedBuilder WithModMailStyle(this EmbedBuilder builder, DiscordSocketClient clientInstance)
    {
        return builder
            .WithColor(Color.DarkRed)
            .WithTimestamp(DateTimeOffset.UtcNow)
            .WithFooter($"ARC v{Arc3.ArcVersion} - Modmail", clientInstance.CurrentUser.GetAvatarUrl());
    }
}