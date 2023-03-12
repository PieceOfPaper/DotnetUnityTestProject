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
            var packetType = (PacketType)PacketSerializer.DeserializeType(bytes);
            switch (packetType)
            {
                case PacketType.SC_Chat_Message:
                    {
                        var data = PacketSerializer.DeserializeWithoutType<ChatMessage>(bytes);
                        if (data.ErrorCode != ErrorCode.Success)
                        {
                            UnityEngine.Debug.LogError("ChatMessage Error " + data.ErrorCode);
                            return;
                        }
                        
                        OnReceiveMessage?.Invoke(data);
                    }
                    break;
                case PacketType.SC_Change_Channel:
                    {
                        var errorCode = PacketSerializer.DeserializeErrorCode(bytes);
                        UnityEngine.Debug.Log("ChangeChannel " + errorCode);
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
            SendPacket(PacketSerializer.SerializeWithType((ushort)PacketType.CS_Chat_Message, chatMessage));
        }

        public void SendChangeChannel(int type, int channelId)
        {
            var data = new Change_Channel();
            data.Type = type;
            data.ChannelId = channelId;
            SendPacket(PacketSerializer.SerializeWithType((ushort)PacketType.CS_Change_Channel, data));
        }
    }
}
