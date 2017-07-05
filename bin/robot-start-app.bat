start center.exe robot_config.txt center
sleep 10
start dbproxy.exe robot_config.txt dbproxy
sleep 2
start gate.exe robot_config.txt gate
start test_hub.exe robot_config.txt test_hub
start test_robot.exe robot_config.txt test_robot

