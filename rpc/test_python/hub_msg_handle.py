import msgpack
import abelkhan
import hubmanager
import hub
import modulemanager

class hub_msg_handle(object):
    def __init__(self, _hubmanager:hubmanager.hubmanager, _modulemanager:modulemanager.modulemanager, _modulemng:abelkhan.modulemng) -> None:
        self.hubmgr = _hubmanager
        self.modulemanager = _modulemanager

        self.hub_call_hub_module = hub.hub_call_hub_module(_modulemng)

        self.hub_call_hub_module.cb_reg_hub = self.reg_hub
        self.hub_call_hub_module.cb_hub_call_hub_mothed = self.hub_call_hub_mothed

    def reg_hub(self, hub_name:str, hub_type:str):
        print(f"hub:{hub_name},{hub_type} registered!")

        rsp = self.hub_call_hub_module.rsp
        self.hubmgr.reg_hub(hub_name, hub_type, self.hub_call_hub_module.current_ch)
        rsp.rsp()

    def hub_call_hub_mothed(self, rpc_argvs:bytes):
        _event = msgpack.loads(rpc_argvs)

        self.hubmgr.current_hubproxy = self.hubmgr.get_hub(self.hub_call_hub_module.current_ch)
        self.modulemanager.process_module_mothed(_event[0], _event[1])
        self.hubmgr.current_hubproxy = None