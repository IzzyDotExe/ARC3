using Arc3.Core.Schema;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Arc3.Core.Services;

public class ModMailService : ArcService
{
    
    
    private List<long> ActiveChannels { get; }
    
    private readonly DbService _dbService;
    
    public ModMailService(DiscordSocketClient clientInstance, InteractionService interactionService,
        DbService dbService)
        : base(clientInstance, interactionService, "MODMAIL")
    {
        _dbService = dbService;
        clientInstance.MessageReceived += ClientInstanceOnMessageReceived;
    }

    private async Task ClientInstanceOnMessageReceived(SocketMessage arg)
    {
     
        // Non private messages are handled as from a moderator
        if (arg.Channel.GetChannelType() != ChannelType.DM)
        {
            
            // Quit if the message is from a bot
            if (arg.Author.IsBot)
                return;
                
            // Handle the mail message 
            await HandleMailChannelMessage(arg);
            return;
        }
        
        
        // Private messages are handled as from a user
        
        
    }
    
    private async Task HandleMailChannelMessage(SocketMessage msg)
    {
        
    }
    
}