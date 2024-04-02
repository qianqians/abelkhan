cd ../../../rpc
python genc2h.py ../csharp_component/expand/rank/cli csharp ../csharp_component/expand/rank ../csharp_component/expand/rank/comm
python genc2h.py ../csharp_component/expand/rank/cli ts ../csharp_component/expand/rank_client_ts ../csharp_component/expand/rank/comm
python genh2h.py ../csharp_component/expand/rank/svr csharp ../csharp_component/expand/rank ../csharp_component/expand/rank/comm
pause