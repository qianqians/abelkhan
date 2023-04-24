import uuid 
import json
import time
from threading import Timer
from collections.abc import Callable
import hub
import abelkhan
import redis_mq
import centerproxy
import hubmanager
import modulemanager
import center_msg_handle
import hub_msg_handle

class hub_svr(object):
    def __init__(self, config_file:str, _hub_name:str, _hub_type:str) -> None:
        self.name = f"{_hub_name}_{uuid.uuid1().hex}"
        self.type = _hub_type

        self.__is_closed__ = False

        self.add_chs : list[abelkhan.Ichannel] = []
        self.remove_chs : list[abelkhan.Ichannel] = []

        self.modulemng = abelkhan.modulemng()
        self.modulemanager = modulemanager.modulemanager()

        self.cfg = self.__open_config__(config_file)
        self.center_cfg = self.cfg.get("center")

        self.redis_mq_service = redis_mq.redis_mq(self.self.get("redis_for_mq"), self.modulemng)

        self.reconn_count = 0
        _center_ch = self.redis_mq_service.connect(self.center_cfg.get("name"))
        self.add_chs.append(_center_ch)
        self.centerproxy = centerproxy.centerproxy(_center_ch, self.modulemng, self)
        self.centerproxy.reg_hub(self.heartbeat)

        self.hubs = hubmanager.hubmanager(self.modulemng, self)
        self.is_support_take_over_svr = True
        
        self.center_msg_handle = center_msg_handle.center_msg_handle(self, self.modulemng)
        self.hub_msg_handle = hub_msg_handle.hub_msg_handle(self.hubs, self.modulemanager, self.modulemng)

        self.onCenterCrash : Callable[[]] = None
        self.onCloseServer : Callable[[]] = None
        self.onReload : Callable[[str]] = None

    def __open_config__(self, config_file:str) -> dict:
        f = open(config_file)
        config = f.read()
        return json.loads(config)
    
    def set_support_take_over_svr(self, is_support:bool) -> None:
        self.is_support_take_over_svr = is_support

    def heartbeat(self):
        t = Timer(3000, self.heartbeat)
        t.start()

        if (abelkhan.timetmp() - self.centerproxy.timetmp) > 9000:
            self.reconnect_center()

        self.centerproxy.heartbeat()

    def reset_reconn_count(self):
        self.reconn_count = 0

    def reconnect_center(self):
        if self.reconn_count > 5:
            if self.onCenterCrash:
                self.onCenterCrash()

        self.reconn_count += 1
        self.remove_chs.append(self.centerproxy.ch)

        _center_ch = self.redis_mq_service.connect(self.center_cfg.get("name"))
        self.add_chs.append(_center_ch)
        self.centerproxy = centerproxy.centerproxy(_center_ch, self.modulemng, self)
        self.centerproxy.reconn_reg_hub(lambda : self.reset_reconn_count())

    def onCloseServer_event(self):
        if self.onCloseServer:
            self.onCloseServer()

    def __set_closed__(self):
        self.__is_closed__ = True

    def closeSvr(self):
        self.centerproxy.closed()

        t = Timer(3000, self.__set_closed__)
        t.start()

    def onReload_event(self, argv:str):
        if self.onReload:
            self.onReload(argv)

    def reg_hub(self, hub_name:str):
        ch = self.redis_mq_service.connect(hub_name)
        _caller = Hub.Hub_call_hub_caller(ch, self.modulemng)
        _caller.reg_hub(self.name, self.type)
    
    def poll(self):
        _tick_begin = abelkhan.timetmp()

        try:
            self.redis_mq_service.recvmsg_mq()

            for ch in self.remove_chs:
                del self.add_chs[self.add_chs.index(ch)]

        except Exception as ex:
            print(ex)

        return abelkhan.timetmp() - _tick_begin

    def run(self):
        while not self.__is_closed__:
            _tick = self.poll()

            if _tick < 33:
                time.sleep(float(33 - _tick) / 1000)

        print(f"hub server{self.name} stop!")