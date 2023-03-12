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
            var port = 10002;
            var endpoint = new IPEndPoint(ip, port);

            var listener = new TcpListener(endpoint);
            listener.Start();

            Console.WriteLine($"Server listening on {endpoint}");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine($"Client connected from {client.Client.RemoteEndPoint} {client.Client.Handle}");
                var tcpClientHandle = TcpClientManager.Instance.AddTcpClient(client);

                var stream = client.GetStream();
                var chatClient = new ChatClient(tcpClientHandle, stream);
                ChatClientManager.Instance.AddChatClient(chatClient);

                chatClient.Read();
            }
        }
    }
}
