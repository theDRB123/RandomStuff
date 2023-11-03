using System.Net;
using System.Net.Sockets;
using System.Text;




class Program
{
    static List<Socket> clients = new List<Socket>();

    static async Task Main(string[] args)
    {

        IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, 5000);


        using Socket server = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(iPEndPoint);
        server.Listen(100);

        Console.WriteLine("Server listening on port 5000....");

        while (true)
        {
            Console.WriteLine("Waiting for connection...");
            Socket client = await server.AcceptAsync();
            Console.WriteLine("Client connected...");
            clients.Add(client);
            Task.Run(async () => await HandleClientAsync(client));
        }
    }

    static async Task HandleClientAsync(Socket client)
    {
        using (client)
        {
            Console.WriteLine("Connected..");

            while (true)
            {
                //recieve message
                var buffer = new byte[1_024];
                var recieved = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, recieved);
                Console.WriteLine("The response is :-" + response);
                if(response[0] == '!'){
                    await BroadcastMessage(response);
                    Console.WriteLine("Broadcasted");
                    continue;
                }
                buffer = Encoding.UTF8.GetBytes(response.ToUpper(), 0, recieved);
                await client.SendAsync(buffer, SocketFlags.None);
            }
        }
    }

    static async Task BroadcastMessage(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        foreach (var client in clients)
        {
            await client.SendAsync(messageBytes, SocketFlags.None);
        }
    }
}
