using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleChatUI : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField m_InputNickname;
    [SerializeField] TMPro.TMP_InputField m_InputField;
    [SerializeField] TMPro.TextMeshProUGUI m_TextMessages;
    [SerializeField] Button m_BtnSend;

    private void Start()
    {
        NetworkManager.Instance.ChatServer.OnReceiveMessage += packet =>
        {
            if (packet == null) return;
            if (m_TextMessages == null) return;

            var dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0).Add(System.TimeZoneInfo.Local.BaseUtcOffset).AddSeconds(packet.Timestamp);
            m_TextMessages.text += '\n';
            m_TextMessages.text += string.Format("[{0}] {1}\n{2}", dateTime, packet.Nickname, packet.Message);
        };
        NetworkManager.Instance.ChatServer.Connect("127.0.0.1", 12345);
    }

    public void OnClick_SendInput()
    {
        if (string.IsNullOrEmpty(m_InputField.text))
            return;

        var lowerText = m_InputField.text.ToLower();
        if (lowerText.StartsWith("@changechannel "))
        {
            var splited = lowerText.Split(' ');
            if (splited.Length < 3) return;

            int type = 0;
            int channelId = 0;

            int.TryParse(splited[1], out type);
            int.TryParse(splited[2], out channelId);

            NetworkManager.Instance.ChatServer.SendChangeChannel(type, channelId);
        }
        else
        {
            NetworkManager.Instance.ChatServer.SendChatMessage(
                0,
                m_InputNickname == null || string.IsNullOrWhiteSpace(m_InputNickname.text) ? "Unknown" : m_InputNickname.text,
                m_InputField.text,
                null);

        }
    }
}
