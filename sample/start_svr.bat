start ./bin/center_svr.exe ./config.txt center &
start ./bin/dbproxy_svr.exe ./config.txt dbproxy &
start  ./bin/gate.exe ./config.txt gate &
start  ./bin/test_csharp.exe ./config.txt test_csharp &
start  ./bin/test.exe ./config.txt test_cpp &
pause