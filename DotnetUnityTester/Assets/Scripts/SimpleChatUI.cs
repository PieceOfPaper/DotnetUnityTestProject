using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleChatUI : MonoBehaviour
{
    [SerializeField] Server_Chat m_ChatServer;
    [SerializeField] TMPro.TMP_InputField m_InputField;
    [SerializeField] TMPro.TextMeshProUGUI m_TextMessages;
    [SerializeField] Button m_BtnSend;

    private void Start()
    {
        m_ChatServer.Connect("127.0.0.1", 12345);
        m_ChatServer.OnReceiveMessage += message =>
        {
            if (m_TextMessages == null) return;
            m_TextMessages.text += '\n';
            m_TextMessages.text += message;
        };
    }

    public void OnClick_SendInput()
    {
        if (string.IsNullOrEmpty(m_InputField.text))
            return;

        m_ChatServer.SendChatMessage(m_InputField.text);
    }
}
