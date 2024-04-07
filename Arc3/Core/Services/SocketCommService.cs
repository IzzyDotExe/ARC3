

using System.Net;
using System.Net.Sockets;
using System.Text;
using Arc3.Core.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace arc3.Core.Services;

public class SocketCommService : ArcService {
  private readonly DbService _dbService;

  private TcpListener _serverListener;

  public SocketCommService(DiscordSocketClient clientInstance, InteractionService interactionService,
    DbService dbService)
    : base(clientInstance, interactionService, "SOCKET COMMS") {

      _dbService = dbService;
      IPHostEntry ipHostInfo = Dns.GetHostEntryAsync("127.0.0.1").GetAwaiter().GetResult();
      IPAddress ipAddr = ipHostInfo.AddressList[0];

      IPEndPoint endp = new(ipAddr, 8018);

      _serverListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8018);

      _serverListener.Start();

      Task.Run(AcceptConnections);

    }

  private async Task AcceptConnections() {

    while (true) {
      
      TcpClient client = await _serverListener.AcceptTcpClientAsync();
      NetworkStream stream = client.GetStream();

      while (!stream.DataAvailable);

      byte[] clientBytes = new byte[client.Available];
      await stream.ReadAsync(clientBytes, 0, clientBytes.Length);
      
      string clientMessage = Encoding.ASCII.GetString(clientBytes);
      
      Console.WriteLine(clientMessage);
      Console.WriteLine("end client message");
      
      client.Dispose();
    }

    _serverListener.Dispose();;

  }



}