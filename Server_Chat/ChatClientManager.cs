using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_Chat
{
    public class ChatClientManager : SingletonTemplate<ChatClientManager>
    {
        Dictionary<long, ChatClient> m_DicChatClients = new Dictionary<long, ChatClient>();

        public void AddChatClient(ChatClient chatClient)
        {
            m_DicChatClients.Add(chatClient.Handle, chatClient);
        }

        public bool RemoveChatClient(long handle)
        {
            if (m_DicChatClients.ContainsKey(handle) == false)
                return false;

            m_DicChatClients.Remove(handle);
            return true;
        }

        public void BrodcastMessage(ChatClient client, ChatMessage chatMessage)
        {
            foreach (var chatClient in m_DicChatClients.Values)
            {
                chatClient.SendMessage(chatMessage);
            }
        }
    }
}