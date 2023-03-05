using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public abstract class ServerBase : MonoBehaviour
{
    public virtual AddressFamily AddressFamily => AddressFamily.InterNetwork;
    public virtual SocketType SocketType => SocketType.Stream;
    public virtual ProtocolType ProtocolType => ProtocolType.Tcp;

    public virtual int ReconnectCount => 5;
    public virtual float ReconnectInterval => 1.0f;

    private Socket m_ClientSocket;
    private System.Threading.Thread m_ClientThread;
    private byte[] m_Buffer = new byte[1024];
    private bool m_IsConnecting = false;
    private string m_IP;
    private int m_Port;
    private int m_CurrentReconnectCount = -1;
    private float m_LastTryConnectTime = 0f;

    private Queue<byte[]> m_QueuedMessages = new Queue<byte[]>();

    public bool IsConnected => m_ClientSocket != null && m_ClientSocket.Connected;
    public bool IsConnecting => m_IsConnecting;
    public bool IsReconnecting => m_CurrentReconnectCount >= 0;

    public void Connect(string ip, int port)
    {
        m_IP = ip;
        m_Port = port;
        m_LastTryConnectTime = Time.realtimeSinceStartup;

        m_ClientSocket = new Socket(AddressFamily, SocketType, ProtocolType);

        m_IsConnecting = true;
        m_ClientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), (ar) =>
        {
            m_IsConnecting = false;

            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                m_CurrentReconnectCount = -1;
            }
            catch (SocketException e)
            {
                m_ClientSocket = null;
                if (m_CurrentReconnectCount == -1) m_CurrentReconnectCount = 1;
                Debug.LogError(e);
            }

            if (m_ClientSocket != null)
            {
                m_ClientThread = new System.Threading.Thread(() =>
                {
                    while (true)
                    {
                        int bytesReceived = m_ClientSocket.Receive(m_Buffer);
                        var bytes = new byte[bytesReceived];
                        System.Array.Copy(m_Buffer, bytes, bytesReceived);
                        m_QueuedMessages.Enqueue(bytes);
                    }
                });
                m_ClientThread.Start();
            }
        }, m_ClientSocket);
    }

    public void Disconnect()
    {
        if (m_ClientSocket != null) m_ClientSocket.Close();
        m_ClientSocket = null;
        if (m_ClientThread != null) m_ClientThread.Abort();
        m_ClientThread = null;
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
        if (m_IsConnecting == true)
            return;

        if (m_ClientSocket == null && 
            m_CurrentReconnectCount >= 0 &&
            (Time.realtimeSinceStartup - m_LastTryConnectTime) >= ReconnectInterval)
        {
            if (m_CurrentReconnectCount < ReconnectCount)
            {
                Connect(m_IP, m_Port);
                m_CurrentReconnectCount++;
            }
            else
            {
                m_CurrentReconnectCount = -1;
            }
        }

        while(m_QueuedMessages.Count > 0)
            OnProcessReceivedPacket(m_QueuedMessages.Dequeue());
    }

    protected virtual void OnDestroy()
    {
        Disconnect();
    }
}
