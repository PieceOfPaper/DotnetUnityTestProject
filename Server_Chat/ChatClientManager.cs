using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_Chat
{
    public class ChatClientManager : SingletonTemplate<ChatClientManager>
    {
        Dictionary<long, ChatClient> m_DicChatClients = new Dictionary<long, ChatClient>();
        Dictionary<int, Dictionary<int, List<ChatClient>>> m_DicChatClientsByChannel = new Dictionary<int, Dictionary<int, List<ChatClient>>>();

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

        public bool ChangeChannel(ChatClient client, int type, int prevChannelId, int nextChannelId)
        {
            if (m_DicChatClientsByChannel.ContainsKey(type) == false)
                m_DicChatClientsByChannel.Add(type, new Dictionary<int, List<ChatClient>>());

            if (m_DicChatClientsByChannel[type].ContainsKey(prevChannelId) == false)
                m_DicChatClientsByChannel[type].Add(prevChannelId, new List<ChatClient>());

            if (m_DicChatClientsByChannel[type].ContainsKey(nextChannelId) == false)
                m_DicChatClientsByChannel[type].Add(nextChannelId, new List<ChatClient>());

            m_DicChatClientsByChannel[type][prevChannelId].Remove(client);

            //이미 추가되어있었네?
            if (m_DicChatClientsByChannel[type][nextChannelId].Contains(client))
                return false;

            m_DicChatClientsByChannel[type][nextChannelId].Add(client);
            return true;
        }

        public bool BrodcastMessage(ChatClient client, ChatMessage chatMessage)
        {
            if (m_DicChatClientsByChannel.ContainsKey(chatMessage.Type) == false)
                return false;

            int channelId = client.GetChannelId(chatMessage.Type);
            if (channelId == 0)
                return false;

            if (m_DicChatClientsByChannel[chatMessage.Type].ContainsKey(channelId) == false)
                return false;

            foreach (var chatClient in m_DicChatClientsByChannel[chatMessage.Type][channelId])
            {
                chatClient.SendChatMessage(chatMessage);
            }
            return true;
        }
    }
}