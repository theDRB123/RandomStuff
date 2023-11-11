using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("Http://localhost:4000");
var app = builder.Build();
app.UseWebSockets();

List<WebSocket> connections = new();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        string? clientName = context.Request.Query["name"];

        using var ws = await context.WebSockets.AcceptWebSocketAsync();

        connections.Add(ws);

        await BroadCast($"{clientName} joined the room");
        await BroadCast($"{connections.Count} users connected..");
        await ReceiveMessage(ws, async (result, buffer) =>
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await BroadCast(clientName + ":" + message);
            }
        });
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

async Task ReceiveMessage(WebSocket client, Action<WebSocketReceiveResult, byte[]> handleMessage)
{
    var buffer = new byte[1024 * 4];
    while (client.State == WebSocketState.Open)
    {
        var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

        BroadCast(message);

        // BroadCast(message + "Is dead RIP");

    }
}


async Task RemoveDisconnectedClients()
{
    for (int i = connections.Count - 1; i >= 0; i--)
    {
        var client = connections[i];
        if (client.State != WebSocketState.Open)
        {
            connections.RemoveAt(i);
        }
    }
}

async Task BroadCast(string message)
{
    await RemoveDisconnectedClients();
    var bytes = Encoding.UTF8.GetBytes(message);
    foreach (WebSocket client in connections)
    {
        if (client.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await client.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

await app.RunAsync();

