from abelkhan import *
from threading import Timer
from collections.abc import Callable
from enum import Enum

from framework_error import *
# this enum code is codegen by abelkhan codegen for python

#this struct code is codegen by abelkhan codegen for python
#this caller code is codegen by abelkhan codegen for python
#this cb code is codegen by abelkhan for python
class gate_call_hub_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(gate_call_hub_rsp_cb, self).__init__()

rsp_cb_gate_call_hub_handle = None
class gate_call_hub_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(gate_call_hub_caller, self).__init__(_ch)

        self.uuid_e1565384_c90b_3a02_ae2e_d0d91b2758d1 = RandomUUID()

        global rsp_cb_gate_call_hub_handle
        if not rsp_cb_gate_call_hub_handle:
            rsp_cb_gate_call_hub_handle = gate_call_hub_rsp_cb(modules)

    def client_disconnect(self, client_uuid:str):
        _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60 = []
        _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60.append(client_uuid)
        self.call_module_method("gate_call_hub_client_disconnect", _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60)

    def client_exception(self, client_uuid:str):
        _argv_706b1331_3629_3681_9d39_d2ef3b6675ed = []
        _argv_706b1331_3629_3681_9d39_d2ef3b6675ed.append(client_uuid)
        self.call_module_method("gate_call_hub_client_exception", _argv_706b1331_3629_3681_9d39_d2ef3b6675ed)

    def client_call_hub(self, client_uuid:str, rpc_argv:bytes):
        _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = []
        _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.append(client_uuid)
        _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.append(rpc_argv)
        self.call_module_method("gate_call_hub_client_call_hub", _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263)

    def migrate_client(self, client_uuid:str, target_hub:str):
        _argv_871a9539_533c_387f_b7f2_4bd2ac7f4ef9 = []
        _argv_871a9539_533c_387f_b7f2_4bd2ac7f4ef9.append(client_uuid)
        _argv_871a9539_533c_387f_b7f2_4bd2ac7f4ef9.append(target_hub)
        self.call_module_method("gate_call_hub_migrate_client", _argv_871a9539_533c_387f_b7f2_4bd2ac7f4ef9)

#this cb code is codegen by abelkhan for python
class hub_call_hub_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(hub_call_hub_rsp_cb, self).__init__()
        self.map_reg_hub = {}
        modules.reg_method("hub_call_hub_rsp_cb_reg_hub_rsp", [self, self.reg_hub_rsp])
        modules.reg_method("hub_call_hub_rsp_cb_reg_hub_err", [self, self.reg_hub_err])
        self.map_seep_client_gate = {}
        modules.reg_method("hub_call_hub_rsp_cb_seep_client_gate_rsp", [self, self.seep_client_gate_rsp])
        modules.reg_method("hub_call_hub_rsp_cb_seep_client_gate_err", [self, self.seep_client_gate_err])

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

    def seep_client_gate_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_seep_client_gate_cb(uuid)
        if rsp and rsp.event_seep_client_gate_handle_cb:
            rsp.event_seep_client_gate_handle_cb()

    def seep_client_gate_err(self, inArray:list):
        uuid = inArray[0]
        _err = inArray[1]
        rsp = self.try_get_and_del_seep_client_gate_cb(uuid)
        if rsp and rsp.event_seep_client_gate_handle_err:
            rsp.event_seep_client_gate_handle_err(_err)

    def seep_client_gate_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_seep_client_gate_cb(cb_uuid)
        if rsp and rsp.event_seep_client_gate_handle_timeout:
            rsp.event_seep_client_gate_handle_timeout()

    def try_get_and_del_seep_client_gate_cb(self, uuid : int):
        rsp = self.map_seep_client_gate.get(uuid)
        del self.map_seep_client_gate[uuid]
        return rsp

class hub_call_hub_reg_hub_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_hub_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_reg_hub_handle_cb:Callable[[]] = None
        self.event_reg_hub_handle_err:Callable[[]] = None
        self.event_reg_hub_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[]]):
        self.event_reg_hub_handle_cb = _cb
        self.event_reg_hub_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.reg_hub_timeout(self.cb_uuid))
        t.start()
        self.event_reg_hub_handle_timeout = timeout_cb

class hub_call_hub_seep_client_gate_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_hub_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_seep_client_gate_handle_cb:Callable[[]] = None
        self.event_seep_client_gate_handle_err:Callable[[framework_error.framework_error]] = None
        self.event_seep_client_gate_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[framework_error.framework_error]]):
        self.event_seep_client_gate_handle_cb = _cb
        self.event_seep_client_gate_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.seep_client_gate_timeout(self.cb_uuid))
        t.start()
        self.event_seep_client_gate_handle_timeout = timeout_cb

rsp_cb_hub_call_hub_handle = None
class hub_call_hub_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(hub_call_hub_caller, self).__init__(_ch)

        self.uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f = RandomUUID()

        global rsp_cb_hub_call_hub_handle
        if not rsp_cb_hub_call_hub_handle:
            rsp_cb_hub_call_hub_handle = hub_call_hub_rsp_cb(modules)

    def reg_hub(self, hub_name:str, hub_type:str):
        self.uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f = (self.uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f + 1) & 0x7fffffff
        uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = self.uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f

        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106]
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.append(hub_name)
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.append(hub_type)
        self.call_module_method("hub_call_hub_reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

        global rsp_cb_hub_call_hub_handle
        cb_reg_hub_obj = hub_call_hub_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_hub_handle)
        if rsp_cb_hub_call_hub_handle:
            rsp_cb_hub_call_hub_handle.map_reg_hub[uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106] = cb_reg_hub_obj
        return cb_reg_hub_obj

    def seep_client_gate(self, client_uuid:str, gate_name:str):
        self.uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f = (self.uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f + 1) & 0x7fffffff
        uuid_31169fc3_4fd4_512f_b157_203819bcbd47 = self.uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f

        _argv_78da410b_1845_3253_9a34_d7cda82883b6 = [uuid_31169fc3_4fd4_512f_b157_203819bcbd47]
        _argv_78da410b_1845_3253_9a34_d7cda82883b6.append(client_uuid)
        _argv_78da410b_1845_3253_9a34_d7cda82883b6.append(gate_name)
        self.call_module_method("hub_call_hub_seep_client_gate", _argv_78da410b_1845_3253_9a34_d7cda82883b6)

        global rsp_cb_hub_call_hub_handle
        cb_seep_client_gate_obj = hub_call_hub_seep_client_gate_cb(uuid_31169fc3_4fd4_512f_b157_203819bcbd47, rsp_cb_hub_call_hub_handle)
        if rsp_cb_hub_call_hub_handle:
            rsp_cb_hub_call_hub_handle.map_seep_client_gate[uuid_31169fc3_4fd4_512f_b157_203819bcbd47] = cb_seep_client_gate_obj
        return cb_seep_client_gate_obj

    def hub_call_hub_mothed(self, rpc_argv:bytes):
        _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e = []
        _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.append(rpc_argv)
        self.call_module_method("hub_call_hub_hub_call_hub_mothed", _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e)

    def migrate_client(self, client_uuid:str, guid:int):
        _argv_871a9539_533c_387f_b7f2_4bd2ac7f4ef9 = []
        _argv_871a9539_533c_387f_b7f2_4bd2ac7f4ef9.append(client_uuid)
        _argv_871a9539_533c_387f_b7f2_4bd2ac7f4ef9.append(guid)
        self.call_module_method("hub_call_hub_migrate_client", _argv_871a9539_533c_387f_b7f2_4bd2ac7f4ef9)

#this cb code is codegen by abelkhan for python
class hub_call_client_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(hub_call_client_rsp_cb, self).__init__()

rsp_cb_hub_call_client_handle = None
class hub_call_client_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(hub_call_client_caller, self).__init__(_ch)

        self.uuid_44e0e3b5_d5d3_3ab4_87a3_bdf8d8aefeeb = RandomUUID()

        global rsp_cb_hub_call_client_handle
        if not rsp_cb_hub_call_client_handle:
            rsp_cb_hub_call_client_handle = hub_call_client_rsp_cb(modules)

    def call_client(self, rpc_argv:bytes):
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = []
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.append(rpc_argv)
        self.call_module_method("hub_call_client_call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab)

#this cb code is codegen by abelkhan for python
class client_call_hub_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(client_call_hub_rsp_cb, self).__init__()
        self.map_heartbeats = {}
        modules.reg_method("client_call_hub_rsp_cb_heartbeats_rsp", [self, self.heartbeats_rsp])
        modules.reg_method("client_call_hub_rsp_cb_heartbeats_err", [self, self.heartbeats_err])

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

class client_call_hub_heartbeats_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : client_call_hub_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_heartbeats_handle_cb:Callable[[int]] = None
        self.event_heartbeats_handle_err:Callable[[]] = None
        self.event_heartbeats_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[int]], _err:Callable[[]]):
        self.event_heartbeats_handle_cb = _cb
        self.event_heartbeats_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.heartbeats_timeout(self.cb_uuid))
        t.start()
        self.event_heartbeats_handle_timeout = timeout_cb

rsp_cb_client_call_hub_handle = None
class client_call_hub_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(client_call_hub_caller, self).__init__(_ch)

        self.uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = RandomUUID()

        global rsp_cb_client_call_hub_handle
        if not rsp_cb_client_call_hub_handle:
            rsp_cb_client_call_hub_handle = client_call_hub_rsp_cb(modules)

    def connect_hub(self, client_uuid:str):
        _argv_dc2ee339_bef5_3af9_a492_592ba4f08559 = []
        _argv_dc2ee339_bef5_3af9_a492_592ba4f08559.append(client_uuid)
        self.call_module_method("client_call_hub_connect_hub", _argv_dc2ee339_bef5_3af9_a492_592ba4f08559)

    def heartbeats(self, ):
        self.uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = (self.uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 + 1) & 0x7fffffff
        uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = self.uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263

        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [uuid_a514ca5f_2c67_5668_aac0_354397bdce36]
        self.call_module_method("client_call_hub_heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4)

        global rsp_cb_client_call_hub_handle
        cb_heartbeats_obj = client_call_hub_heartbeats_cb(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, rsp_cb_client_call_hub_handle)
        if rsp_cb_client_call_hub_handle:
            rsp_cb_client_call_hub_handle.map_heartbeats[uuid_a514ca5f_2c67_5668_aac0_354397bdce36] = cb_heartbeats_obj
        return cb_heartbeats_obj

    def call_hub(self, rpc_argv:bytes):
        _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3 = []
        _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.append(rpc_argv)
        self.call_module_method("client_call_hub_call_hub", _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3)

#this module code is codegen by abelkhan codegen for python
class gate_call_hub_module(Imodule):
    def __init__(self, modules:modulemng):
        super(gate_call_hub_module, self)
        self.modules = modules
        self.modules.reg_method("gate_call_hub_client_disconnect", [self, self.client_disconnect])
        self.modules.reg_method("gate_call_hub_client_exception", [self, self.client_exception])
        self.modules.reg_method("gate_call_hub_client_call_hub", [self, self.client_call_hub])
        self.modules.reg_method("gate_call_hub_migrate_client", [self, self.migrate_client])

        self.cb_client_disconnect : Callable[[str]] = None
        self.cb_client_exception : Callable[[str]] = None
        self.cb_client_call_hub : Callable[[str, bytes]] = None
        self.cb_migrate_client : Callable[[str, str]] = None

    def client_disconnect(self, inArray:list):
        _client_uuid = inArray[0]
        if self.cb_client_disconnect:
            self.cb_client_disconnect(_client_uuid)

    def client_exception(self, inArray:list):
        _client_uuid = inArray[0]
        if self.cb_client_exception:
            self.cb_client_exception(_client_uuid)

    def client_call_hub(self, inArray:list):
        _client_uuid = inArray[0]
        _rpc_argv = inArray[1]
        if self.cb_client_call_hub:
            self.cb_client_call_hub(_client_uuid, _rpc_argv)

    def migrate_client(self, inArray:list):
        _client_uuid = inArray[0]
        _target_hub = inArray[1]
        if self.cb_migrate_client:
            self.cb_migrate_client(_client_uuid, _target_hub)

class hub_call_hub_reg_hub_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_hub_reg_hub_rsp, self).__init(_ch, _uuid)
        self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid

    def rsp(self, ):
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67]
        self.call_module_method("hub_call_hub_rsp_cb_reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

    def err(self, ):
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67]
        self.call_module_method("hub_call_hub_rsp_cb_reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

class hub_call_hub_seep_client_gate_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_hub_seep_client_gate_rsp, self).__init(_ch, _uuid)
        self.uuid_3068725f_71fe_3459_a18d_b3f1dc698c98 = _uuid

    def rsp(self, ):
        _argv_78da410b_1845_3253_9a34_d7cda82883b6 = [self.uuid_3068725f_71fe_3459_a18d_b3f1dc698c98]
        self.call_module_method("hub_call_hub_rsp_cb_seep_client_gate_rsp", _argv_78da410b_1845_3253_9a34_d7cda82883b6)

    def err(self, err:framework_error.framework_error):
        _argv_78da410b_1845_3253_9a34_d7cda82883b6 = [self.uuid_3068725f_71fe_3459_a18d_b3f1dc698c98]
        _argv_78da410b_1845_3253_9a34_d7cda82883b6.append(err)
        self.call_module_method("hub_call_hub_rsp_cb_seep_client_gate_err", _argv_78da410b_1845_3253_9a34_d7cda82883b6)

class hub_call_hub_module(Imodule):
    def __init__(self, modules:modulemng):
        super(hub_call_hub_module, self)
        self.modules = modules
        self.modules.reg_method("hub_call_hub_reg_hub", [self, self.reg_hub])
        self.modules.reg_method("hub_call_hub_seep_client_gate", [self, self.seep_client_gate])
        self.modules.reg_method("hub_call_hub_hub_call_hub_mothed", [self, self.hub_call_hub_mothed])
        self.modules.reg_method("hub_call_hub_migrate_client", [self, self.migrate_client])

        self.cb_reg_hub : Callable[[str, str]] = None
        self.cb_seep_client_gate : Callable[[str, str]] = None
        self.cb_hub_call_hub_mothed : Callable[[bytes]] = None
        self.cb_migrate_client : Callable[[str, int]] = None

    def reg_hub(self, inArray:list):
        _cb_uuid = inArray[0]
        _hub_name = inArray[1]
        _hub_type = inArray[2]
        self.rsp = hub_call_hub_reg_hub_rsp(self.current_ch, _cb_uuid)
        if self.cb_reg_hub:
            self.cb_reg_hub(_hub_name, _hub_type)
        self.rsp = None

    def seep_client_gate(self, inArray:list):
        _cb_uuid = inArray[0]
        _client_uuid = inArray[1]
        _gate_name = inArray[2]
        self.rsp = hub_call_hub_seep_client_gate_rsp(self.current_ch, _cb_uuid)
        if self.cb_seep_client_gate:
            self.cb_seep_client_gate(_client_uuid, _gate_name)
        self.rsp = None

    def hub_call_hub_mothed(self, inArray:list):
        _rpc_argv = inArray[0]
        if self.cb_hub_call_hub_mothed:
            self.cb_hub_call_hub_mothed(_rpc_argv)

    def migrate_client(self, inArray:list):
        _client_uuid = inArray[0]
        _guid = inArray[1]
        if self.cb_migrate_client:
            self.cb_migrate_client(_client_uuid, _guid)

class hub_call_client_module(Imodule):
    def __init__(self, modules:modulemng):
        super(hub_call_client_module, self)
        self.modules = modules
        self.modules.reg_method("hub_call_client_call_client", [self, self.call_client])

        self.cb_call_client : Callable[[bytes]] = None

    def call_client(self, inArray:list):
        _rpc_argv = inArray[0]
        if self.cb_call_client:
            self.cb_call_client(_rpc_argv)

class client_call_hub_heartbeats_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(client_call_hub_heartbeats_rsp, self).__init(_ch, _uuid)
        self.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 = _uuid

    def rsp(self, timetmp:int):
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [self.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56]
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.append(timetmp)
        self.call_module_method("client_call_hub_rsp_cb_heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4)

    def err(self, ):
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [self.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56]
        self.call_module_method("client_call_hub_rsp_cb_heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4)

class client_call_hub_module(Imodule):
    def __init__(self, modules:modulemng):
        super(client_call_hub_module, self)
        self.modules = modules
        self.modules.reg_method("client_call_hub_connect_hub", [self, self.connect_hub])
        self.modules.reg_method("client_call_hub_heartbeats", [self, self.heartbeats])
        self.modules.reg_method("client_call_hub_call_hub", [self, self.call_hub])

        self.cb_connect_hub : Callable[[str]] = None
        self.cb_heartbeats : Callable[[]] = None
        self.cb_call_hub : Callable[[bytes]] = None

    def connect_hub(self, inArray:list):
        _client_uuid = inArray[0]
        if self.cb_connect_hub:
            self.cb_connect_hub(_client_uuid)

    def heartbeats(self, inArray:list):
        _cb_uuid = inArray[0]
        self.rsp = client_call_hub_heartbeats_rsp(self.current_ch, _cb_uuid)
        if self.cb_heartbeats:
            self.cb_heartbeats()
        self.rsp = None

    def call_hub(self, inArray:list):
        _rpc_argv = inArray[0]
        if self.cb_call_hub:
            self.cb_call_hub(_rpc_argv)

