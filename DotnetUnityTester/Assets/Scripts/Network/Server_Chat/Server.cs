using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

namespace Server_Chat
{
    public class Server : ServerBase
    {
        public System.Action<ChatMessage> OnReceiveMessage;

        protected override void OnProcessReceivedPacket(byte[] bytes)
        {
            var protocolType = (PacketType)ProtobufExtension.DeserializeType(bytes);
            switch(protocolType)
            {
                case PacketType.SC_Chat_Message:
                    {
                        OnReceiveMessage?.Invoke(ProtobufExtension.DeserializeWithoutType<ChatMessage>(bytes));
                    }
                    break;
            }
        }

        public void SendChatMessage(int type, string nickname, string message, string otherJsonData = null)
        {
            var chatMessage = new ChatMessage();
            chatMessage.Type = type;
            chatMessage.Nickname = nickname;
            chatMessage.Message = message;
            chatMessage.OtherJsonData = otherJsonData == null ? string.Empty : otherJsonData;
            SendPacket(ProtobufExtension.SerializeWithType((ushort)PacketType.CS_Chat_Message, chatMessage));
        }
    }
}
