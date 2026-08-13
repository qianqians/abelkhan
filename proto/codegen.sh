protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./common.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./dbproxy.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./gate_client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./gate_hub.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./hub_dbproxy.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./hub_gate.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=./  ./hub_hub.proto

protoc --csharp_out=../../client/csharp  --proto_path=./  ./common.proto
protoc --csharp_out=../../client/csharp  --proto_path=./  ./client.proto

protoc --plugin=protoc-gen-ts_proto=../../node_modules/.bin/protoc-gen-ts_proto --ts_proto_out=../../client/typescript --proto_path=./  ./common.proto
protoc --plugin=protoc-gen-ts_proto=../../node_modules/.bin/protoc-gen-ts_proto --ts_proto_out=../../client/typescript --proto_path=./  ./client.proto