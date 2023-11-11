using System;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;


ClientWebSocket ws = new();

Dictionary<int, (ConsoleColor, ConsoleColor)> theme = new(){
    {0 , (ConsoleColor.Gray , ConsoleColor.Black)},
    {1 , (ConsoleColor.Black , ConsoleColor.DarkBlue)},
    {2 , (ConsoleColor.White , ConsoleColor.Black)},
    {3 , (ConsoleColor.Red , ConsoleColor.Black)},
    {4 , (ConsoleColor.Black , ConsoleColor.DarkRed)}
};

void changeTheme(int key)
{
    Console.ForegroundColor = theme[key].Item1;
    Console.BackgroundColor = theme[key].Item2;
}

string name;

changeTheme(1);
Console.Write("Enter your userName :");
changeTheme(4);
name = Console.ReadLine();
changeTheme(0);

Console.WriteLine("Connecting to the server ...");
changeTheme(0);
await ws.ConnectAsync(new Uri($"ws://localhost:4000/ws?name={name}"), CancellationToken.None);
Console.WriteLine("Connected...");

static void ClearCurrentConsoleLine()
{
    int currentLineCursor = Console.CursorTop;
    Console.SetCursorPosition(0, Console.CursorTop - 1);
    Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
    Console.SetCursorPosition(0, currentLineCursor - 1);
}

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

        Console.WriteLine(message);
    }
});

Task sendTask = Task.Run(async () =>
{
    while (true)
    {
        string? message;
        message = Console.ReadLine();
        if (message == "exit")
        {
            message = "ud" + "/" + name + "/" + "" + "/" + "user is ded" + "/";
            break;
        }
        if (Regex.IsMatch(message, "^dm"))
        {
            //input format = dm || receiver || message
            string[] parts = message.Split(' ');
            if (parts.Length < 3)
            {
                Console.WriteLine("Invalid format");
                continue;
            }

            message = "dm" + "||" + name + "||" + parts[1] + "||" + parts[2] + "||" + DateTime.Now.ToString("hh:mm tt");
        }
        else if (Regex.IsMatch("^bd", message))
        {
            string pattern = "\"(.*?)\"";
            MatchCollection matches = Regex.Matches(message, pattern);
            if (matches.Count == 0)
            {
                Console.WriteLine("Invalid format");
                continue;
            }
            string msg = matches[0].Groups[0].Value;
            message = "bd" + "||" + name + "||" + msg + "||" + DateTime.Now.ToString();
        }

        var bytes = Encoding.UTF8.GetBytes(message);
        try
        {
            await ws.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);

        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
});

await Task.WhenAll(receiveTask, sendTask);

if (ws.State != WebSocketState.Closed)
{
    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
}

await Task.WhenAll(receiveTask, sendTask);
