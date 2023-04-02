import hubproxy
import abelkhan
import hub_server
from collections.abc import Callable

class hubmanager(object):
    def __init__(self, _modulemng:abelkhan.modulemng, _hub:hub_server.hub_svr) -> None:
        self.modulemng = _modulemng
        self.hub = _hub

        self.current_hubproxy : hubproxy.hubproxy = None
        self.hubproxys : dict[str, hubproxy.hubproxy] = {}
        self.ch_hubproxys : dict[abelkhan.Ichannel, hubproxy.hubproxy] = {}

        self.on_hubproxy : Callable[[hubproxy.hubproxy]] = None
        self.on_hubproxy_reconn : Callable[[hubproxy.hubproxy]] = None

    def reg_hub(self, hub_name:str, hub_type:str, ch:abelkhan.Ichannel):
        _proxy = hubproxy.hubproxy(hub_name, hub_type, ch, self.modulemng)

        _old_proxy = self.hubproxys.get(hub_name)
        self.hubproxys[hub_name] = _proxy
        if _old_proxy:
            if self.on_hubproxy_reconn:
                self.on_hubproxy_reconn(_proxy)
        else:
            if self.on_hubproxy:
                self.on_hubproxy(_proxy)
        self.ch_hubproxys[ch] = _proxy

        self.hub.add_chs.append(ch)

    def get_hub(self, ch:abelkhan.Ichannel):
        return self.ch_hubproxys.get(ch)
    
    def call_hub(self, hub_name:str, func_name:str, _argvs:list):
        _proxy = self.hubproxys.get(hub_name)
        if _proxy:
            _proxy.caller_hub(func_name, _argvs)
        else:
            print(f"unreg hub:{hub_name}!")