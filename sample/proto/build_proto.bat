cd ../../rpc/
python genc2h.py ../sample/proto/proto/client_call_server csharp ../sample/proto/csharp/cli ../sample/proto/csharp/svr
python genc2h.py ../sample/proto/proto/client_call_server cpp "" ../sample/proto/cpp 
python genc2h.py ../sample/proto/proto/client_call_server ts ../sample/proto/ts ""
python genh2c.py ../sample/proto/proto/server_call_client csharp  ../sample/proto/csharp/cli ../sample/proto/csharp/svr
python genh2c.py ../sample/proto/proto/server_call_client cpp "" ../sample/proto/cpp 
python genh2c.py ../sample/proto/proto/server_call_client ts ../sample/proto/ts ""
pause