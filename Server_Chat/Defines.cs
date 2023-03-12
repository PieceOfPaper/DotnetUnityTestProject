using System;
using Google.Protobuf;

namespace Server_Chat
{
    public enum PacketType : ushort
    {
        None = 0,
        CS_Chat_Message,
        SC_Chat_Message,

        CS_Change_Channel,
        SC_Change_Channel,
    }

    public static class PacketSerializer
    {
        public static byte[] SerializeWithType(ushort type, IMessage message)
        {
            var typeSize = sizeof(ushort);
            var typeBytes = BitConverter.GetBytes(type);
            byte[] data = message.ToByteArray();
            byte[] result = new byte[data.Length + typeSize];
            if (typeBytes != null && typeBytes.Length > 0)
                Buffer.BlockCopy(typeBytes, 0, result, 0, typeBytes.Length);
            Buffer.BlockCopy(data, 0, result, typeSize, data.Length);
            return result;
        }

        public static byte[] SerializeWithTypeErrorCode(ushort type, ErrorCode errorCode)
        {
            var typeSize = sizeof(ushort);
            var typeBytes = BitConverter.GetBytes(type);
            byte[] data = BitConverter.GetBytes((int)errorCode);
            byte[] result = new byte[data.Length + typeSize];
            if (typeBytes != null && typeBytes.Length > 0)
                Buffer.BlockCopy(typeBytes, 0, result, 0, typeBytes.Length);
            Buffer.BlockCopy(data, 0, result, typeSize, data.Length);
            return result;
        }

        public static ushort DeserializeType(byte[] data)
        {
            byte[] typeBytes = new byte[sizeof(ushort)];
            Buffer.BlockCopy(data, 0, typeBytes, 0, typeBytes.Length);
            return BitConverter.ToUInt16(typeBytes);
        }

        public static ErrorCode DeserializeErrorCode(byte[] data, int byteRead = 0)
        {
            if (byteRead == 0) byteRead = data.Length;
            var typeSize = sizeof(ushort);
            byte[] dataWithoutType = new byte[byteRead - typeSize];
            Buffer.BlockCopy(data, typeSize, dataWithoutType, 0, dataWithoutType.Length);
            return (ErrorCode)BitConverter.ToInt32(dataWithoutType);
        }

        public static T DeserializeWithoutType<T>(byte[] data, int byteRead = 0) where T : IMessage, new()
        {
            if (byteRead == 0) byteRead = data.Length;
            var typeSize = sizeof(ushort);
            byte[] dataWithoutType = new byte[byteRead - typeSize];
            Buffer.BlockCopy(data, typeSize, dataWithoutType, 0, dataWithoutType.Length);
            T message = new T();
            message.MergeFrom(dataWithoutType);
            return message;
        }
    }
}