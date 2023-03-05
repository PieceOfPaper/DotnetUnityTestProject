using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting chat server...");

            var ip = IPAddress.Any;
            var port = 12345;
            var endpoint = new IPEndPoint(ip, port);

            var listener = new TcpListener(endpoint);
            listener.Start();

            Console.WriteLine($"Server listening on {endpoint}");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine($"Client connected from {client.Client.RemoteEndPoint}");

                var stream = client.GetStream();

                while (client.Connected)
                {
                    var buffer = new byte[1024];
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine($"Received message: {message}");

                    var response = Encoding.UTF8.GetBytes($"Echo: {message}");
                    stream.Write(response, 0, response.Length);
                }

                client.Close();
            }
        }
    }
}
