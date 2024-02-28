using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Arc3.Core.Services;

public class DbService : ArcService {

  static string DB_NAME = Environment.GetEnvironmentVariable("DB_NAME")?? "Arc3";

  private readonly MongoClient mongoClient;

  public DbService(DiscordSocketClient clientInstance, InteractionService interactionService) 
  : base(clientInstance, interactionService, "Database" ) {

    // Retrieve the URI from the .env file
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");

    if (string.IsNullOrEmpty(connectionString)) 
    {
      Console.WriteLine("You must set your 'MONGODB_URI' environment variable.");
      Environment.Exit(0); // Shuts down the bot if the URI is not set
    }
    else
    {
      // Initialize the MongoClient with the URI
      mongoClient = new MongoClient(connectionString);
    }

    // Check the connection
    try
    {
      Console.WriteLine("Connecting to MongoDB...");
      mongoClient.GetDatabase(DB_NAME).RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();
      Console.WriteLine("Connected to MongoDB.");
    }
    catch (Exception ex)
    {
      Console.WriteLine("Error connecting to MongoDB: " + ex.Message);
      Environment.Exit(0); // Shuts down the bot if the connection fails
    }

  }

  // Access the database and collection
  public IMongoCollection<T> GetCollection<T>(string name)
  {
    var database = mongoClient.GetDatabase(DB_NAME);
    return database.GetCollection<T>(name);
  }

}
