protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./common.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./gate_client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./gate_hub.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./hub_gate.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./hub_hub.proto

protoc --csharp_out=../../client/csharp  --proto_path=./  ./common.proto
protoc --csharp_out=../../client/csharp  --proto_path=./  ./client.proto