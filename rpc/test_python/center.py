from abelkhan import *
from threading import Timer
from collections.abc import Callable
from enum import Enum

# this enum code is codegen by abelkhan codegen for python

#this struct code is codegen by abelkhan codegen for python
#this caller code is codegen by abelkhan codegen for python
#this cb code is codegen by abelkhan for python
class center_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(center_rsp_cb, self).__init__()
        self.map_reg_server_mq = {}
        modules.reg_method("center_rsp_cb_reg_server_mq_rsp", [self, self.reg_server_mq_rsp])
        modules.reg_method("center_rsp_cb_reg_server_mq_err", [self, self.reg_server_mq_err])
        self.map_reconn_reg_server_mq = {}
        modules.reg_method("center_rsp_cb_reconn_reg_server_mq_rsp", [self, self.reconn_reg_server_mq_rsp])
        modules.reg_method("center_rsp_cb_reconn_reg_server_mq_err", [self, self.reconn_reg_server_mq_err])
        self.map_heartbeat = {}
        modules.reg_method("center_rsp_cb_heartbeat_rsp", [self, self.heartbeat_rsp])
        modules.reg_method("center_rsp_cb_heartbeat_err", [self, self.heartbeat_err])

    def reg_server_mq_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_reg_server_mq_cb(uuid)
        if rsp and rsp.event_reg_server_mq_handle_cb:
            rsp.event_reg_server_mq_handle_cb()

    def reg_server_mq_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_reg_server_mq_cb(uuid)
        if rsp and rsp.event_reg_server_mq_handle_err:
            rsp.event_reg_server_mq_handle_err()

    def reg_server_mq_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_reg_server_mq_cb(cb_uuid)
        if rsp and rsp.event_reg_server_mq_handle_timeout:
            rsp.event_reg_server_mq_handle_timeout()

    def try_get_and_del_reg_server_mq_cb(self, uuid : int):
        rsp = self.map_reg_server_mq.get(uuid)
        del self.map_reg_server_mq[uuid]
        return rsp

    def reconn_reg_server_mq_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_reconn_reg_server_mq_cb(uuid)
        if rsp and rsp.event_reconn_reg_server_mq_handle_cb:
            rsp.event_reconn_reg_server_mq_handle_cb()

    def reconn_reg_server_mq_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_reconn_reg_server_mq_cb(uuid)
        if rsp and rsp.event_reconn_reg_server_mq_handle_err:
            rsp.event_reconn_reg_server_mq_handle_err()

    def reconn_reg_server_mq_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_reconn_reg_server_mq_cb(cb_uuid)
        if rsp and rsp.event_reconn_reg_server_mq_handle_timeout:
            rsp.event_reconn_reg_server_mq_handle_timeout()

    def try_get_and_del_reconn_reg_server_mq_cb(self, uuid : int):
        rsp = self.map_reconn_reg_server_mq.get(uuid)
        del self.map_reconn_reg_server_mq[uuid]
        return rsp

    def heartbeat_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_heartbeat_cb(uuid)
        if rsp and rsp.event_heartbeat_handle_cb:
            rsp.event_heartbeat_handle_cb()

    def heartbeat_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_heartbeat_cb(uuid)
        if rsp and rsp.event_heartbeat_handle_err:
            rsp.event_heartbeat_handle_err()

    def heartbeat_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_heartbeat_cb(cb_uuid)
        if rsp and rsp.event_heartbeat_handle_timeout:
            rsp.event_heartbeat_handle_timeout()

    def try_get_and_del_heartbeat_cb(self, uuid : int):
        rsp = self.map_heartbeat.get(uuid)
        del self.map_heartbeat[uuid]
        return rsp

class center_reg_server_mq_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : center_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_reg_server_mq_handle_cb:Callable[[]] = None
        self.event_reg_server_mq_handle_err:Callable[[]] = None
        self.event_reg_server_mq_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[]]):
        self.event_reg_server_mq_handle_cb = _cb
        self.event_reg_server_mq_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.reg_server_mq_timeout(self.cb_uuid))
        t.start()
        self.event_reg_server_mq_handle_timeout = timeout_cb

class center_reconn_reg_server_mq_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : center_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_reconn_reg_server_mq_handle_cb:Callable[[]] = None
        self.event_reconn_reg_server_mq_handle_err:Callable[[]] = None
        self.event_reconn_reg_server_mq_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[]]):
        self.event_reconn_reg_server_mq_handle_cb = _cb
        self.event_reconn_reg_server_mq_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.reconn_reg_server_mq_timeout(self.cb_uuid))
        t.start()
        self.event_reconn_reg_server_mq_handle_timeout = timeout_cb

class center_heartbeat_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : center_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_heartbeat_handle_cb:Callable[[]] = None
        self.event_heartbeat_handle_err:Callable[[]] = None
        self.event_heartbeat_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[]]):
        self.event_heartbeat_handle_cb = _cb
        self.event_heartbeat_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.heartbeat_timeout(self.cb_uuid))
        t.start()
        self.event_heartbeat_handle_timeout = timeout_cb

rsp_cb_center_handle = None
class center_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(center_caller, self).__init__(_ch)

        self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 = RandomUUID()

        global rsp_cb_center_handle
        if not rsp_cb_center_handle:
            rsp_cb_center_handle = center_rsp_cb(modules)

    def reg_server_mq(self, type:str, hub_type:str, svr_name:str):
        self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 = (self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 + 1) & 0x7fffffff
        uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf = self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066

        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2 = [uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf]
        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.append(type)
        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.append(hub_type)
        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.append(svr_name)
        self.call_module_method("center_reg_server_mq", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2)

        cb_reg_server_mq_obj = center_reg_server_mq_cb(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf, rsp_cb_center_handle)
        global rsp_cb_center_handle
        if rsp_cb_center_handle:
            rsp_cb_center_handle.map_reg_server_mq[uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf] = cb_reg_server_mq_obj
        return cb_reg_server_mq_obj

    def reconn_reg_server_mq(self, type:str, hub_type:str, svr_name:str):
        self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 = (self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 + 1) & 0x7fffffff
        uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21 = self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066

        _argv_a018be20_2048_315d_9832_8120b194980f = [uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21]
        _argv_a018be20_2048_315d_9832_8120b194980f.append(type)
        _argv_a018be20_2048_315d_9832_8120b194980f.append(hub_type)
        _argv_a018be20_2048_315d_9832_8120b194980f.append(svr_name)
        self.call_module_method("center_reconn_reg_server_mq", _argv_a018be20_2048_315d_9832_8120b194980f)

        cb_reconn_reg_server_mq_obj = center_reconn_reg_server_mq_cb(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21, rsp_cb_center_handle)
        global rsp_cb_center_handle
        if rsp_cb_center_handle:
            rsp_cb_center_handle.map_reconn_reg_server_mq[uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21] = cb_reconn_reg_server_mq_obj
        return cb_reconn_reg_server_mq_obj

    def heartbeat(self, tick:int):
        self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 = (self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 + 1) & 0x7fffffff
        uuid_9654538a_9916_57dc_8ea5_806086d7a378 = self.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066

        _argv_af04a217_eafb_393c_9e34_0303485bef77 = [uuid_9654538a_9916_57dc_8ea5_806086d7a378]
        _argv_af04a217_eafb_393c_9e34_0303485bef77.append(tick)
        self.call_module_method("center_heartbeat", _argv_af04a217_eafb_393c_9e34_0303485bef77)

        cb_heartbeat_obj = center_heartbeat_cb(uuid_9654538a_9916_57dc_8ea5_806086d7a378, rsp_cb_center_handle)
        global rsp_cb_center_handle
        if rsp_cb_center_handle:
            rsp_cb_center_handle.map_heartbeat[uuid_9654538a_9916_57dc_8ea5_806086d7a378] = cb_heartbeat_obj
        return cb_heartbeat_obj

    def closed(self, ):
        _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee = []
        self.call_module_method("center_closed", _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee)

#this cb code is codegen by abelkhan for python
class center_call_server_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(center_call_server_rsp_cb, self).__init__()

rsp_cb_center_call_server_handle = None
class center_call_server_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(center_call_server_caller, self).__init__(_ch)

        self.uuid_8c11e5bb_e9ff_3a0b_a436_65a9922a8da5 = RandomUUID()

        global rsp_cb_center_call_server_handle
        if not rsp_cb_center_call_server_handle:
            rsp_cb_center_call_server_handle = center_call_server_rsp_cb(modules)

    def close_server(self, ):
        _argv_8394af17_8a06_3068_977d_477a1276f56e = []
        self.call_module_method("center_call_server_close_server", _argv_8394af17_8a06_3068_977d_477a1276f56e)

    def console_close_server(self, svr_type:str, svr_name:str):
        _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc = []
        _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc.append(svr_type)
        _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc.append(svr_name)
        self.call_module_method("center_call_server_console_close_server", _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc)

    def svr_be_closed(self, svr_type:str, svr_name:str):
        _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac = []
        _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.append(svr_type)
        _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.append(svr_name)
        self.call_module_method("center_call_server_svr_be_closed", _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac)

    def take_over_svr(self, svr_name:str):
        _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4 = []
        _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4.append(svr_name)
        self.call_module_method("center_call_server_take_over_svr", _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4)

#this cb code is codegen by abelkhan for python
class center_call_hub_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(center_call_hub_rsp_cb, self).__init__()

rsp_cb_center_call_hub_handle = None
class center_call_hub_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(center_call_hub_caller, self).__init__(_ch)

        self.uuid_adbd1e34_0c90_3426_aefa_4d734c07a706 = RandomUUID()

        global rsp_cb_center_call_hub_handle
        if not rsp_cb_center_call_hub_handle:
            rsp_cb_center_call_hub_handle = center_call_hub_rsp_cb(modules)

    def distribute_server_mq(self, svr_type:str, svr_name:str):
        _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393 = []
        _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393.append(svr_type)
        _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393.append(svr_name)
        self.call_module_method("center_call_hub_distribute_server_mq", _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393)

    def reload(self, argv:str):
        _argv_ba37af53_beea_3d61_82e1_8d15e335971d = []
        _argv_ba37af53_beea_3d61_82e1_8d15e335971d.append(argv)
        self.call_module_method("center_call_hub_reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d)

#this cb code is codegen by abelkhan for python
class gm_center_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(gm_center_rsp_cb, self).__init__()

rsp_cb_gm_center_handle = None
class gm_center_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(gm_center_caller, self).__init__(_ch)

        self.uuid_130fb971_5ae0_3446_b480_f9ee83dbeb28 = RandomUUID()

        global rsp_cb_gm_center_handle
        if not rsp_cb_gm_center_handle:
            rsp_cb_gm_center_handle = gm_center_rsp_cb(modules)

    def confirm_gm(self, gm_name:str):
        _argv_d097689c_b711_3837_8783_64916ae34cea = []
        _argv_d097689c_b711_3837_8783_64916ae34cea.append(gm_name)
        self.call_module_method("gm_center_confirm_gm", _argv_d097689c_b711_3837_8783_64916ae34cea)

    def close_clutter(self, gmname:str):
        _argv_576c03c3_06da_36a2_b868_752da601cb54 = []
        _argv_576c03c3_06da_36a2_b868_752da601cb54.append(gmname)
        self.call_module_method("gm_center_close_clutter", _argv_576c03c3_06da_36a2_b868_752da601cb54)

    def reload(self, gmname:str, argv:str):
        _argv_ba37af53_beea_3d61_82e1_8d15e335971d = []
        _argv_ba37af53_beea_3d61_82e1_8d15e335971d.append(gmname)
        _argv_ba37af53_beea_3d61_82e1_8d15e335971d.append(argv)
        self.call_module_method("gm_center_reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d)

#this module code is codegen by abelkhan codegen for python
class center_reg_server_mq_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(center_reg_server_mq_rsp, self).__init(_ch, _uuid)
        self.uuid_7254d987_ac9c_3d73_831c_f43efb3268a9 = _uuid

    def rsp(self, ):
        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2 = [self.uuid_7254d987_ac9c_3d73_831c_f43efb3268a9]
        self.call_module_method("center_rsp_cb_reg_server_mq_rsp", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2)

    def err(self, ):
        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2 = [self.uuid_7254d987_ac9c_3d73_831c_f43efb3268a9]
        self.call_module_method("center_rsp_cb_reg_server_mq_err", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2)

class center_reconn_reg_server_mq_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(center_reconn_reg_server_mq_rsp, self).__init(_ch, _uuid)
        self.uuid_4d058274_a122_382e_8084_b9067ed713c5 = _uuid

    def rsp(self, ):
        _argv_a018be20_2048_315d_9832_8120b194980f = [self.uuid_4d058274_a122_382e_8084_b9067ed713c5]
        self.call_module_method("center_rsp_cb_reconn_reg_server_mq_rsp", _argv_a018be20_2048_315d_9832_8120b194980f)

    def err(self, ):
        _argv_a018be20_2048_315d_9832_8120b194980f = [self.uuid_4d058274_a122_382e_8084_b9067ed713c5]
        self.call_module_method("center_rsp_cb_reconn_reg_server_mq_err", _argv_a018be20_2048_315d_9832_8120b194980f)

class center_heartbeat_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(center_heartbeat_rsp, self).__init(_ch, _uuid)
        self.uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f = _uuid

    def rsp(self, ):
        _argv_af04a217_eafb_393c_9e34_0303485bef77 = [self.uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f]
        self.call_module_method("center_rsp_cb_heartbeat_rsp", _argv_af04a217_eafb_393c_9e34_0303485bef77)

    def err(self, ):
        _argv_af04a217_eafb_393c_9e34_0303485bef77 = [self.uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f]
        self.call_module_method("center_rsp_cb_heartbeat_err", _argv_af04a217_eafb_393c_9e34_0303485bef77)

class center_module(Imodule):
    def __init__(self, modules:modulemng):
        super(center_module, self)
        self.modules = modules
        self.modules.reg_method("center_reg_server_mq", [self, self.reg_server_mq])
        self.modules.reg_method("center_reconn_reg_server_mq", [self, self.reconn_reg_server_mq])
        self.modules.reg_method("center_heartbeat", [self, self.heartbeat])
        self.modules.reg_method("center_closed", [self, self.closed])

        self.cb_reg_server_mq : Callable[[str, str, str]] = None
        self.cb_reconn_reg_server_mq : Callable[[str, str, str]] = None
        self.cb_heartbeat : Callable[[int]] = None
        self.cb_closed : Callable[[]] = None

    def reg_server_mq(self, inArray:list):
        _cb_uuid = inArray[0]
        _type = inArray[1]
        _hub_type = inArray[2]
        _svr_name = inArray[3]
        self.rsp = center_reg_server_mq_rsp(self.current_ch, _cb_uuid)
        if self.cb_reg_server_mq:
            self.cb_reg_server_mq(_type, _hub_type, _svr_name)
        self.rsp = None

    def reconn_reg_server_mq(self, inArray:list):
        _cb_uuid = inArray[0]
        _type = inArray[1]
        _hub_type = inArray[2]
        _svr_name = inArray[3]
        self.rsp = center_reconn_reg_server_mq_rsp(self.current_ch, _cb_uuid)
        if self.cb_reconn_reg_server_mq:
            self.cb_reconn_reg_server_mq(_type, _hub_type, _svr_name)
        self.rsp = None

    def heartbeat(self, inArray:list):
        _cb_uuid = inArray[0]
        _tick = inArray[1]
        self.rsp = center_heartbeat_rsp(self.current_ch, _cb_uuid)
        if self.cb_heartbeat:
            self.cb_heartbeat(_tick)
        self.rsp = None

    def closed(self, inArray:list):
        if self.cb_closed:
            self.cb_closed()

class center_call_server_module(Imodule):
    def __init__(self, modules:modulemng):
        super(center_call_server_module, self)
        self.modules = modules
        self.modules.reg_method("center_call_server_close_server", [self, self.close_server])
        self.modules.reg_method("center_call_server_console_close_server", [self, self.console_close_server])
        self.modules.reg_method("center_call_server_svr_be_closed", [self, self.svr_be_closed])
        self.modules.reg_method("center_call_server_take_over_svr", [self, self.take_over_svr])

        self.cb_close_server : Callable[[]] = None
        self.cb_console_close_server : Callable[[str, str]] = None
        self.cb_svr_be_closed : Callable[[str, str]] = None
        self.cb_take_over_svr : Callable[[str]] = None

    def close_server(self, inArray:list):
        if self.cb_close_server:
            self.cb_close_server()

    def console_close_server(self, inArray:list):
        _svr_type = inArray[0]
        _svr_name = inArray[1]
        if self.cb_console_close_server:
            self.cb_console_close_server(_svr_type, _svr_name)

    def svr_be_closed(self, inArray:list):
        _svr_type = inArray[0]
        _svr_name = inArray[1]
        if self.cb_svr_be_closed:
            self.cb_svr_be_closed(_svr_type, _svr_name)

    def take_over_svr(self, inArray:list):
        _svr_name = inArray[0]
        if self.cb_take_over_svr:
            self.cb_take_over_svr(_svr_name)

class center_call_hub_module(Imodule):
    def __init__(self, modules:modulemng):
        super(center_call_hub_module, self)
        self.modules = modules
        self.modules.reg_method("center_call_hub_distribute_server_mq", [self, self.distribute_server_mq])
        self.modules.reg_method("center_call_hub_reload", [self, self.reload])

        self.cb_distribute_server_mq : Callable[[str, str]] = None
        self.cb_reload : Callable[[str]] = None

    def distribute_server_mq(self, inArray:list):
        _svr_type = inArray[0]
        _svr_name = inArray[1]
        if self.cb_distribute_server_mq:
            self.cb_distribute_server_mq(_svr_type, _svr_name)

    def reload(self, inArray:list):
        _argv = inArray[0]
        if self.cb_reload:
            self.cb_reload(_argv)

class gm_center_module(Imodule):
    def __init__(self, modules:modulemng):
        super(gm_center_module, self)
        self.modules = modules
        self.modules.reg_method("gm_center_confirm_gm", [self, self.confirm_gm])
        self.modules.reg_method("gm_center_close_clutter", [self, self.close_clutter])
        self.modules.reg_method("gm_center_reload", [self, self.reload])

        self.cb_confirm_gm : Callable[[str]] = None
        self.cb_close_clutter : Callable[[str]] = None
        self.cb_reload : Callable[[str, str]] = None

    def confirm_gm(self, inArray:list):
        _gm_name = inArray[0]
        if self.cb_confirm_gm:
            self.cb_confirm_gm(_gm_name)

    def close_clutter(self, inArray:list):
        _gmname = inArray[0]
        if self.cb_close_clutter:
            self.cb_close_clutter(_gmname)

    def reload(self, inArray:list):
        _gmname = inArray[0]
        _argv = inArray[1]
        if self.cb_reload:
            self.cb_reload(_gmname, _argv)

