using System;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;


ClientWebSocket ws = new();

string name;
Console.WriteLine("Enter your userName :");
name = Console.ReadLine();

Console.WriteLine("Connecting to the server ...");
await ws.ConnectAsync(new Uri($"ws://localhost:4000/ws?name={name}"), CancellationToken.None);
Console.WriteLine("Connected...");


Task receiveTask = Task.Run(async () =>
{
    var buffer = new byte[1024];
    while (true)
    {
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            break;
        }
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);//result holds the length of the returned buffer
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Received " + message);
    }
});

Task sendTask = Task.Run(async () =>
{
    while (true)
    {
        string message;
        message = Console.ReadLine();
        if (message == "exit") {
            break;
        }
        message = name + " : " + message;
        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
    }
});

await Task.WhenAll(receiveTask, sendTask);

if (ws.State != WebSocketState.Closed){
    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
}

await Task.WhenAll(receiveTask, sendTask);
