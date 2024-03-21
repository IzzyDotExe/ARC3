

using System.Net;
using System.Net.Sockets;
using System.Text;
using Arc3.Core.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace arc3.Core.Services;

public class SocketCommService : ArcService {
  private readonly DbService _dbService;

  private Socket _socketListner;

  public SocketCommService(DiscordSocketClient clientInstance, InteractionService interactionService,
    DbService dbService)
    : base(clientInstance, interactionService, "SOCKET COMMS") {

      _dbService = dbService;
      IPHostEntry ipHostInfo = Dns.GetHostEntryAsync("127.0.0.1").GetAwaiter().GetResult();
      IPAddress ipAddr = ipHostInfo.AddressList[0];

      IPEndPoint endp = new(ipAddr, 8018);

      _socketListner = new Socket(
        endp.AddressFamily,
        SocketType.Stream,
        ProtocolType.Tcp
      );

      _socketListner.Bind(endp);
      _socketListner.Listen(5);

      Task.Run(AcceptConnections);

    }


  public async Task AcceptConnections() {


    Console.WriteLine("SOCKET SERVER CREATED!");

    while (true) {
      
      Socket clientSocket = await _socketListner.AcceptAsync();

      byte[] temp = new byte[1024];
      int clientBytes = clientSocket.Receive(temp);
      string clientMessage = Encoding.ASCII.GetString(temp, 0, clientBytes);

      Console.WriteLine(clientMessage);

      clientSocket.Shutdown(SocketShutdown.Both);
      clientSocket.Close();

    }

    _socketListner.Close();

  }



}