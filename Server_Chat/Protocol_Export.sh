protoc --csharp_out=. *.proto
protoc --csharp_out=../DotnetUnityTester/Assets/Scripts/Network/Server_Chat *.proto
cp ./Protocol.proto ../DotnetUnityTester/Assets/Scripts/Network/Server_Chat/Protocol.proto
cp ./Defines.cs ../DotnetUnityTester/Assets/Scripts/Network/Server_Chat/Defines.cs
cp ./ProtobufExtension.cs ../DotnetUnityTester/Assets/Scripts/Network/Server_Chat/ProtobufExtension.cs