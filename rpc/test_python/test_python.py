from abelkhan import *
from threading import Timer
from collections.abc import Callable
from module import *
from enum import Enum
import modulemanager
import hubmanager

# this enum code is codegen by abelkhan codegen for python

#this struct code is codegen by abelkhan codegen for python
#this caller code is codegen by abelkhan codegen for python
#this cb code is codegen by abelkhan for python
class hub_hub_rsp_cb(imodule):
    def __init__(self, _modulemanager:modulemanager.modulemanager):
        self.modulemanager = _modulemanager
        self.map_test2:dict[str, hub_hub_test2_cb] = {}
        self.modulemanager.add_mothed("hub_hub_rsp_cb_test2_rsp", self.test2_rsp)
        self.modulemanager.add_mothed("hub_hub_rsp_cb_test2_err", self.test2_err)

    def test2_rsp(self, inArray:list):
        uuid = inArray[0]
        _token = inArray[1]
        rsp = self.try_get_and_del_test2_cb(uuid)
        if rsp and rsp.on_test2_cb:
            rsp.on_test2_cb(_token)

    def test2_err(self, inArray:list):
        uuid = inArray[0]
        _err = inArray[1]
        rsp = self.try_get_and_del_test2_cb(uuid)
        if rsp and rsp.on_test2_err:
            rsp.on_test2_err(_err)

    def test2_timeout(self, cb_uuid:int):
        rsp = self.try_get_and_del_test2_cb(cb_uuid)
        if rsp and rsp.on_test2_timeout:
            rsp.on_test2_timeout()

    def try_get_and_del_test2_cb(self, uuid:int):
        rsp = self.map_test2.get(uuid)
        del self.map_test2[uuid]
        return rsp

class hub_hub_test2_cb(object):
    def __init__(self, _cb_uuid:int, _module_rsp_cb : hub_hub_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.on_test2_cb:Callable[[str]] = None
        self.on_test2_err:Callable[[int]] = None
        self.on_test2_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[str]], _err:Callable[[int]]):
        self.on_test2_cb += _cb
        self.on_test2_err += _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.test2_timeout(self.cb_uuid))
        t.start()
        self.on_test2_timeout = timeout_cb

rsp_cb_hub_hub_handle : hub_hub_rsp_cb  = None
class hub_hub_hubproxy(object):
    def __init__(self, _hubs:hubmanager.hubmanager):
        self.hubs = _hubs
        self.hub_name_2707e093_e344_3ec7_8063_8b86e948eca8 = ""
        self.uuid_2707e093_e344_3ec7_8063_8b86e948eca8 = RandomUUID()

    def test1(self, id1:str, id2:int, _is:bool):
        _argv_c501822b_22a8_37ff_91a9_9545f4689a3d = []
        _argv_c501822b_22a8_37ff_91a9_9545f4689a3d.append(id1)
        _argv_c501822b_22a8_37ff_91a9_9545f4689a3d.append(id2)
        _argv_c501822b_22a8_37ff_91a9_9545f4689a3d.append(_is)
        self.hubs.call_hub(self.hub_name_2707e093_e344_3ec7_8063_8b86e948eca8, "hub_hub_test1", _argv_c501822b_22a8_37ff_91a9_9545f4689a3d)

    def test2(self, id:str):
        self.uuid_2707e093_e344_3ec7_8063_8b86e948eca8 = (self.uuid_2707e093_e344_3ec7_8063_8b86e948eca8 + 1) & 0x7fffffff
        uuid_6eabff02_c968_5cbc_bc7f_3b672928a761 = self.uuid_2707e093_e344_3ec7_8063_8b86e948eca8

        _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a = [uuid_6eabff02_c968_5cbc_bc7f_3b672928a761]
        _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a.append(id)
        self.hubs.call_hub(self.hub_name_2707e093_e344_3ec7_8063_8b86e948eca8, "hub_hub_test2", _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a)

        global rsp_cb_hub_hub_handle
        cb_test2_obj = hub_hub_test2_cb(uuid_6eabff02_c968_5cbc_bc7f_3b672928a761, rsp_cb_hub_hub_handle)
        if rsp_cb_hub_hub_handle:
            rsp_cb_hub_hub_handle.map_test2[uuid_6eabff02_c968_5cbc_bc7f_3b672928a761] = cb_test2_obj
        return cb_test2_obj

class hub_hub_caller(object):
    def __init__(self, _hubs:hubmanager.hubmanager):
        global rsp_cb_hub_hub_handle
        if rsp_cb_hub_hub_handle == None:
             rsp_cb_hub_hub_handle = hub_hub_rsp_cb()

        self.hubs = _hubs
        self._hubproxy = hub_hub_hubproxy(self.hubs)

    def get_hub(self, hub_name:str):
        self._hubproxy.hub_name_2707e093_e344_3ec7_8063_8b86e948eca8 = hub_name
        return self._hubproxy

#this module code is codegen by abelkhan codegen for python
class hub_hub_test2_rsp(Response):
    def __init__(self, hub_name:str, _uuid:int, _hubs:hubmanager.hubmanager):
        self.hubs = _hubs
        self._hub_name_f1917643_06b2_3e6d_ab77_0a5044067d0a = hub_name
        self.uuid_8a9af2f3_f1e2_3090_a9fd_bbf7656b7029 = _uuid

    def rsp(self, token:str):
        _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a = [self.uuid_8a9af2f3_f1e2_3090_a9fd_bbf7656b7029]
        _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a.append(token)
        self.hubs.call_hub(self._hub_name_f1917643_06b2_3e6d_ab77_0a5044067d0a, "hub_hub_rsp_cb_test2_rsp", _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a)

    def err(self, err:int):
        _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a = [self.uuid_8a9af2f3_f1e2_3090_a9fd_bbf7656b7029]
        _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a.append(err)
        self.hubs.call_hub(self._hub_name_f1917643_06b2_3e6d_ab77_0a5044067d0a, "hub_hub_rsp_cb_test2_err", _argv_f1917643_06b2_3e6d_ab77_0a5044067d0a)

class hub_hub_module(imodule):
    def __init__(self, _modulemanager:modulemanager.modulemanager, _hubs:hubmanager.hubmanager):
        self.modulemanager = _modulemanager
        self.hubs = _hubs
        self.on_test1:Callable[[str, int, bool]] = None
        self.modulemanager.add_mothed("hub_hub_test1", self.test1)
        self.on_test2:Callable[[str]] = None
        self.modulemanager.add_mothed("hub_hub_test2", self.test2)

    def test1(self, inArray:list):
        _id1 = inArray[0]
        _id2 = inArray[1]
        __is = inArray[2]
        if self.on_test1:
            self.on_test1(_id1, _id2, __is)

    def test2(self, inArray:list):
        _cb_uuid = inArray[0]
        _id = inArray[1]
        self.rsp = hub_hub_test2_rsp(self.hubs.current_hubproxy.name, _cb_uuid, self.hubs)
        if self.on_test2:
            self.on_test2(_id)
        self.rsp = None



