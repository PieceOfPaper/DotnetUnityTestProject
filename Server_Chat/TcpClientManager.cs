using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_Chat
{
    public class TcpClientManager : SingletonTemplate<TcpClientManager>
    {
        private long m_GeneratedHandle = 1;
        private Dictionary<long, TcpClient> m_DicTcpClients = new Dictionary<long, TcpClient>();


        public long AddTcpClient(TcpClient tcpClient)
        {
            var handle = m_GeneratedHandle ++;
            m_DicTcpClients.Add(handle, tcpClient);
            return handle;
        }

        public bool RemoveTcpClient(long handle)
        {
            if (m_DicTcpClients.ContainsKey(handle) == false)
                return false;

            m_DicTcpClients[handle].Close();
            m_DicTcpClients.Remove(handle);
            return true;
        }
    }
}