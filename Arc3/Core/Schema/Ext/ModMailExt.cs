using System.Threading.Channels;
using Arc3.Core.Ext;
using Arc3.Core.Services;
using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;

namespace Arc3.Core.Schema.Ext;

public static class ModMailExt
{
    public static SocketUser GetUser(this ModMail self, DiscordSocketClient clientInstance) {
        var user = clientInstance.GetUser(((ulong)self.UserSnowflake));
        return user;
    }

    public static async Task<ITextChannel> GetChannel(this ModMail self, DiscordSocketClient clientInstance) {
        var channel = await clientInstance.GetChannelAsync((ulong)self.ChannelSnowflake);
        return (ITextChannel)channel;
    }   

    public static async Task<IWebhook> GetWebhook(this ModMail self, DiscordSocketClient clientInstance)
    {
        var  channel = await clientInstance.GetChannelAsync((ulong)self.ChannelSnowflake);
        var interactionchannel = (IIntegrationChannel)channel;

        var webhook = await interactionchannel.GetWebhookAsync((ulong)self.WebhookSnowflake);
        return webhook;
    }
    
    public static async Task SendUserAsync(this ModMail self, SocketMessage msg, DiscordSocketClient clientInstance, DbService dbService)
    {
        var embed = new EmbedBuilder()
            .WithModMailStyle(clientInstance)
            .WithAuthor(msg.Author.Username, msg.Author.GetDisplayAvatarUrl())
            .WithDescription(msg.Content)
            .Build();

        var channel = await self.GetChannel(clientInstance);
    
        var transcript = new Transcript {
            Id = msg.Id.ToString(),
            ModMailId = self.Id,
            SenderSnowfake = ((long)msg.Author.Id),
            AttachmentURls = msg.Attachments.Select(x => x.ProxyUrl).ToArray(),
            CreatedAt = msg.CreatedAt.UtcDateTime,
            GuildSnowflake = ((long)channel.Guild.Id),
            MessageContent = msg.Content
        };

        await dbService.AddTranscriptAsync(transcript);
        
        // Send the message
        if (!string.IsNullOrWhiteSpace(msg.Content))
            await self.GetUser(clientInstance).SendMessageAsync(embed: embed);
        
        // Share attachments
        if (msg.Attachments.Count > 0)
        {
            foreach (var attachment in msg.Attachments)
            {
                var emb = new EmbedBuilder()
                    .WithModMailStyle(clientInstance)
                    .WithAuthor(msg.Author.Username, msg.Author.GetDisplayAvatarUrl())
                    .WithDescription("You recieved an image")
                    .WithImageUrl(attachment.ProxyUrl)
                    .Build();
                
                await self.GetUser(clientInstance).SendMessageAsync(embed: emb);
                
            }
        }
        
    }

    public static async Task SendUserSystem(this ModMail self, DiscordSocketClient clientInstance, string content)
    {
        var author = clientInstance.CurrentUser;

        var embed = new EmbedBuilder()
            .WithModMailStyle(clientInstance)
            .WithAuthor(author.Username, author.GetDisplayAvatarUrl())
            .WithDescription(content)
            .Build();

        await self.GetUser(clientInstance).SendMessageAsync(embed: embed);
    }

    public static async Task SendMods(this ModMail self, SocketMessage msg, DiscordSocketClient clientInstance, DbService dbService)
    {
   

        DiscordWebhookClient client = new DiscordWebhookClient(await self.GetWebhook(clientInstance));

        await client.SendMessageAsync(msg.CleanContent, avatarUrl: msg.Author.GetDisplayAvatarUrl());
        
        var channel = await self.GetChannel(clientInstance);

        var transcript = new Transcript {
            Id = msg.Id.ToString(),
            ModMailId = self.Id,
            SenderSnowfake = ((long)msg.Author.Id),
            AttachmentURls = msg.Attachments.Select(x => x.ProxyUrl).ToArray(),
            CreatedAt = msg.CreatedAt.UtcDateTime,
            GuildSnowflake = ((long)channel.Guild.Id),
            MessageContent = msg.Content
        };

        await dbService.AddTranscriptAsync(transcript);

        if (msg.Attachments.Count > 0)
        {
            foreach (var att in msg.Attachments)
            {
                await client.SendMessageAsync(att.ProxyUrl, avatarUrl: msg.Author.GetDisplayAvatarUrl());
            }
        }
    }

    public static async Task SendModMailMenu(this ModMail self, DiscordSocketClient clientInstance, Appeal? appeal = null)
    {

        var user = self.GetUser(clientInstance);

        var embed = appeal == null? 
            new EmbedBuilder()
                .WithModMailStyle(clientInstance)
                .WithTitle("Modmail")
                .WithDescription($"A Modmail sesseion was opened by {user.Mention}")
                .Build()
            :
            new EmbedBuilder()
                .WithModMailStyle(clientInstance)
                .WithTitle("Modmail")
                .WithDescription($"Modmail for {user.Mention}'s ban appeal")
                .Build();
        
        var buttons = appeal == null?
            new ComponentBuilder()
                .WithRows(new List<ActionRowBuilder> {
                    new ActionRowBuilder()
                        .WithButton("Save and Close", 
                                    $"modmail.save.{self.Id}",
                                    ButtonStyle.Secondary,
                                    new Emoji("📝"))
                        .WithButton("Close",
                                    $"modmail.close.{self.Id}",
                                    ButtonStyle.Danger,
                                    new Emoji("🔒"))
                        .WithButton("Ban",
                                    $"modmail.ban.{self.Id}",
                                    ButtonStyle.Danger,
                                    new Emoji("🔨"))
                })
                .Build()
            :
            new ComponentBuilder()
                .WithRows(new List<ActionRowBuilder> {
                    new ActionRowBuilder()
                        .WithButton("Save and Close", 
                                    $"modmail.save.{self.Id}",
                                    ButtonStyle.Secondary,
                                    new Emoji("📝"))
                        .WithButton("Close",
                                    $"modmail.close.{self.Id}",
                                    ButtonStyle.Danger,
                                    new Emoji("🔒"))
                        // TODO: Add Unban and Deny buttons
                        // .WithButton("Ban",
                        //             $"modmail.ban.{self.Id}",
                        //             ButtonStyle.Danger,
                        //             new Emoji("🔨"))
                })
                .Build();

        var channel = await self.GetChannel(clientInstance);
        await channel.SendMessageAsync(embed:embed, components: buttons);
    }

    public static async Task<bool> InitAsync(this ModMail self, DiscordSocketClient clientInstance, SocketGuild guild, SocketUser user, DbService dbService) {

        self.UserSnowflake = ((long)user.Id);

        if (guild is null)
            return false;

        var guildConfig = dbService.Config[guild.Id];
        var modmails = await dbService.GetModMails();
        
        if (modmails.Any(x => x.UserSnowflake == self.UserSnowflake))
            return false;

        if (!guildConfig.ContainsKey("modmailchannel"))
            return false;
        
        var modmailChannelSnowflake = ulong.Parse(guildConfig["modmailchannel"]);
        var mailCategory = guild.GetCategoryChannel(modmailChannelSnowflake);

        if (!(mailCategory.GetChannelType() == ChannelType.Category))
            return false;
        
        var mailChannel = await guild.CreateTextChannelAsync(
            $"Modmail-{user.Username}",
            x => {
                x.ChannelType = ChannelType.Text;
                x.CategoryId = mailCategory.Id;
            }
        );

        var webhook = await mailChannel.CreateWebhookAsync(user.Username);

        self.Id = Guid.NewGuid().ToString();
        self.ChannelSnowflake = ((long)mailChannel.Id);
        self.WebhookSnowflake = ((long)webhook.Id);

        await dbService.AddModMail(self);

        return true;
        
    }

    public static async Task CloseAsync(this ModMail self, DiscordSocketClient client, DbService dbService) {
        await dbService.RemoveModMail(self.Id);
        var channel = await self.GetChannel(client);
        await channel.DeleteAsync();
    }

    /// <summary>
    /// DEPROCATED METHOD
    /// </summary>
    /// <param name="self"></param>
    /// <param name="client"></param>
    /// <param name="dbService"></param>
    public static async Task SaveTranscriptAsync(this ModMail self, DiscordSocketClient client, DbService dbService) {
        
        var channel = await self.GetChannel(client);
        SocketGuild guild;
        guild = client.GetGuild(channel.GuildId);
        
        var messages = channel.GetMessagesAsync(2000);
        var transcripts = new List<Transcript>();

        await messages.ForEachAsync(x => {
            foreach (var message in x) {

                var transcript = new Transcript {
                    Id = message.Id.ToString(),
                    ModMailId = self.Id,
                    SenderSnowfake = ((long)message.Author.Id),
                    AttachmentURls = message.Attachments.Select(x => x.ProxyUrl).ToArray(),
                    CreatedAt = message.CreatedAt.UtcDateTime,
                    GuildSnowflake = ((long)guild.Id),
                    MessageContent = message.Content
                };
                
                transcripts.Add(transcript);
            }
        });

        await dbService.AddTranscriptsAsync(transcripts);

    }

}