using Arc3.Core.Schema;
using Arc3.Core.Services;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;

namespace Arc3.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RequireCommandBlacklistAttribute : PreconditionAttribute
{

    public RequireCommandBlacklistAttribute()
    {
    }

    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services)
    {

        var dbService = services.GetRequiredService<DbService>();
        var blacklists = await dbService.GetItemsAsync<Blacklist>("blacklist");

        var cmd = commandInfo.Name.ToLower();

        if (blacklists.Any(x => (x.GuildSnowflake == ((long)context.Guild.Id) || x.GuildSnowflake == 0) && x.UserSnowflake == ((long)context.User.Id) && (x.Command == "all" || x.Command == cmd))) {
            await context.Interaction.RespondAsync("You are blacklisted from using this command.");
            return PreconditionResult.FromError(new Exception("Blacklisted"));
        }

        return PreconditionResult.FromSuccess();
  
    }
}