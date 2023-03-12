protoc --csharp_out=. *.proto
protoc --csharp_out=../DotnetUnityTester/Assets/Scripts/Network/Server_Game *.proto
cp ./Protocol.proto ../DotnetUnityTester/Assets/Scripts/Network/Server_Game/Protocol.proto
cp ./Defines.cs ../DotnetUnityTester/Assets/Scripts/Network/Server_Game/Defines.cs