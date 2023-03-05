using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_Chat
{
    public class ChatClient
    {
        public long Handle { get; private set; }
        private NetworkStream m_NetworkStream;

        private byte[] m_Buffer = new byte[1024];
        private int m_BytesRead = 0;

        public ChatClient(long handle, NetworkStream networkStream)
        {
            Handle = handle;
            m_NetworkStream = networkStream;
            Console.WriteLine($"Create ChatClient {handle}");
        }

        public async void Read()
        {
            m_BytesRead = await m_NetworkStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
            if (m_BytesRead > 0)
            {
                var message = Encoding.UTF8.GetString(m_Buffer, 0, m_BytesRead);
                Console.WriteLine($"Received message: {message}");

                // SendMessage(message);
                ChatClientManager.Instance.BrodcastMessage(message);
            }
            else
            {
                m_NetworkStream.Close();
                ChatClientManager.Instance.RemoveChatClient(Handle);
                TcpClientManager.Instance.RemoveTcpClient(Handle);
                return;
            }

            Read();
        }

        public void SendMessage(string message)
        {
            var response = Encoding.UTF8.GetBytes(message);
            m_NetworkStream.Write(response, 0, response.Length);
        }
    }
}