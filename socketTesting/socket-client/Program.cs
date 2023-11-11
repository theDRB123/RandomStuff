using System.Net;
using System.Net.Sockets;
using System.Text;


class Program
{
    static async Task Main(string[] args)
    {
        
        IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint iPEndPoint = new(ipAddress, 5000);

        using Socket client = new(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await client.ConnectAsync(iPEndPoint);
        Console.WriteLine("connected...");

        // Task.Run(async () => await HandleSendData(client));
        Task.Run(async () => await HandleGetData(client));

        while (true)
        {
            Console.WriteLine("Enter the message :-");
            var message = Console.ReadLine();
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);
        }

    }


    static async Task HandleGetData(Socket client)
    {
        while (true)
        {
            var buffer = new Byte[1_024];
            var receive = await client.ReceiveAsync(buffer, SocketFlags.None);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, receive);
            Console.WriteLine("Message Recieved :-" + receivedMessage);
        }
    }
}


