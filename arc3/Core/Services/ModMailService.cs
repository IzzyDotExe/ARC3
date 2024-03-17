using Arc3.Core.Ext;
using Arc3.Core.Schema;
using Arc3.Core.Schema.Ext;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Services;

public class ModMailService : ArcService
{
    
    private readonly DbService _dbService;
    
    public ModMailService(DiscordSocketClient clientInstance, InteractionService interactionService,
        DbService dbService)
        : base(clientInstance, interactionService, "MODMAIL")
    {
        _dbService = dbService;
        clientInstance.MessageReceived += ClientInstanceOnMessageReceived;
        clientInstance.ButtonExecuted += ButtonInteractionCreated;
        clientInstance.ModalSubmitted += ModalInteractionCreated;
    }

    private async Task ModalInteractionCreated(SocketModal ctx) {
        
        var eventId = ctx.Data.CustomId;
        String reason = ctx.Data.Components.First(x => x.CustomId == "modmail.ban.reason").Value;


        if (!eventId.StartsWith("modmail"))
            return;

        var eventAction = _clientInstance.GetEventAction(eventId);

        if (eventAction == null)
            return;

        var modmails = await _dbService.GetModMails();

        ModMail modmail;

        try {
            modmail = modmails.First(x => x.Id == eventAction.Value.Item2);
        } catch (InvalidDataException) {
            // TODO: Log failed to get modmail
            return;
        }

        switch (eventAction.Value.Item1) {
            case "modmail.ban.confirm":
                await ctx.DeferAsync();
                var member = modmail.GetUser(_clientInstance);
                await SaveModMailSession(modmail, ctx.User);
                await CloseModMailSession(modmail, ctx.User);
                await BanMailUser(member, reason, modmail);
                break;
        }

        await ctx.RespondAsync("👍🏾", ephemeral: true);
        
    }

    private async Task ButtonInteractionCreated(SocketMessageComponent ctx) {
        
        var eventId = ctx.Data.CustomId;

        if (!eventId.StartsWith("modmail"))
            return;

        var eventAction = _clientInstance.GetEventAction(eventId);

        if (eventAction == null)
            return;
        
        var modmails = await _dbService.GetModMails();
        var modmail = modmails.First(x=> x.Id == eventAction.Value.Item2);

        switch (eventAction.Value.Item1) {

            case "modmail.close":
                await CloseModMailSession(modmail, ctx.User);
                break;

            case "modmail.save":
                await SaveModMailSession(modmail, ctx.User);
                await CloseModMailSession(modmail, ctx.User);
                break;

            case "modmail.ban":
                await ConfirmBanUser(modmail, ctx);
                break;

            default:
                break;

        }

    }

    private async Task ConfirmBanUser(ModMail mail, SocketMessageComponent ctx) {
        
        var resp = new ModalBuilder()
            .WithTitle("Are you sure you want to ban this user?")
            .WithCustomId($"modmail.ban.confirm.{mail.Id}")
            .AddTextInput(new TextInputBuilder()
                .WithCustomId("modmail.ban.reason")
                .WithPlaceholder("reason")
                .WithRequired(true)
                .WithMaxLength(30))
            .Build();
        
        await ctx.RespondWithModalAsync(resp);

    }

    private async Task BanMailUser(SocketUser user, string reason, ModMail mail) {
        var author = _clientInstance.CurrentUser;
        var channel = await mail.GetChannel(_clientInstance);
        var guild = channel.Guild;
        var embed = new EmbedBuilder()
            .WithModMailStyle(_clientInstance)
            .WithAuthor(new EmbedAuthorBuilder()
                .WithName(author.Username)
                .WithIconUrl(author.GetAvatarUrl(ImageFormat.Auto)))
                .WithDescription($"You have been banned in {guild.Name} for: ``{reason}``")
                .Build();
        
        try {
            await user.SendMessageAsync(embed: embed);
        } catch {
            // TODO: Log Failed message send
        } finally {
            await guild.AddBanAsync(user, reason: $"Banned during modmail for: {reason}");
        }

    }

    private async Task ClientInstanceOnMessageReceived(SocketMessage arg)
    {
     
        // Non private messages are handled as from a moderator
        if (arg.Channel.GetChannelType() != ChannelType.DM)
        {
            
            // Quit if the message is from a bot
            if (arg.Author.IsBot)
                return;
                
            // Quit if the message is commented
            if (arg.Content.StartsWith('#'))
                return;
            
            // Handle the mail message 
            await HandleMailChannelMessage(arg);
            return;
        }
        
        
        // Private messages are handled as from a user
        var mails = await _dbService.GetModMails();
        
        if (!mails.Any(x=>(ulong)x.UserSnowflake == arg.Author.Id)) {

            // If there are no modmails in the database for this user, 
            // then we first check if they said modmail
            // if they did then we can start a session if not

            if (!arg.Content.ToLower().Contains("modmail") && 
                !arg.Content.ToLower().Contains("mod") && 
                !arg.Content.ToLower().Contains("mail"))
                return;

            // TODO: Insert server picking mechanism
            // For now choose the default guild
            var guild_id = Environment.GetEnvironmentVariable("GUILD_ID");
            ModMail? modmail = null;
            try {

                var guild = _clientInstance.GetGuild(ulong.Parse(guild_id??"0"));
                modmail = new ModMail();

                
                await modmail.InitAsync(_clientInstance, guild, arg.Author, _dbService);
                await modmail.SendUserSystem(_clientInstance, "Your modmail request was recieved! Please wait and a staff member will assist you shortly.");
                await modmail.SendModMailMenu(_clientInstance);
                
            } catch(Exception e) {
                
                // TODO: Log Failure 
                Console.WriteLine(e);

                if (modmail != null) {
                    var modmails = await _dbService.GetModMails();
                    if (modmails.Any(x => x.Id == modmail.Id)) 
                        await _dbService.RemoveModMail(modmail.Id);
                }

            }


        } else {

            if (arg.Author.IsBot)
                return;

            var modmail = mails.First(x=>(ulong)x.UserSnowflake == arg.Author.Id);

            if (arg.Content.ToLower().Equals("close session")) {
                
                await SaveModMailSession(modmail, arg.Author);
                await CloseModMailSession(modmail, arg.Author);
                return;

            }
            
            await modmail.SendMods(arg, _clientInstance, _dbService);

        }
        
        
    }

    private async Task CloseModMailSession(ModMail m, SocketUser user)
    {
        await m.SendUserSystem(_clientInstance, $"Your mod mail session was closed by {user.Mention}!");
        await m.CloseAsync(_clientInstance, _dbService);
    }

    private async Task SaveModMailSession(ModMail m, SocketUser s) {
        // await m.SaveTranscriptAsync(_clientInstance, _dbService);
        var channel = await m.GetChannel(_clientInstance);
        var guild = channel.Guild;
        var transcriptchannel = await _clientInstance.GetChannelAsync(ulong.Parse(_dbService.Config[guild.Id]["transcriptchannel"]));
        var user = m.GetUser(_clientInstance);
        var embed = new EmbedBuilder()
            .WithModMailStyle(_clientInstance)
            .WithTitle("Modmail Transcript")
            .WithDescription($"**Modmail with:** {user.Mention}\n**Saved** <t:{DateTimeOffset.Now.ToUnixTimeSeconds()}:R> **by** {s.Mention}\n\n[Transcript](https://billiecord.com/modmail/transcripts/{m.Id})")
            .Build();

        await ((SocketTextChannel)transcriptchannel).SendMessageAsync(embed: embed);
    }
    
    private async Task HandleMailChannelMessage(SocketMessage msg)
    {
    
        
        var mails = await _dbService.GetModMails();
        ModMail mail;
        try
        {
            mail = mails.First(x => x.ChannelSnowflake == (long)msg.Channel.Id);
        }
        catch (InvalidOperationException ex)
        {
            // No modmail exists
            // Console.WriteLine($"Failed to get modmail {ex}");
            return;
        }

        await mail.SendUserAsync(msg, _clientInstance, _dbService);
    }
    
}