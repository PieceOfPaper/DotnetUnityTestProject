using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class Server_Chat : ServerBase
{
    public System.Action<ChatMessage> OnReceiveMessage;

    protected override void OnProcessReceivedPacket(byte[] bytes)
    {
        ChatMessage chatMessage;
        using (var memStream = new System.IO.MemoryStream(bytes))
        {
            chatMessage = ChatMessage.Parser.ParseFrom(memStream);
        }
        OnReceiveMessage?.Invoke(chatMessage);
    }

    public void SendChatMessage(int type, string nickname, string message, string otherJsonData = null)
    {
        var chatMessage = new ChatMessage();
        chatMessage.Type = type;
        chatMessage.Nickname = nickname;
        chatMessage.Message = message;
        chatMessage.OtherJsonData = otherJsonData == null ? string.Empty : otherJsonData;
        SendPacket(chatMessage.ToByteArray());
    }
}
