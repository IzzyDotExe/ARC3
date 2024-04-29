﻿
using dotenv.net;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using System.Reflection;
using Arc3.Core.Services;
using arc3.Core.Services;
using System.Diagnostics;

namespace Arc3;

internal class Arc3
{
  public static string ArcVersion = "3.2";
    
  private DiscordSocketClient? _client;
    
  private IServiceProvider? _serviceProvider;

  private InteractionService? _interactions;

  public static Task Main(string[] args) => new Arc3().MainAsync();

  public async Task MainAsync() {

    // Load the .env file from the root directory change the path if needed
    var envFilePath = Path.GetFullPath("../.env");
    Console.WriteLine($".env file path:{envFilePath}"); // Debugging

    var envOptions = new DotEnvOptions(envFilePaths: new[] { envFilePath });
    DotEnv.Load(envOptions);

    var config = new DiscordSocketConfig {
      GatewayIntents = (Discord.GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent),
      LogLevel = Discord.LogSeverity.Debug
    };

    // Create a new instance of the socket client
    _client = new DiscordSocketClient(config);

    // Create our service provider
    _serviceProvider = new ServiceCollection()
      .AddSingleton<InteractionService>()
      .AddSingleton<DiscordSocketClient>(_client)
      .AddSingleton<DbService>()
      .AddSingleton<KaraokeService>()
      .AddSingleton<PaginationService>()
      .AddSingleton<JailService>()
      .AddSingleton<UptimeService>()
      .AddSingleton<ModMailService>()
      // .AddSingleton<SocketCommService>()
      .BuildServiceProvider();

    // Instantiate your services
    _interactions = _serviceProvider.GetRequiredService<InteractionService>();
    // TODO: Figure out Socket comms
    // var socketComms = _serviceProvider.GetRequiredService<SocketCommService>();
    var dbService = _serviceProvider.GetRequiredService<DbService>();
    var modmailService = _serviceProvider.GetRequiredService<ModMailService>();
    var jailService = _serviceProvider.GetRequiredService<JailService>();
    var karaokeService = _serviceProvider.GetRequiredService<KaraokeService>();
      
    _client.InteractionCreated += async interaction => 
    {
      var ctx = new SocketInteractionContext(_client, interaction);
      await _interactions.ExecuteCommandAsync(ctx, services: _serviceProvider);
    };

    var debug = Environment.GetEnvironmentVariable("DEBUG");

    if (debug == "true") {
      _client.Log += Log;
      _interactions.Log += Log;
    }

    _client.Ready += ReadyAsync;

    // Get the token from our environment.
    var token = Environment.GetEnvironmentVariable("TOKEN");


    // Login and start the bot
    await _client.LoginAsync(Discord.TokenType.Bot, token);
    await _client.StartAsync();

    // Block this task until the program is closed!
    await Task.Delay(-1);

  }

  private Task Log(LogMessage message)
  {
    Console.WriteLine($"{DateTime.Now} [{message.Severity}] {message.Source}: {message.Message}");
    if (message.Exception is not null) // Check if there is an exception
    {
      // Log the full exception, including the stack trace
      Console.WriteLine($"Exception: {message.Exception.ToString()}");
    }
    return Task.CompletedTask;
  }

  private async Task ReadyAsync()
  {

    if (_client == null)
      throw new Exception("Client is not initialized");

    if (_interactions == null)
      throw new Exception("Interaction service is not initialized");

    try
    { 
      // Things to be run when the bot is ready
      if (_client.Guilds.Any())
      {
        // Register command modules with the InteractionService.
        // Tels it to scan the whole assembly for classes that define slash commands.
        // Also pass in the service provider so services can be accessed in modules
        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        // Get the ID of the first guild the bot is a member of
        // Then register the commands to that guild
        var guildId = ulong.Parse(Environment.GetEnvironmentVariable("GUILD_ID")!);
        await _interactions.RegisterCommandsToGuildAsync(guildId, true);
        
      }
      else
      {
        Console.WriteLine($"\nNo guilds found\n");
      }

      Console.WriteLine($"\nLogged in as {_client.CurrentUser.Username}\n" +
                        $"Registered {_interactions.SlashCommands.Count} slash commands\n" +
                        $"Bot is a member of {_client.Guilds.Count} guilds\n");

      await _client.SetGameAsync("True Blue", null, ActivityType.Listening);
    }
    catch (Exception e)
    {
      // Log the exception
      Console.WriteLine($"Exception: {e}");
      throw;
    }
  }

}