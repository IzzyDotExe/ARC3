using Arc3.Core.Services;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;

namespace Arc3.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class KaraokePreconditionAttribute(bool respectLock = true) : PreconditionAttribute
{

    private bool _respectLock = respectLock;

    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context, 
        ICommandInfo command, 
        IServiceProvider services)
    {
        var karaokeService = services.GetRequiredService<KaraokeService>();
        var guild = context.Guild;
        var guildUser = await guild.GetUserAsync(context.User.Id);
        
        // Guard if the user has a voice state.
        if (guildUser.VoiceChannel is null) {
            await context.Interaction.RespondAsync("You must be in a voice channel to join a queue!", ephemeral: true);
            return PreconditionResult.FromError(new Exception("User not in voice"));
        }

        // Guard if the invoker is using it in the voice channel chat.
        if (context.Channel.Id != guildUser.VoiceChannel.Id) {
            await context.Interaction.RespondAsync($"You must use this command inside a voice channel chat! {guildUser.VoiceChannel.Mention}", ephemeral: true);
            return PreconditionResult.FromError(new Exception("Invoker is not in vcc"));
        }
        
        // Guard If the user has mute members allow
        if (guildUser.GuildPermissions.MuteMembers)
        {
            return PreconditionResult.FromSuccess();
        }
        
        // Guard  If the channel is not locked or we do not respect locks allow
        if (!_respectLock || !karaokeService.ChannelCache[guildUser.VoiceChannel.Id].Locked)
        {
            return PreconditionResult.FromSuccess();
        }
        
        // Guard If the user is admin allow
        if (karaokeService.ChannelCache[guildUser.VoiceChannel.Id].AdminSnowflake == guildUser.Id)
        {
            return PreconditionResult.FromSuccess();
        }
        
        // Deny
        await context.Interaction.RespondAsync($"You do not have permission to use that command!", ephemeral: true);
        return PreconditionResult.FromError(new Exception("Invoker does not have permission"));
            
    }
}