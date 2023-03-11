using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server_Chat : ServerBase
{
    public System.Action<ChatPacket> OnReceiveMessage;

    [System.Serializable]
    public struct ChatPacket
    {
        [System.NonSerialized] public bool IsValid;

        public string nickname;
        public string message;
    }

    protected override void OnProcessReceivedPacket(byte[] bytes)
    {
        string json = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        ChatPacket packet = default;
        if (string.IsNullOrWhiteSpace(json) == false)
        {
            packet = JsonUtility.FromJson<ChatPacket>(json);
            packet.IsValid = true;
        }
        OnReceiveMessage?.Invoke(packet);
    }

    public void SendChatMessage(ChatPacket packet)
    {
        var json = JsonUtility.ToJson(packet);
        byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(json);
        SendPacket(messageBytes);
    }
}
