using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("Http://localhost:4000");
var app = builder.Build();
app.UseWebSockets();

// List<WebSocket> connections = new();
List<(string, WebSocket)> connections = new() { };

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        string? clientName = context.Request.Query["name"];

        using var ws = await context.WebSockets.AcceptWebSocketAsync();

        connections.Add((clientName, ws));

        await BroadCast($"{clientName} joined the room");
        await BroadCast($"{connections.Count} users are connected..");
        //this is the recieve message task running for the current client
        await ReceiveMessage(ws , clientName);
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

async Task ReceiveMessage(WebSocket client , string clientName)
{
    var buffer = new byte[1024 * 4];
    while (client.State == WebSocketState.Open)
    {
        ;
        try
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (new Regex("^dm").IsMatch(message))
            {
                await DirectMessage(message);
            }
            else if (new Regex("^bd").IsMatch(message))
            {
                //broadcast
                await BroadCast( clientName + ":" + message);
            }
            else if (new Regex("^ud").IsMatch(message))
            {
                //user disconnected
            }
            
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine("client disconnected");
            RemoveDisconnectedClients();
            break;
        }


    }
}


async Task RemoveDisconnectedClients()
{
    for (int i = connections.Count - 1; i >= 0; i--)
    {
        var client = connections[i];
        if (client.Item2.State != WebSocketState.Open)
        {
            Console.WriteLine("removing client");
            await BroadCast($"{client.Item1} left the room");
            connections.RemoveAt(i);
        }
    }
}

async Task DirectMessage(string message)
{
    string[] parts = message.Split("||");
    string sender = parts[1];
    string receiver = parts[2];
    string body = parts[3];
    string time = parts[4];
    string msg = "dm sent from user" + sender + " :: " + body + " at " + time;

    foreach ((string, WebSocket) client in connections)
    {
        if (client.Item1 == receiver)
        {
            //send the message
            var arraySegment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg), 0, Encoding.UTF8.GetBytes(msg).Length);
            await client.Item2.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("dm sent to user :- " + receiver + "from user:- " + sender);
        }
    }
}

async Task BroadCast(string message)
{
    var bytes = Encoding.UTF8.GetBytes(message);
    foreach ((string, WebSocket) client in connections)
    {
        if (client.Item2.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            try
            {
                await client.Item2.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("client disconnected");
                RemoveDisconnectedClients();
                break;
            }
        }
    }
}

await app.RunAsync();
