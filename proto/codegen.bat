cd ../tools

protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/common.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/gate_client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/gate_hub.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/hub_gate.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/hub_hub.proto

protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/bag.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/mail.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/shop.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/leaderboard.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/friend.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/chat.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/guild.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/quest.proto

pause
