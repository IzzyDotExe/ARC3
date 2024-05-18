using Arc3.Core.Schema;
using Arc3.Core.Services;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;

namespace Arc3.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequirePremiumAttribute : PreconditionAttribute
{

    public RequirePremiumAttribute()
    {
    }

    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services)
    {

        var dbService = services.GetRequiredService<DbService>();
        var guildInfos = await dbService.GetItemsAsync<GuildInfo>("Guilds");

        var guild = context.Guild;
        var config = guildInfos.First(x => x.GuildSnowflake == guild.Id.ToString());
        if (config.Premium) return PreconditionResult.FromSuccess();
        
        await context.Interaction.RespondAsync("This feature requires " + context.Client.CurrentUser.Username + " Premium. Contact @ox.izzy to learn more.");
        return PreconditionResult.FromError(new Exception("NoPremium"));

    }
}