cd ../../tools

protoc --csharp_out=../abelkhan/core  --proto_path=../abelkhan/proto  ../abelkhan/proto/common.proto
protoc --csharp_out=../abelkhan/core  --proto_path=../abelkhan/proto  ../abelkhan/proto/client.proto
protoc --csharp_out=../abelkhan/core  --proto_path=../abelkhan/proto  ../abelkhan/proto/dbproxy.proto
protoc --csharp_out=../abelkhan/core  --proto_path=../abelkhan/proto  ../abelkhan/proto/gate_client.proto
protoc --csharp_out=../abelkhan/core  --proto_path=../abelkhan/proto  ../abelkhan/proto/gate_hub.proto
protoc --csharp_out=../abelkhan/core  --proto_path=../abelkhan/proto  ../abelkhan/proto/hub_dbproxy.proto
protoc --csharp_out=../abelkhan/core  --proto_path=../abelkhan/proto  ../abelkhan/proto/hub_gate.proto


protoc --csharp_out=../client/csharp  --proto_path=../abelkhan/proto  ../abelkhan/proto/common.proto
protoc --csharp_out=../client/csharp  --proto_path=../abelkhan/proto  ../abelkhan/proto/client.proto

protoc --plugin=protoc-gen-ts_proto=..\node_modules\.bin\protoc-gen-ts_proto.cmd --ts_proto_out=..\client\typescript --proto_path=..\abelkhan\proto ..\abelkhan\proto\common.proto
protoc --plugin=protoc-gen-ts_proto=..\node_modules\.bin\protoc-gen-ts_proto.cmd --ts_proto_out=..\client\typescript --proto_path=..\abelkhan\proto ..\abelkhan\proto\client.proto

pause