cd ../../rpc/
python genc2h.py ../sample/proto/proto/client_call_server csharp ../sample/proto/csharp
python genc2h.py ../sample/proto/proto/client_call_server cpp ../sample/proto/cpp 
python genh2c.py ../sample/proto/proto/server_call_client csharp  ../sample/proto/csharp  
python genh2c.py ../sample/proto/proto/server_call_client cpp ../sample/proto/cpp 
pause