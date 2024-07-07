using System.Collections.Immutable;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Driver;
using MongoDB.Bson;
using Arc3.Core.Schema;
using arc3.Core.Schema;
using System.Runtime.InteropServices;

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

  // Access for guild configs
  public Dictionary<ulong, Dictionary<string, string>> Config {
    get {

      Dictionary<ulong, Dictionary<string, string>> configs =  new();
      var allconfigs = GetGuildConfigs();
      foreach (var config in allconfigs) {
        if (!configs.ContainsKey((ulong)config.GuildSnowflake))
          configs[(ulong)config.GuildSnowflake] = new Dictionary<string, string>();

        configs[(ulong)config.GuildSnowflake][config.ConfigKey] = config.ConfigValue;
      }
      return configs;

    }
  }

  // Get all Guild Configs
  public async Task<List<GuildConfig>> GetGuildConfigsAsync() {
    var configsCollection = GetCollection<GuildConfig>("guild_configs");
    var filter = Builders<GuildConfig>.Filter.Where(x => true);
    var configs = await configsCollection.FindAsync(filter);
    return await configs.ToListAsync();
  }

  public async Task<IEnumerable<T>> GetItemsAsync<T>(string collection)
  {
    var itemColletion = GetCollection<T>(collection);
    var items = await itemColletion.FindAsync(Builders<T>.Filter.Where(x => true));
    return await items.ToListAsync();
  }

  public List<GuildConfig> GetGuildConfigs() {
    var configsCollection = GetCollection<GuildConfig>("guild_configs");
    var filter = Builders<GuildConfig>.Filter.Where(x => true);
    var configs = configsCollection.Find(filter);
    return configs.ToList();
  }

  public async Task SetGuildConfigAsync(GuildConfig config) {
    
    var configsCollection = GetCollection<GuildConfig>("guild_configs");
   
    var filter = Builders<GuildConfig>.Filter.Where(x => 
      x.GuildSnowflake == config.GuildSnowflake &&
      x.ConfigKey.Equals(config.ConfigKey)
    );

    await configsCollection.ReplaceOneAsync(filter, config, options: new ReplaceOptions() { IsUpsert = true });
  
  }

  // Get all usernotes
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

  // Add a usernote
  public async Task AddUserNote(UserNote note) {
    
    var allFilter = Builders<UserNote>.Filter.Where(x=>true);
    
    IMongoCollection<UserNote> notes = GetCollection<UserNote>("user_notes");

    note.Id = (await notes.CountDocumentsAsync(allFilter) + 1).ToString();

    await notes.InsertOneAsync(note);

  }

  // Remove a usernote
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
    
    await mails.InsertOneAsync(mail);
    
  }
  
  // Delete a modmail
  public async Task RemoveModMail(string id)
  {
    var mails = GetCollection<ModMail>("mod_mails");
    var filter = Builders<ModMail>.Filter.Where(x => x.Id == id);
    await mails.DeleteOneAsync(filter);
  }
  
  public async Task AddTranscriptsAsync(List<Transcript> transcripts) {
    IMongoCollection<Transcript> transcriptCollection = GetCollection<Transcript>("transcripts");
    await transcriptCollection.InsertManyAsync(transcripts);
  }

  public async Task AddTranscriptAsync(Transcript transcript) {
    IMongoCollection<Transcript> transcriptCollection = GetCollection<Transcript>("transcripts");
    await transcriptCollection.InsertOneAsync(transcript);
  }

  public async Task UpdateTranscriptAsync(SocketMessage msg)
  {
    IMongoCollection<Transcript> transcriptCollection = GetCollection<Transcript>("transcripts");
    var filter = Builders<Transcript>.Filter.Where(x => x.Id == msg.Id.ToString());
    var update = Builders<Transcript>.Update.Set(x => x.MessageContent, msg.Content)
      .Set(x => x.AttachmentURls, msg.Attachments.Select(x => x.ProxyUrl).ToArray());
    await transcriptCollection.UpdateOneAsync(filter, update);
  }

  public async Task UpdateInsightDataAsync(Insight item)
  {
    IMongoCollection<Insight> collection = GetCollection<Insight>("Insights");
    var filter = Builders<Insight>.Filter.Where(x => x.Id == item.Id);
    var update = Builders<Insight>.Update.Set(x => x.Data, item.Data).Set(x => x.Date, item.Date);
    await collection.UpdateOneAsync(filter, update);
  }
  
  public async Task<List<Jail>> GetJailsAsync() {
    
    // Get the jails colleciton
    var jailCollection = GetCollection<Jail>("jails");

    var filter = Builders<Jail>.Filter.Where(x=>true);

    var jails = await jailCollection.FindAsync(filter);

    return await jails.ToListAsync();

  }

  public async Task AddJailAsync(Jail jail) {

    var allFilter = Builders<Jail>.Filter.Where(x => true);

    IMongoCollection<Jail> jails = GetCollection<Jail>("jails");

    await jails.InsertOneAsync(jail);
    
  }

  public async Task DeleteJailAsync(Jail jail)
  {

    var jails = GetCollection<Jail>("jails");
    var filter = Builders<Jail>.Filter.Where(x => x.Id == jail.Id);
    await jails.DeleteOneAsync(filter);

  }

  public async Task AddAsync<T>(T item, string collection) {
    var items = GetCollection<T>(collection);
    await items.InsertOneAsync(item);
  }

  public async Task DeleteBlacklistAsync(Blacklist blacklist)
  {
    var blacklists = GetCollection<Blacklist>("blacklist");
    var filter = Builders<Blacklist>.Filter.Where(x => x.Id == blacklist.Id);
    await blacklists.DeleteOneAsync(filter);
  }

  public async Task ClearBlacklistAsync(long userSnowflake) {
    var blacklists = GetCollection<Blacklist>("blacklist");
    var filter = Builders<Blacklist>.Filter.Where(x => x.UserSnowflake == userSnowflake);
    await blacklists.DeleteManyAsync(filter);
  }

  public async Task<int> AddToQueue(KaraokeUser user) {
    
    var karaokeCollection = GetCollection<KaraokeUser>("karaoke");
    var queue = await GetQueueAsync((ulong)user.ChannelSnowflake);

    if (queue.Any(x => x.ChannelSnowflake == user.ChannelSnowflake && user.UserSnowflake == x.UserSnowflake)) {
      var usr = queue.First(x => x.ChannelSnowflake == user.ChannelSnowflake && user.UserSnowflake == x.UserSnowflake);
      return usr.Rank;
    }

    user.Rank = queue.Count + 1;
    await karaokeCollection.InsertOneAsync(user);
    return user.Rank;

  }

  public async Task UpdatePremium(string guild, bool premium)
  {
    var collection = GetCollection<GuildInfo>("Guilds");
    var filter = Builders<GuildInfo>.Filter.Where(x => x.GuildSnowflake == guild);
    var upd = Builders<GuildInfo>.Update.Set(x => x.Premium, premium);
    await collection.UpdateOneAsync(filter, upd);
  }

  public async Task UpdateModList(string guild, List<string> mods) {
    var collection = GetCollection<GuildInfo>("Guilds");
    var filter = Builders<GuildInfo>.Filter.Where(x => x.GuildSnowflake == guild);
    var upd = Builders<GuildInfo>.Update.Set(x => x.Moderators, mods);
    await collection.UpdateOneAsync(filter, upd);
  }

  public async Task<List<KaraokeUser>> GetQueueAsync(ulong channel) {

    var karaokeCollection = GetCollection<KaraokeUser>("karaoke");

    var filter = Builders<KaraokeUser>.Filter.Where(x => x.ChannelSnowflake == ((long)channel));

    var collection = await karaokeCollection.FindAsync(filter);

    var items = await collection.ToListAsync();

    var queue = items.OrderBy(x => x.Rank).ToList();

    return queue;

  }

  public async Task PopQueue(ulong channelSnowflake) {
        
    var karaokeCollection = GetCollection<KaraokeUser>("karaoke");

    var filter = Builders<KaraokeUser>.Filter.Where(x => x.ChannelSnowflake == ((long)channelSnowflake) && x.Rank == 1);
    await karaokeCollection.DeleteOneAsync(filter);
    var collectionFilter = Builders<KaraokeUser>.Filter.Where(x => x.ChannelSnowflake == ((long)channelSnowflake));

    var update = Builders<KaraokeUser>.Update.Inc(x => x.Rank, -1);

    await karaokeCollection.UpdateManyAsync(collectionFilter, update);

  }

}
