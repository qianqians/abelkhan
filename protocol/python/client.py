from abelkhan import *
from threading import Timer
from collections.abc import Callable
from enum import Enum

# this enum code is codegen by abelkhan codegen for python

#this struct code is codegen by abelkhan codegen for python
#this caller code is codegen by abelkhan codegen for python
#this cb code is codegen by abelkhan for python
class gate_call_client_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(gate_call_client_rsp_cb, self).__init__()

rsp_cb_gate_call_client_handle = None
class gate_call_client_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(gate_call_client_caller, self).__init__(_ch)

        self.uuid_b84dd831_2e79_3280_a337_a69dd489e75f = RandomUUID()

        global rsp_cb_gate_call_client_handle
        if not rsp_cb_gate_call_client_handle:
            rsp_cb_gate_call_client_handle = gate_call_client_rsp_cb(modules)

    def ntf_cuuid(self, cuuid:str):
        _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1 = []
        _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.append(cuuid)
        self.call_module_method("gate_call_client_ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1)

    def call_client(self, hub_name:str, rpc_argv:bytes):
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = []
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.append(hub_name)
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.append(rpc_argv)
        self.call_module_method("gate_call_client_call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab)

    def migrate_client_start(self, src_hub:str, target_hub:str):
        _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee = []
        _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee.append(src_hub)
        _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee.append(target_hub)
        self.call_module_method("gate_call_client_migrate_client_start", _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee)

    def migrate_client_done(self, src_hub:str, target_hub:str):
        _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c = []
        _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c.append(src_hub)
        _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c.append(target_hub)
        self.call_module_method("gate_call_client_migrate_client_done", _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c)

    def hub_loss(self, hub_name:str):
        _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099 = []
        _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099.append(hub_name)
        self.call_module_method("gate_call_client_hub_loss", _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099)

#this module code is codegen by abelkhan codegen for python
class gate_call_client_module(Imodule):
    def __init__(self, modules:modulemng):
        super(gate_call_client_module, self)
        self.modules = modules
        self.modules.reg_method("gate_call_client_ntf_cuuid", [self, self.ntf_cuuid])
        self.modules.reg_method("gate_call_client_call_client", [self, self.call_client])
        self.modules.reg_method("gate_call_client_migrate_client_start", [self, self.migrate_client_start])
        self.modules.reg_method("gate_call_client_migrate_client_done", [self, self.migrate_client_done])
        self.modules.reg_method("gate_call_client_hub_loss", [self, self.hub_loss])

        self.cb_ntf_cuuid : Callable[[str]] = None
        self.cb_call_client : Callable[[str, bytes]] = None
        self.cb_migrate_client_start : Callable[[str, str]] = None
        self.cb_migrate_client_done : Callable[[str, str]] = None
        self.cb_hub_loss : Callable[[str]] = None

    def ntf_cuuid(self, inArray:list):
        _cuuid = inArray[0]
        if self.cb_ntf_cuuid:
            self.cb_ntf_cuuid(_cuuid)

    def call_client(self, inArray:list):
        _hub_name = inArray[0]
        _rpc_argv = inArray[1]
        if self.cb_call_client:
            self.cb_call_client(_hub_name, _rpc_argv)

    def migrate_client_start(self, inArray:list):
        _src_hub = inArray[0]
        _target_hub = inArray[1]
        if self.cb_migrate_client_start:
            self.cb_migrate_client_start(_src_hub, _target_hub)

    def migrate_client_done(self, inArray:list):
        _src_hub = inArray[0]
        _target_hub = inArray[1]
        if self.cb_migrate_client_done:
            self.cb_migrate_client_done(_src_hub, _target_hub)

    def hub_loss(self, inArray:list):
        _hub_name = inArray[0]
        if self.cb_hub_loss:
            self.cb_hub_loss(_hub_name)

