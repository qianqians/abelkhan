protoc --csharp_out=../core  --proto_path=./  ./underlying.proto
protoc --csharp_out=../../client/csharp  --proto_path=./  ./underlying.proto

protoc --plugin=protoc-gen-ts_proto=../../node_modules/.bin/protoc-gen-ts_proto --ts_proto_out=../../client/typescript --proto_path=./  ./underlying.proto