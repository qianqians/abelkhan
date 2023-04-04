import abelkhan
import center
import hub_server

class center_msg_handle(object):
    def __init__(self, _hub:hub_server.hub_svr, _modulemng:abelkhan.modulemng) -> None:
        self.hub = _hub
        self.centerproxy = _hub.centerproxy

        self.center_call_server_module = center.center_call_server_module(_modulemng)
        self.center_call_server_module.cb_close_server = self.close_server
        self.center_call_server_module.cb_console_close_server = self.console_close_server
        self.center_call_server_module.cb_svr_be_closed = self.svr_be_closed
        self.center_call_server_module.cb_take_over_svr = self.take_over_svr
		
        self.center_call_hub_module = center.center_call_hub_module(_modulemng)
        self.center_call_hub_module.cb_distribute_server_mq = self.distribute_server_mq
        self.center_call_hub_module.cb_reload = self.reload

    def close_server(self):
        self.hub.onCloseServer_event()

    def console_close_server(self, svr_type:str, svr_name:str):
        if svr_type == "hub" and svr_name == self.hub.name:
            self.hub.onCloseServer_event()
        else:
            self.svr_be_closed(svr_type, svr_name)
	
    def svr_be_closed(self, svr_type:str, svr_name:str):
        if svr_type == "hub":
            pass
    
    def take_over_svr(self, svr_name:str):
        if self.hub.is_support_take_over_svr:
            self.hub.redis_mq_service.take_over_svr(svr_name)
    
    def distribute_server_mq(self, type:str, name:str):
        if type == "hub":
            self.hub.reg_hub(name)

    def reload(self, argv:str):
        self.hub.onReload_event(argv)

