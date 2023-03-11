using System;
using Google.Protobuf;

namespace Server_Chat
{
    public static class ProtobufExtension
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

        public static ushort DeserializeType(byte[] data)
        {
            byte[] typeBytes = new byte[sizeof(ushort)];
            Buffer.BlockCopy(data, 0, typeBytes, 0, typeBytes.Length);
            return BitConverter.ToUInt16(typeBytes);
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