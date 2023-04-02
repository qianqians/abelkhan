import abelkhan
import center
from collections.abc import Callable
import hub_server

class centerproxy(object):
    def __init__(self, _ch:abelkhan.Ichannel, _modulemng:abelkhan.modulemng, _hub:hub_server.hub) -> None:
        self.timetmp = abelkhan.timetmp()
        self.is_reg_center_sucess = False
        self.ch = _ch
        self.center_caller = center.center_caller(self.ch, _modulemng)
        self.hub = _hub

    def __refresh_timetmp__(self):
        self.timetmp = abelkhan.timetmp()

    def reg_hub(self, callback:Callable[[]]) -> None:
        print("begin connect center server")

        self.center_caller.reg_server_mq("hub", self.hub.type, self.hub.name).callBack(
            callback,
            lambda : print("connect center server faild")
        ).timeout(5000, lambda : print("connect center server timeout"))

    def reconn_reg_hub(self, callback:Callable[[]]) -> None:
        print("begin connect center server")

        self.center_caller.reconn_reg_server_mq("hub", self.hub.type, self.hub.name).callBack(
            callback,
            lambda : print("reconnect center server faild")
        ).timeout(5000, lambda : print("reconnect center server timeout"))

    def heartbeat(self) -> None:
        print("begin heartbeath center server tick:!", abelkhan.timetmp())

        self.center_caller.heartbeat(abelkhan.timetmp()).callBack(
            lambda : self.__refresh_timetmp__(),
            lambda : print("heartbeat center server faild")
        ).timeout(5000, lambda : print("heartbeat center server timeout"))

    def closed(self):
        self.center_caller.closed()