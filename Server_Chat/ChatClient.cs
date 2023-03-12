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

        //key:type, value:channelId, channelId가 0이면 미사용.
        private Dictionary<int, int> m_DicChannels = new Dictionary<int, int>();

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
                var packetType = (PacketType)PacketSerializer.DeserializeType(m_Buffer);
                switch(packetType)
                {
                    case PacketType.CS_Chat_Message:
                        {
                            var chatMessage = PacketSerializer.DeserializeWithoutType<ChatMessage>(m_Buffer, m_BytesRead);
                            chatMessage.Timestamp = ((DateTimeOffset)System.DateTime.UtcNow).ToUnixTimeSeconds();
                            Console.WriteLine($"[ChatClient] CS_Chat_Message - Type:{chatMessage.Type}. TimeStamp:{chatMessage.Timestamp}, Nickname:{chatMessage.Nickname}, Message:{chatMessage.Message}");

                            if (m_DicChannels.ContainsKey(chatMessage.Type) == false || m_DicChannels[chatMessage.Type] == 0)
                            {
                                chatMessage.ErrorCode = ErrorCode.ErrorChatMessageInvalidChannelId;
                                SendChatMessage(chatMessage);
                            }
                            else
                            {
                                chatMessage.ErrorCode = ErrorCode.Success;
                                if (ChatClientManager.Instance.BrodcastMessage(this, chatMessage) == false)
                                {
                                    chatMessage.ErrorCode = ErrorCode.ErrorChatMessageFail;
                                    SendChatMessage(chatMessage);
                                }
                            }
                        }
                        break;
                    case PacketType.CS_Change_Channel:
                        {
                            var data = PacketSerializer.DeserializeWithoutType<Change_Channel>(m_Buffer, m_BytesRead);
                            Console.WriteLine($"[ChatClient] CS_Change_Channel - Type:{data.Type}. ChannelId:{data.ChannelId}");

                            if (m_DicChannels.ContainsKey(data.Type) == false)
                                m_DicChannels.Add(data.Type, 0);

                            if (m_DicChannels[data.Type] == data.ChannelId)
                            {
                                SendTypeErrorCode((ushort)PacketType.SC_Change_Channel, ErrorCode.ErrorChangeChannelAlreadyChanged);
                            }
                            else
                            {
                                ChatClientManager.Instance.ChangeChannel(this, data.Type, m_DicChannels[data.Type], data.ChannelId);
                                m_DicChannels[data.Type] = data.ChannelId;
                                SendTypeErrorCode((ushort)PacketType.SC_Change_Channel, ErrorCode.Success);
                            }
                        }
                        break;
                    default:
                        //TODO - Error
                        break;
                }
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

        public int GetChannelId(int type) => m_DicChannels.ContainsKey(type) ? m_DicChannels[type] : 0;

        public void SendChatMessage(ChatMessage chatMessage)
        {
            var response = PacketSerializer.SerializeWithType((ushort)PacketType.SC_Chat_Message, chatMessage);
            m_NetworkStream.Write(response, 0, response.Length);
            Console.WriteLine($"[ChatClient] SC_Chat_Message - ErrorCode:{chatMessage.ErrorCode}, Type:{chatMessage.Type}");
        }

        public void SendTypeErrorCode(ushort type, ErrorCode errorCode)
        {
            var response = PacketSerializer.SerializeWithTypeErrorCode(type, errorCode);
            m_NetworkStream.Write(response, 0, response.Length);
            Console.WriteLine($"[ChatClient] {((PacketType)type)} - ErrorCode:{errorCode}");
        }
    }
}