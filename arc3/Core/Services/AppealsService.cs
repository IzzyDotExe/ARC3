using Arc3.Core.Schema;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Arc3.Core.Services;

public class AppealsService : ArcService {
  
  private readonly DbService _dbService;
  private readonly IMongoCollection<Appeal> _appealCollection;
  
  public AppealsService(DiscordSocketClient clientInstance, InteractionService interactionService,
    DbService dbService)
    : base(clientInstance, interactionService, "APPEALS") {
      _dbService = dbService;
      _appealCollection = dbService.GetCollection<Appeal>("appeals");
    }

  public async Task AppealsChanged() {

    while (true)
    {
      var changes = await _appealCollection.WatchAsync();

      await changes.ForEachAsync(doc =>
      {

        // Get the previous document
        var previous = doc.FullDocumentBeforeChange;

        // Get the document
        var appeal = doc.FullDocument;
        
        // Fetch the appeals channel
        var appealChannel = _dbService.Config[ulong.Parse(Environment.GetEnvironmentVariable("GUILD_ID") ?? string.Empty)]["appealChannel"];
        
        // Fetch the channel 
        var channel = _clientInstance.GetChannelAsync(ulong.Parse(appealChannel));

        while (!channel.IsCompleted) ;

      });

    }
    
  }
  

}