import msgpack
import abelkhan
import hub

class hubproxy(object):
    def __init__(self, hub_name:str, hub_type:str, _ch:abelkhan.Ichannel, _modulemng:abelkhan.modulemng) -> None:
        self.name = hub_name
        self.type = hub_type
        self.ch = _ch
        self.hub_call_hub_caller = hub.hub_call_hub_caller(self.ch, _modulemng)
    
    def caller_hub(self, func_name:str, argvs:list):
        _event = [func_name, argvs]
        self.hub_call_hub_caller.hub_call_hub_mothed(msgpack.dumps(_event))
