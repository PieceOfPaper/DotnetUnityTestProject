using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleChatUI : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField m_InputField;
    [SerializeField] TMPro.TextMeshProUGUI m_TextMessages;
    [SerializeField] Button m_BtnSend;

    private void Start()
    {
        NetworkManager.Instance.ChatServer.OnReceiveMessage += packet =>
        {
            if (m_TextMessages == null) return;
            m_TextMessages.text += '\n';
            m_TextMessages.text += packet.message;
        };
        NetworkManager.Instance.ChatServer.Connect("127.0.0.1", 12345);
    }

    public void OnClick_SendInput()
    {
        if (string.IsNullOrEmpty(m_InputField.text))
            return;

        NetworkManager.Instance.ChatServer.SendChatMessage(new Server_Chat.ChatPacket()
        {
            nickname = "", //TODO
            message = m_InputField.text,
        });
    }
}
