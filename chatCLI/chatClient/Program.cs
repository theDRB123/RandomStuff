using System;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

public class Program
{
    public static async Task Main(string[] args)
    {
        int[] array = new int[5];
        Console.WriteLine(array.Length);

        ClientWebSocket ws = new();

        string? name;

        TextUtil.Write("Enter your userName :", 1);
        TextUtil.changeTheme(3);
        name = Console.ReadLine();
        TextUtil.changeTheme(0);

        TextUtil.WriteLine("Connecting to the server ...", 2);
        await ws.ConnectAsync(new Uri($"ws://10.42.0.1:4000/ws?name={name}"), CancellationToken.None);
        TextUtil.WriteLine("Connected...", 5);

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
                    TextUtil.Write(parts[0], 3);
                }
                else
                {
                    TextUtil.Write(parts[0], 5);
                }
                TextUtil.Write("::", 6);
                TextUtil.Write(parts[1], 2);
                TextUtil.Write(" at ", 7);
                TextUtil.Write(parts[2], 0);
                TextUtil.WriteLine(" ", 0);
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
                    TextUtil.ClearCurrentConsoleLine();
                    TextUtil.Write("bd ", 6);
                    TextUtil.WriteLine(message, 2);
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
    }
}
