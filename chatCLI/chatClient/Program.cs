using System;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;


int[] array = new int[5];
Console.WriteLine(array.Length);


ClientWebSocket ws = new();

Dictionary<int, (ConsoleColor, ConsoleColor)> theme = new(){
    {0 , (ConsoleColor.Gray , ConsoleColor.Black)},
    {1 , (ConsoleColor.Black , ConsoleColor.DarkBlue)},
    {2 , (ConsoleColor.White , ConsoleColor.Black)},
    {3 , (ConsoleColor.Red , ConsoleColor.Black)},
    {4 , (ConsoleColor.Black , ConsoleColor.DarkRed)},
    {5 , (ConsoleColor.Green , ConsoleColor.Black)},
    {6 , (ConsoleColor.Cyan , ConsoleColor.Black) },
    {7 , (ConsoleColor.Yellow , ConsoleColor.Black)}
};

void changeTheme(int key)
{
    Console.ForegroundColor = theme[key].Item1;
    Console.BackgroundColor = theme[key].Item2;
}

string? name;

changeTheme(1);
Console.Write("Enter your userName :");
changeTheme(3);
name = Console.ReadLine();
changeTheme(0);


changeTheme(2);
Console.WriteLine("Connecting to the server ...");
changeTheme(5);
await ws.ConnectAsync(new Uri($"ws://localhost:4000/ws?name={name}"), CancellationToken.None);

Console.WriteLine("Connected...");
changeTheme(0);
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

        //message format = sender :: message at time
        string[] parts = message.Split("::");

        if (parts[0] == "server")
        {
            changeTheme(3);
            Console.Write(parts[0]);
            changeTheme(0);
        }
        else
        {
            changeTheme(5);
            Console.Write(parts[0]);
            changeTheme(0);
        }
        changeTheme(6);
        Console.Write("::");
        changeTheme(0);
        Console.Write(parts[1]);
        changeTheme(7);
        Console.Write(" at ");
        changeTheme(2);
        Console.Write(parts[2]);
        changeTheme(0);
        Console.WriteLine(" ");
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
            message = message.Replace("dm ", "");
            string[] parts = message.Split(" ");
            var receiver = parts[0];
            var msg = message.Replace(receiver, "");
            message = "dm" + "||" + name + "||" + receiver + "||" + msg + "||" + DateTime.Now.ToString("hh:mm tt");

        }
        else if (Regex.IsMatch("^bd", message))
        {
            //input format = bd  message
            string[] parts = message.Split('_');
            string msg = parts[1];
            message = "bd" + "||" + name + "||" + "all" + "||" + msg + "||" + DateTime.Now.ToString("hh:mm tt");

        }
        else
        {
            ClearCurrentConsoleLine();
            Console.WriteLine("bd " + message);
            message = "bd" + "||" + name + "||" + "all" + "||" + message + "||" + DateTime.Now.ToString("hh:mm tt");
        }

        var bytes = Encoding.UTF8.GetBytes(message);
        try
        {
            await ws.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);

        }
        catch (Exception e)
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
