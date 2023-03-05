using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Server_Chat : MonoBehaviour
{
    private Socket clientSocket;
    private System.Threading.Thread clientThread;
    private byte[] buffer = new byte[1024];

    private void Start()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345)); // Replace with your server IP address and port number

        clientThread = new System.Threading.Thread(ReceiveChatMessages);
        clientThread.Start();
    }

    private void ReceiveChatMessages()
    {
        while (true)
        {
            int bytesReceived = clientSocket.Receive(buffer);
            string message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesReceived);
            Debug.Log("Received: " + message);
        }
    }

    public void SendChatMessage(string message)
    {
        byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
        clientSocket.Send(messageBytes);
    }

    private void OnDestroy()
    {
        clientSocket.Close();
        clientThread.Abort();
    }

    [ContextMenu("Test1")]
    public void Test() => SendChatMessage("Test1");
}
