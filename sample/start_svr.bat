start ./bin/center_svr.exe ./config.txt center &
start ./bin/dbproxy.exe ./config.txt dbproxy &
start  ./bin/gate.exe ./config.txt gate &
start  ./bin/test_csharp.exe ./config.txt test_csharp &
pause