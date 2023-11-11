# BASIC CONNECTION UPGRADE

```csharp
using System.Net.Sockets;
using System.Net;
using System;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;

class Server
{
    public static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 4000);

        server.Start();

        Console.WriteLine("Server has started on 127.0.0.1 on port 4000...");

        //note that same thing can be acompolished as follows
        // Socket server = new Socket(/*Address family*/ , SocketType.Stream , ProtocolType.Tcp);
        // server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1 ,. 4000")));
        // server.Listen(100);

        TcpClient client = server.AcceptTcpClient();

        Console.WriteLine("A client has connected");

        NetworkStream stream = client.GetStream();

        while (true)
        {
            while (!stream.DataAvailable) ;
            byte[] bytes = new byte[client.Available];

            while (client.Available < 3) ;

            stream.Read(bytes, 0, client.Available);

            string data = Encoding.UTF8.GetString(bytes, 0, client.Available);


            //in the server we are first catching a get request from the client (this will be a 0normal http get request)
            if ( new Regex("^GET").IsMatch(data))
            {
                const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

                byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                    + "Connection: Upgrade" + eol
                    + "Upgrade: websocket" + eol
                    + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                        System.Security.Cryptography.SHA1.Create().ComputeHash(
                            Encoding.UTF8.GetBytes(
                                new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                            )
                        )
                    ) + eol
                    + eol);

                // byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101" + eol
                // + "Connection: Upgrade" + eol
                // + "Upgrade: websocket" + eol
                // + "Sec-WebSocket-Accept: " + );

                
                stream.Write(response, 0, response.Length);
            }


        }

    }
}
```
