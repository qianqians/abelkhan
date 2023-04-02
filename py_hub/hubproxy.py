import msgpack
import abelkhan
import hub
from collections.abc import Callable
import hub_server

class hubproxy(object):
    def __init__(self, hub_name:str, hub_type:str, _ch:abelkhan.Ichannel, _modulemng:abelkhan.modulemng, _hub:hub_server.hub) -> None:
        self.name = hub_name
        self.type = hub_type
        self.ch = _ch
        self.hub = _hub
        self.hub_call_hub_caller = hub.hub_call_hub_caller(self.ch, _modulemng)
    
    def caller_hub(self, func_name:str, argvs:list):
        _event = [func_name, argvs]
        self.hub_call_hub_caller.hub_call_hub_mothed(msgpack.dumps(_event))

    def client_seep(self, client_uuid:str):
        self.hub_call_hub_caller.seep_client_gate(client_uuid, self.hub._gates.get_client_gate_name(client_uuid))
