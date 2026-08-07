cd ../../tools

protoc --csharp_out=../abelkhan/core  --proto_path=../abelkhan/proto  ../abelkhan/proto/underlying.proto
protoc --csharp_out=../client/csharp  --proto_path=../abelkhan/proto  ../abelkhan/proto/underlying.proto

protoc --plugin=protoc-gen-ts_proto=..\node_modules\.bin\protoc-gen-ts_proto.cmd --ts_proto_out=..\client\typescript --proto_path=..\abelkhan\proto ..\abelkhan\proto\underlying.proto

pause