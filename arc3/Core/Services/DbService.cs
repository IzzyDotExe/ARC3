using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson;
using Arc3.Core.Schema;

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

  public async Task<List<UserNote>> GetUserNotes(ulong userSnowflake, ulong guildSnowflake) {
    
    // Get the notes collection
    var notesCollection = GetCollection<UserNote>("user_notes");
    
    // Build a filter for the searching the notes collection
    var filter = Builders<UserNote>.Filter.Where(x => 
      x.GuildSnowflake == (long)guildSnowflake && 
      x.UserSnowflake == (long)userSnowflake
    );

    // Get the user notes
    var notes = await notesCollection.FindAsync(filter);

    return await notes.ToListAsync();

  }

  public async Task AddUserNote(UserNote note) {
    
    var allFilter = Builders<UserNote>.Filter.Where(x=>true);
    
    IMongoCollection<UserNote> notes = GetCollection<UserNote>("user_notes");

    note.Id = (await notes.CountDocumentsAsync(allFilter) + 1).ToString();

    await notes.InsertOneAsync(note);

  }

  public async Task RemoveUserNote(string id) {

    IMongoCollection<UserNote> notes = GetCollection<UserNote>("user_notes");
    var filter = Builders<UserNote>.Filter.Where(x=>x.Id == id);
    await notes.DeleteOneAsync(filter);

  }

  // Get Active modmails
  public async Task<List<ModMail>> GetModMails()
  {
    
    // Get the modmail collection
    var modmailscollection = GetCollection<ModMail>("mod_mails");
    
    // Build a filter for searching all the modmails
    var filter = Builders<ModMail>.Filter.Where(x => 
      true
    );
    
    // Get the modmails
    var mails = await modmailscollection.FindAsync(filter);

    return await mails.ToListAsync();

  }
  
  // Add a modmail
  public async Task AddModMail(ModMail mail)
  {
    
    var allFilter = Builders<ModMail>.Filter.Where(x => true);
    
    IMongoCollection<ModMail> mails = GetCollection<ModMail>("mod_mails");
    
    mail.Id = (await mails.CountDocumentsAsync(allFilter) + 1).ToString();
    
    await mails.InsertOneAsync(mail);
    
  }
  
  // Delete a modmail
  public async Task RemoveModMail(string id)
  {
    var mails = GetCollection<ModMail>("mod_mails");
    var filter = Builders<ModMail>.Filter.Where(x => x.Id == id);
    await mails.DeleteOneAsync(filter);
  }
  
}
