
using Discord.Interactions;
using Discord.WebSocket;

namespace arc3 {

  class arc3 {

    private DiscordSocketClient? _client;
    private InteractionService? _interactions;

    public static Task Main(string[] args) => new arc3().MainAsync();

    public async Task MainAsync() {

      var config = new DiscordSocketConfig {
        GatewayIntents = Discord.GatewayIntents.All,
        LogLevel = Discord.LogSeverity.Debug
      };

      _client = new DiscordSocketClient(config);

      _interactions = new InteractionService(_client);

      _client.InteractionCreated += async interaction => {
        var ctx = new SocketInteractionContext(_client, interaction);
        await _interactions.ExecuteCommandAsync(ctx, services: null);
      };

      var token = Environment.GetEnvironmentVariable("TOKEN");

      await _client.LoginAsync(Discord.TokenType.Bot, token);
      await _client.StartAsync();

      // Block this task until the program is closed!
      await Task.Delay(-1);

    }

  }

}