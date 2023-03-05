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
                Console.WriteLine($"Client connected from {client.Client.RemoteEndPoint} {client.Client.Handle}");
                var tcpClientHandle = TcpClientManager.Instance.AddTcpClient(client);

                var stream = client.GetStream();
                var chatClient = new ChatClient(tcpClientHandle, stream);
                ChatClientManager.Instance.AddChatClient(chatClient);

                chatClient.Read();

                // var buffer = new byte[1024];
                // int bytesRead = 0;
                // while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                // {
                //     var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                //     Console.WriteLine($"Received message: {message}");

                //     var response = Encoding.UTF8.GetBytes($"Echo: {message}");
                //     stream.Write(response, 0, response.Length);
                // }

                // // bytesRead가 0보다 작거나 같은 경우, 연결이 끊어졌음을 의미
                // if (bytesRead <= 0)
                // {
                //     Console.WriteLine($"Connection closed by remote host {client.Client.RemoteEndPoint} {client.Client.Handle}");
                // }

                // client.Close();
            }
        }
    }
}
