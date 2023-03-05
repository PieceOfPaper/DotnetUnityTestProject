using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server_Chat : ServerBase
{
    public System.Action<string> OnReceiveMessage;

    protected override void OnProcessReceivedPacket(byte[] bytes)
    {
        string message = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        OnReceiveMessage?.Invoke(message);
    }

    public void SendChatMessage(string message)
    {
        byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
        SendPacket(messageBytes);
    }
}
