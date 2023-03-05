using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public abstract class ServerBase : MonoBehaviour
{
    public virtual AddressFamily addressFamily => AddressFamily.InterNetwork;
    public virtual SocketType socketType => SocketType.Stream;
    public virtual ProtocolType protocolType => ProtocolType.Tcp;


    private Socket m_ClientSocket;
    private System.Threading.Thread m_ClientThread;
    private byte[] buffer = new byte[1024];

    private Queue<byte[]> m_QueuedMessages = new Queue<byte[]>();

    public bool IsConnected => m_ClientSocket != null;

    public void Connect(string ip, int port)
    {
        try
        {
            m_ClientSocket = new Socket(addressFamily, socketType, protocolType);
            m_ClientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
        }
        catch (System.Exception e)
        {
            m_ClientSocket = null;
            Debug.LogError(e);
        }

        if (m_ClientSocket != null)
        {
            m_ClientThread = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    int bytesReceived = m_ClientSocket.Receive(buffer);
                    var bytes = new byte[bytesReceived];
                    System.Array.Copy(buffer, bytes, bytesReceived);
                    m_QueuedMessages.Enqueue(bytes);
                }
            });
            m_ClientThread.Start();
        }
    }

    public void Disconnect()
    {
        if (m_ClientSocket != null) m_ClientSocket.Close();
        if (m_ClientThread != null) m_ClientThread.Abort();
    }

    protected void SendPacket(byte[] bytes)
    {
        if (m_ClientSocket == null)
            return;

        m_ClientSocket.Send(bytes);
    }

    protected abstract void OnProcessReceivedPacket(byte[] bytes);


    protected virtual void Update()
    {
        while(m_QueuedMessages.Count > 0)
            OnProcessReceivedPacket(m_QueuedMessages.Dequeue());
    }
}
