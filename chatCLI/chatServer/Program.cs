using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
// builder.WebHost.UseUrls("Http://localhost:4000");
builder.WebHost.UseUrls("Http://*:4000");
var app = builder.Build();
app.UseWebSockets();

List<(string,IPAddress, WebSocket)> connections = new();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        string? clientName = context.Request.Query["name"];
        IPAddress? ip = context.Connection.RemoteIpAddress;
        var client = connections.Find(x => x.Item1 == clientName);
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
      
        connections.Add((clientName,ip,ws));
        

        await BroadCast($"bg||server||'all'||{clientName + ip.ToString()} joined the room||{DateTime.Now.ToString("h:mm:ss tt")}");
        await BroadCast($"bg||server||'all'||{connections.Count} users are connected..||{DateTime.Now.ToString("h:mm:ss tt")}");
        await ReceiveMessage(ws,ip);
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

async Task ReceiveMessage(WebSocket client, IPAddress ip)
{
    var buffer = new byte[1024 * 4];
    while (client.State == WebSocketState.Open)
    {
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
                await BroadCast(message);
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
        if (client.Item3.State != WebSocketState.Open)
        {
            Console.WriteLine("removing client");
            await BroadCast($"bg||server||'all'||'{client.Item1} left the room'||{DateTime.Now.ToString("h:mm:ss tt")}");
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
    string msg = "dm sent from " + sender + "::" + body + "::" + time;

    foreach ((string,IPAddress,WebSocket) client in connections)
    {
        if (client.Item1 == receiver)
        {
            //send the message
            var arraySegment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg), 0, Encoding.UTF8.GetBytes(msg).Length);
            await client.Item3.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("dm sent to user :- " + receiver + "from user:- " + sender);
        }
    }
}

async Task BroadCast(string message)
{
    //format bd||sender||receiver||message||datetime 
    string[] parts = message.Split("||");
    string sender = parts[1];
    string receiver = parts[2];
    string body = parts[3];
    string time = parts[4];
    // string msg = "dm sent from user" + sender + " :: " + body + " at " + time;
    string msg = sender + "::" + body + "::" + time;
    var bytes = Encoding.UTF8.GetBytes(msg);
    foreach ((string,IPAddress, WebSocket) client in connections)
    {
        if (client.Item3.State == WebSocketState.Open & !(client.Item1 == sender))
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            try
            {
                await client.Item3.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("client disconnected");
                await RemoveDisconnectedClients();
                break;
            }
        }
    }
}

await app.RunAsync();

