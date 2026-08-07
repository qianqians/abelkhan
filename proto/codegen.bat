cd ../tools

protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/common.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/dbproxy.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/gate_client.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/gate_hub.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/hub_dbproxy.proto
protoc --csharp_out=../abelkhan/proto  --proto_path=../proto  ../proto/hub_gate.proto


protoc --csharp_out=../client/csharp  --proto_path=../proto  ../proto/common.proto
protoc --csharp_out=../client/csharp  --proto_path=../proto  ../proto/client.proto

protoc --plugin=protoc-gen-ts_proto=..\node_modules\.bin\protoc-gen-ts_proto.cmd --ts_proto_out=..\client\typescript --proto_path=..\proto ..\proto\common.proto
protoc --plugin=protoc-gen-ts_proto=..\node_modules\.bin\protoc-gen-ts_proto.cmd --ts_proto_out=..\client\typescript --proto_path=..\proto ..\proto\client.proto

pause