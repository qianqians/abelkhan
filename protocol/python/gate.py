from abelkhan import *
from threading import Timer
from collections.abc import Callable
from enum import Enum

from framework_error import *
# this enum code is codegen by abelkhan codegen for python

#this struct code is codegen by abelkhan codegen for python
class hub_info(object):
    def __init__(self):
        self.hub_name = 
        self.hub_type = 


def hub_info_to_protcol(_struct:hub_info):
    _protocol = {}
    _protocol["hub_name"] = _struct.hub_name
    _protocol["hub_type"] = _struct.hub_type
    return _protocol

def protcol_to_hub_info(_protocol:any):
    _struct = hub_info()
    for key, val in _protocol:
        if key == "hub_name":
            _struct.hub_name = val
        elif key == "hub_type":
            _struct.hub_type = val
    return _struct

#this caller code is codegen by abelkhan codegen for python
#this cb code is codegen by abelkhan for python
class client_call_gate_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(client_call_gate_rsp_cb, self).__init__()
        self.map_heartbeats = {}
        modules.reg_method("client_call_gate_rsp_cb_heartbeats_rsp", [self, self.heartbeats_rsp])
        modules.reg_method("client_call_gate_rsp_cb_heartbeats_err", [self, self.heartbeats_err])
        self.map_get_hub_info = {}
        modules.reg_method("client_call_gate_rsp_cb_get_hub_info_rsp", [self, self.get_hub_info_rsp])
        modules.reg_method("client_call_gate_rsp_cb_get_hub_info_err", [self, self.get_hub_info_err])

    def heartbeats_rsp(self, inArray:list):
        uuid = inArray[0]
        _timetmp = inArray[1]
        rsp = self.try_get_and_del_heartbeats_cb(uuid)
        if rsp and rsp.event_heartbeats_handle_cb:
            rsp.event_heartbeats_handle_cb(_timetmp)

    def heartbeats_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_heartbeats_cb(uuid)
        if rsp and rsp.event_heartbeats_handle_err:
            rsp.event_heartbeats_handle_err()

    def heartbeats_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_heartbeats_cb(cb_uuid)
        if rsp and rsp.event_heartbeats_handle_timeout:
            rsp.event_heartbeats_handle_timeout()

    def try_get_and_del_heartbeats_cb(self, uuid : int):
        rsp = self.map_heartbeats.get(uuid)
        del self.map_heartbeats[uuid]
        return rsp

    def get_hub_info_rsp(self, inArray:list):
        uuid = inArray[0]
        _hub_info = protcol_to_hub_info(inArray[1])
        rsp = self.try_get_and_del_get_hub_info_cb(uuid)
        if rsp and rsp.event_get_hub_info_handle_cb:
            rsp.event_get_hub_info_handle_cb(_hub_info)

    def get_hub_info_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_get_hub_info_cb(uuid)
        if rsp and rsp.event_get_hub_info_handle_err:
            rsp.event_get_hub_info_handle_err()

    def get_hub_info_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_get_hub_info_cb(cb_uuid)
        if rsp and rsp.event_get_hub_info_handle_timeout:
            rsp.event_get_hub_info_handle_timeout()

    def try_get_and_del_get_hub_info_cb(self, uuid : int):
        rsp = self.map_get_hub_info.get(uuid)
        del self.map_get_hub_info[uuid]
        return rsp

class client_call_gate_heartbeats_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : client_call_gate_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_heartbeats_handle_cb = None
        self.event_heartbeats_handle_err = None
        self.event_heartbeats_handle_timeout = None

    def callBack(self, _cb:Callable[[int]], _err:Callable[[]]):
        self.event_heartbeats_handle_cb = _cb
        self.event_heartbeats_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.heartbeats_timeout(self.cb_uuid))
        t.start()
        self.event_heartbeats_handle_timeout = timeout_cb

class client_call_gate_get_hub_info_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : client_call_gate_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_get_hub_info_handle_cb = None
        self.event_get_hub_info_handle_err = None
        self.event_get_hub_info_handle_timeout = None

    def callBack(self, _cb:Callable[[hub_info]], _err:Callable[[]]):
        self.event_get_hub_info_handle_cb = _cb
        self.event_get_hub_info_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.get_hub_info_timeout(self.cb_uuid))
        t.start()
        self.event_get_hub_info_handle_timeout = timeout_cb

rsp_cb_client_call_gate_handle = None
class client_call_gate_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(client_call_gate_caller, self).__init__(_ch)

        self.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2 = RandomUUID()

        global rsp_cb_client_call_gate_handle
        if not rsp_cb_client_call_gate_handle:
            rsp_cb_client_call_gate_handle = client_call_gate_rsp_cb(modules)

    def heartbeats(self, ):
        self.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2 = (self.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b21) & 0x7fffffff
        uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = self.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2

        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [uuid_a514ca5f_2c67_5668_aac0_354397bdce36]
        self.call_module_method("client_call_gate_heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4)

        cb_heartbeats_obj = client_call_gate_heartbeats_cb(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, rsp_cb_client_call_gate_handle)
        global rsp_cb_client_call_gate_handle
        if rsp_cb_client_call_gate_handle:
            rsp_cb_client_call_gate_handle.map_heartbeats[uuid_a514ca5f_2c67_5668_aac0_354397bdce36] = cb_heartbeats_obj
        return cb_heartbeats_obj

    def get_hub_info(self, hub_type:str):
        self.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2 = (self.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b21) & 0x7fffffff
        uuid_e9d2753f_7d38_512d_80ff_7aae13508048 = self.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2

        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = [uuid_e9d2753f_7d38_512d_80ff_7aae13508048]
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.append(hub_type)
        self.call_module_method("client_call_gate_get_hub_info", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24)

        cb_get_hub_info_obj = client_call_gate_get_hub_info_cb(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, rsp_cb_client_call_gate_handle)
        global rsp_cb_client_call_gate_handle
        if rsp_cb_client_call_gate_handle:
            rsp_cb_client_call_gate_handle.map_get_hub_info[uuid_e9d2753f_7d38_512d_80ff_7aae13508048] = cb_get_hub_info_obj
        return cb_get_hub_info_obj

    def forward_client_call_hub(self, hub_name:str, rpc_argv:bytes):
        _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5 = []
        _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.append(hub_name)
        _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.append(rpc_argv)
        self.call_module_method("client_call_gate_forward_client_call_hub", _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5)

#this cb code is codegen by abelkhan for python
class hub_call_gate_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(hub_call_gate_rsp_cb, self).__init__()
        self.map_reg_hub = {}
        modules.reg_method("hub_call_gate_rsp_cb_reg_hub_rsp", [self, self.reg_hub_rsp])
        modules.reg_method("hub_call_gate_rsp_cb_reg_hub_err", [self, self.reg_hub_err])
        self.map_reverse_reg_client_hub = {}
        modules.reg_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_rsp", [self, self.reverse_reg_client_hub_rsp])
        modules.reg_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_err", [self, self.reverse_reg_client_hub_err])

    def reg_hub_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_reg_hub_cb(uuid)
        if rsp and rsp.event_reg_hub_handle_cb:
            rsp.event_reg_hub_handle_cb()

    def reg_hub_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_reg_hub_cb(uuid)
        if rsp and rsp.event_reg_hub_handle_err:
            rsp.event_reg_hub_handle_err()

    def reg_hub_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_reg_hub_cb(cb_uuid)
        if rsp and rsp.event_reg_hub_handle_timeout:
            rsp.event_reg_hub_handle_timeout()

    def try_get_and_del_reg_hub_cb(self, uuid : int):
        rsp = self.map_reg_hub.get(uuid)
        del self.map_reg_hub[uuid]
        return rsp

    def reverse_reg_client_hub_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_reverse_reg_client_hub_cb(uuid)
        if rsp and rsp.event_reverse_reg_client_hub_handle_cb:
            rsp.event_reverse_reg_client_hub_handle_cb()

    def reverse_reg_client_hub_err(self, inArray:list):
        uuid = inArray[0]
        _err = inArray[1]
        rsp = self.try_get_and_del_reverse_reg_client_hub_cb(uuid)
        if rsp and rsp.event_reverse_reg_client_hub_handle_err:
            rsp.event_reverse_reg_client_hub_handle_err(_err)

    def reverse_reg_client_hub_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_reverse_reg_client_hub_cb(cb_uuid)
        if rsp and rsp.event_reverse_reg_client_hub_handle_timeout:
            rsp.event_reverse_reg_client_hub_handle_timeout()

    def try_get_and_del_reverse_reg_client_hub_cb(self, uuid : int):
        rsp = self.map_reverse_reg_client_hub.get(uuid)
        del self.map_reverse_reg_client_hub[uuid]
        return rsp

class hub_call_gate_reg_hub_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_gate_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_reg_hub_handle_cb = None
        self.event_reg_hub_handle_err = None
        self.event_reg_hub_handle_timeout = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[]]):
        self.event_reg_hub_handle_cb = _cb
        self.event_reg_hub_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.reg_hub_timeout(self.cb_uuid))
        t.start()
        self.event_reg_hub_handle_timeout = timeout_cb

class hub_call_gate_reverse_reg_client_hub_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_gate_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_reverse_reg_client_hub_handle_cb = None
        self.event_reverse_reg_client_hub_handle_err = None
        self.event_reverse_reg_client_hub_handle_timeout = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[framework_error.framework_error]]):
        self.event_reverse_reg_client_hub_handle_cb = _cb
        self.event_reverse_reg_client_hub_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.reverse_reg_client_hub_timeout(self.cb_uuid))
        t.start()
        self.event_reverse_reg_client_hub_handle_timeout = timeout_cb

rsp_cb_hub_call_gate_handle = None
class hub_call_gate_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(hub_call_gate_caller, self).__init__(_ch)

        self.uuid_9796175c_1119_3833_bf31_5ee139b40edc = RandomUUID()

        global rsp_cb_hub_call_gate_handle
        if not rsp_cb_hub_call_gate_handle:
            rsp_cb_hub_call_gate_handle = hub_call_gate_rsp_cb(modules)

    def reg_hub(self, hub_name:str, hub_type:str):
        self.uuid_9796175c_1119_3833_bf31_5ee139b40edc = (self.uuid_9796175c_1119_3833_bf31_5ee139b40edc1) & 0x7fffffff
        uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = self.uuid_9796175c_1119_3833_bf31_5ee139b40edc

        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106]
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.append(hub_name)
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.append(hub_type)
        self.call_module_method("hub_call_gate_reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

        cb_reg_hub_obj = hub_call_gate_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_gate_handle)
        global rsp_cb_hub_call_gate_handle
        if rsp_cb_hub_call_gate_handle:
            rsp_cb_hub_call_gate_handle.map_reg_hub[uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106] = cb_reg_hub_obj
        return cb_reg_hub_obj

    def tick_hub_health(self, tick_time:int):
        _argv_e81472b5_19a4_36bc_9cd9_b8fe87a10079 = []
        _argv_e81472b5_19a4_36bc_9cd9_b8fe87a10079.append(tick_time)
        self.call_module_method("hub_call_gate_tick_hub_health", _argv_e81472b5_19a4_36bc_9cd9_b8fe87a10079)

    def reverse_reg_client_hub(self, client_uuid:str):
        self.uuid_9796175c_1119_3833_bf31_5ee139b40edc = (self.uuid_9796175c_1119_3833_bf31_5ee139b40edc1) & 0x7fffffff
        uuid_5352b179_7aef_5875_a08f_06381972529f = self.uuid_9796175c_1119_3833_bf31_5ee139b40edc

        _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = [uuid_5352b179_7aef_5875_a08f_06381972529f]
        _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.append(client_uuid)
        self.call_module_method("hub_call_gate_reverse_reg_client_hub", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c)

        cb_reverse_reg_client_hub_obj = hub_call_gate_reverse_reg_client_hub_cb(uuid_5352b179_7aef_5875_a08f_06381972529f, rsp_cb_hub_call_gate_handle)
        global rsp_cb_hub_call_gate_handle
        if rsp_cb_hub_call_gate_handle:
            rsp_cb_hub_call_gate_handle.map_reverse_reg_client_hub[uuid_5352b179_7aef_5875_a08f_06381972529f] = cb_reverse_reg_client_hub_obj
        return cb_reverse_reg_client_hub_obj

    def unreg_client_hub(self, client_uuid:str):
        _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae = []
        _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae.append(client_uuid)
        self.call_module_method("hub_call_gate_unreg_client_hub", _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae)

    def disconnect_client(self, client_uuid:str):
        _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85 = []
        _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85.append(client_uuid)
        self.call_module_method("hub_call_gate_disconnect_client", _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85)

    def forward_hub_call_client(self, client_uuid:str, rpc_argv:bytes):
        _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1 = []
        _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.append(client_uuid)
        _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.append(rpc_argv)
        self.call_module_method("hub_call_gate_forward_hub_call_client", _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1)

    def forward_hub_call_group_client(self, client_uuids:list[str], rpc_argv:bytes):
        _argv_374b012d_0718_3f84_91f0_784b12f8189c = []
        _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1 = []
        for v_dfd11414_89c9_5adb_8977_69b93b30195b in client_uuids:
            _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1.append(v_dfd11414_89c9_5adb_8977_69b93b30195b)
        _argv_374b012d_0718_3f84_91f0_784b12f8189c.append(_array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1)
        _argv_374b012d_0718_3f84_91f0_784b12f8189c.append(rpc_argv)
        self.call_module_method("hub_call_gate_forward_hub_call_group_client", _argv_374b012d_0718_3f84_91f0_784b12f8189c)

    def forward_hub_call_global_client(self, rpc_argv:bytes):
        _argv_f69241c3_642a_3b51_bb37_cf638176493a = []
        _argv_f69241c3_642a_3b51_bb37_cf638176493a.append(rpc_argv)
        self.call_module_method("hub_call_gate_forward_hub_call_global_client", _argv_f69241c3_642a_3b51_bb37_cf638176493a)

#this module code is codegen by abelkhan codegen for python
class client_call_gate_heartbeats_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(client_call_gate_heartbeats_rsp, self).__init(_ch, _uuid)
        self.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 = _uuid

    def rsp(self, timetmp:int):
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [self.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56]
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.append(timetmp)
        self.call_module_method("client_call_gate_rsp_cb_heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4)

    def err(self, ):
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [self.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56]
        self.call_module_method("client_call_gate_rsp_cb_heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4)

class client_call_gate_get_hub_info_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(client_call_gate_get_hub_info_rsp, self).__init(_ch, _uuid)
        self.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b = _uuid

    def rsp(self, hub_info:hub_info):
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = [self.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b]
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.append(hub_info_to_protcol(hub_info))
        self.call_module_method("client_call_gate_rsp_cb_get_hub_info_rsp", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24)

    def err(self, ):
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = [self.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b]
        self.call_module_method("client_call_gate_rsp_cb_get_hub_info_err", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24)

class client_call_gate_module(Imodule):
    def __init__(self, modules:modulemng):
        super(client_call_gate_module, self)
        self.modules = modules
        self.modules.reg_method("client_call_gate_heartbeats", [self, self.heartbeats])
        self.modules.reg_method("client_call_gate_get_hub_info", [self, self.get_hub_info])
        self.modules.reg_method("client_call_gate_forward_client_call_hub", [self, self.forward_client_call_hub])

        self.cb_heartbeats : Callable[[]] = None
        self.cb_get_hub_info : Callable[[str]] = None
        self.cb_forward_client_call_hub : Callable[[str, bytes]] = None

    def heartbeats(self, inArray:list):
        _cb_uuid = inArray[0]
        self.rsp = client_call_gate_heartbeats_rsp(self.current_ch, _cb_uuid)
        if self.cb_heartbeats:
            self.cb_heartbeats()
        self.rsp = None

    def get_hub_info(self, inArray:list):
        _cb_uuid = inArray[0]
        _hub_type = inArray[1]
        self.rsp = client_call_gate_get_hub_info_rsp(self.current_ch, _cb_uuid)
        if self.cb_get_hub_info:
            self.cb_get_hub_info(_hub_type)
        self.rsp = None

    def forward_client_call_hub(self, inArray:list):
        _hub_name = inArray[0]
        _rpc_argv = inArray[1]
        if self.cb_forward_client_call_hub:
            self.cb_forward_client_call_hub(_hub_name, _rpc_argv)

class hub_call_gate_reg_hub_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_gate_reg_hub_rsp, self).__init(_ch, _uuid)
        self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid

    def rsp(self, ):
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67]
        self.call_module_method("hub_call_gate_rsp_cb_reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

    def err(self, ):
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67]
        self.call_module_method("hub_call_gate_rsp_cb_reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

class hub_call_gate_reverse_reg_client_hub_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_gate_reverse_reg_client_hub_rsp, self).__init(_ch, _uuid)
        self.uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a = _uuid

    def rsp(self, ):
        _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = [self.uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a]
        self.call_module_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_rsp", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c)

    def err(self, err:framework_error.framework_error):
        _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = [self.uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a]
        _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.append(err)
        self.call_module_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_err", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c)

class hub_call_gate_module(Imodule):
    def __init__(self, modules:modulemng):
        super(hub_call_gate_module, self)
        self.modules = modules
        self.modules.reg_method("hub_call_gate_reg_hub", [self, self.reg_hub])
        self.modules.reg_method("hub_call_gate_tick_hub_health", [self, self.tick_hub_health])
        self.modules.reg_method("hub_call_gate_reverse_reg_client_hub", [self, self.reverse_reg_client_hub])
        self.modules.reg_method("hub_call_gate_unreg_client_hub", [self, self.unreg_client_hub])
        self.modules.reg_method("hub_call_gate_disconnect_client", [self, self.disconnect_client])
        self.modules.reg_method("hub_call_gate_forward_hub_call_client", [self, self.forward_hub_call_client])
        self.modules.reg_method("hub_call_gate_forward_hub_call_group_client", [self, self.forward_hub_call_group_client])
        self.modules.reg_method("hub_call_gate_forward_hub_call_global_client", [self, self.forward_hub_call_global_client])

        self.cb_reg_hub : Callable[[str, str]] = None
        self.cb_tick_hub_health : Callable[[int]] = None
        self.cb_reverse_reg_client_hub : Callable[[str]] = None
        self.cb_unreg_client_hub : Callable[[str]] = None
        self.cb_disconnect_client : Callable[[str]] = None
        self.cb_forward_hub_call_client : Callable[[str, bytes]] = None
        self.cb_forward_hub_call_group_client : Callable[[list[str], bytes]] = None
        self.cb_forward_hub_call_global_client : Callable[[bytes]] = None

    def reg_hub(self, inArray:list):
        _cb_uuid = inArray[0]
        _hub_name = inArray[1]
        _hub_type = inArray[2]
        self.rsp = hub_call_gate_reg_hub_rsp(self.current_ch, _cb_uuid)
        if self.cb_reg_hub:
            self.cb_reg_hub(_hub_name, _hub_type)
        self.rsp = None

    def tick_hub_health(self, inArray:list):
        _tick_time = inArray[0]
        if self.cb_tick_hub_health:
            self.cb_tick_hub_health(_tick_time)

    def reverse_reg_client_hub(self, inArray:list):
        _cb_uuid = inArray[0]
        _client_uuid = inArray[1]
        self.rsp = hub_call_gate_reverse_reg_client_hub_rsp(self.current_ch, _cb_uuid)
        if self.cb_reverse_reg_client_hub:
            self.cb_reverse_reg_client_hub(_client_uuid)
        self.rsp = None

    def unreg_client_hub(self, inArray:list):
        _client_uuid = inArray[0]
        if self.cb_unreg_client_hub:
            self.cb_unreg_client_hub(_client_uuid)

    def disconnect_client(self, inArray:list):
        _client_uuid = inArray[0]
        if self.cb_disconnect_client:
            self.cb_disconnect_client(_client_uuid)

    def forward_hub_call_client(self, inArray:list):
        _client_uuid = inArray[0]
        _rpc_argv = inArray[1]
        if self.cb_forward_hub_call_client:
            self.cb_forward_hub_call_client(_client_uuid, _rpc_argv)

    def forward_hub_call_group_client(self, inArray:list):
        _client_uuids = []
        for v_ in inArray[0]:
            _client_uuids.append(v_)
        _rpc_argv = inArray[1]
        if self.cb_forward_hub_call_group_client:
            self.cb_forward_hub_call_group_client(_client_uuids, _rpc_argv)

    def forward_hub_call_global_client(self, inArray:list):
        _rpc_argv = inArray[0]
        if self.cb_forward_hub_call_global_client:
            self.cb_forward_hub_call_global_client(_rpc_argv)

