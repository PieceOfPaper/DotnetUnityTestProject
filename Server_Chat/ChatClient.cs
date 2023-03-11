using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using Newtonsoft.Json.Linq;

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
                ChatMessage chatMessage;
                using (var memStream = new MemoryStream(m_Buffer, 0, m_BytesRead))
                {
                    chatMessage = ChatMessage.Parser.ParseFrom(memStream);
                    chatMessage.ErrorCode = 0;
                    chatMessage.Timestamp = ((DateTimeOffset)System.DateTime.UtcNow).ToUnixTimeSeconds();

                    ChatClientManager.Instance.BrodcastMessage(this, chatMessage);
                }
                Console.WriteLine($"Received Message - ErrorCode:{chatMessage.ErrorCode}, Type:{chatMessage.Type}. TimeStamp:{chatMessage.Timestamp}, Nickname:{chatMessage.Nickname}, Message:{chatMessage.Message}");
            }
            else
            {
                Console.WriteLine($"Disconnect {Handle}");
                m_NetworkStream.Close();
                ChatClientManager.Instance.RemoveChatClient(Handle);
                TcpClientManager.Instance.RemoveTcpClient(Handle);
                return;
            }

            Read();
        }

        public void SendMessage(ChatMessage chatMessage)
        {
            var response = chatMessage.ToByteArray();
            m_NetworkStream.Write(response, 0, response.Length);
        }
    }
}