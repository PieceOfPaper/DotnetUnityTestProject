using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : SingletonTemplate<NetworkManager>
{
    private GameObject m_ParentGameObject;
    private GameObject ParentGameObject
    {
        get
        {
            if (m_ParentGameObject== null)
            {
                m_ParentGameObject = new GameObject("NetworkManager");
                GameObject.DontDestroyOnLoad(m_ParentGameObject);
            }
            return m_ParentGameObject;
        }
    }

    private Server_Chat m_ChatServer;
    public Server_Chat ChatServer
    {
        get
        {
            if (m_ChatServer == null)
                m_ChatServer = ParentGameObject.AddComponent<Server_Chat>();
            return m_ChatServer;
        }
    }
}
