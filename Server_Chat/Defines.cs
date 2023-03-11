namespace Server_Chat
{
    public enum PacketType : ushort
    {
        None = 0,
        CS_Chat_Message,
        SC_Chat_Message,
    }

    public static class ErrorCode
    {
        public static int SUCCESS = 0;
    }
}