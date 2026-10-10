cd ../tools

protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/common.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/gate_client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/gate_hub.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/hub_gate.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/hub_hub.proto

pause