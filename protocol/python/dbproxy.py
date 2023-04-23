from abelkhan import *
from threading import Timer
from collections.abc import Callable
from enum import Enum

# this enum code is codegen by abelkhan codegen for python

#this struct code is codegen by abelkhan codegen for python
#this caller code is codegen by abelkhan codegen for python
#this cb code is codegen by abelkhan for python
class hub_call_dbproxy_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(hub_call_dbproxy_rsp_cb, self).__init__()
        self.map_reg_hub = {}
        modules.reg_method("hub_call_dbproxy_rsp_cb_reg_hub_rsp", [self, self.reg_hub_rsp])
        modules.reg_method("hub_call_dbproxy_rsp_cb_reg_hub_err", [self, self.reg_hub_err])
        self.map_get_guid = {}
        modules.reg_method("hub_call_dbproxy_rsp_cb_get_guid_rsp", [self, self.get_guid_rsp])
        modules.reg_method("hub_call_dbproxy_rsp_cb_get_guid_err", [self, self.get_guid_err])
        self.map_create_persisted_object = {}
        modules.reg_method("hub_call_dbproxy_rsp_cb_create_persisted_object_rsp", [self, self.create_persisted_object_rsp])
        modules.reg_method("hub_call_dbproxy_rsp_cb_create_persisted_object_err", [self, self.create_persisted_object_err])
        self.map_updata_persisted_object = {}
        modules.reg_method("hub_call_dbproxy_rsp_cb_updata_persisted_object_rsp", [self, self.updata_persisted_object_rsp])
        modules.reg_method("hub_call_dbproxy_rsp_cb_updata_persisted_object_err", [self, self.updata_persisted_object_err])
        self.map_find_and_modify = {}
        modules.reg_method("hub_call_dbproxy_rsp_cb_find_and_modify_rsp", [self, self.find_and_modify_rsp])
        modules.reg_method("hub_call_dbproxy_rsp_cb_find_and_modify_err", [self, self.find_and_modify_err])
        self.map_remove_object = {}
        modules.reg_method("hub_call_dbproxy_rsp_cb_remove_object_rsp", [self, self.remove_object_rsp])
        modules.reg_method("hub_call_dbproxy_rsp_cb_remove_object_err", [self, self.remove_object_err])
        self.map_get_object_count = {}
        modules.reg_method("hub_call_dbproxy_rsp_cb_get_object_count_rsp", [self, self.get_object_count_rsp])
        modules.reg_method("hub_call_dbproxy_rsp_cb_get_object_count_err", [self, self.get_object_count_err])

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

    def get_guid_rsp(self, inArray:list):
        uuid = inArray[0]
        _guid = inArray[1]
        rsp = self.try_get_and_del_get_guid_cb(uuid)
        if rsp and rsp.event_get_guid_handle_cb:
            rsp.event_get_guid_handle_cb(_guid)

    def get_guid_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_get_guid_cb(uuid)
        if rsp and rsp.event_get_guid_handle_err:
            rsp.event_get_guid_handle_err()

    def get_guid_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_get_guid_cb(cb_uuid)
        if rsp and rsp.event_get_guid_handle_timeout:
            rsp.event_get_guid_handle_timeout()

    def try_get_and_del_get_guid_cb(self, uuid : int):
        rsp = self.map_get_guid.get(uuid)
        del self.map_get_guid[uuid]
        return rsp

    def create_persisted_object_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_create_persisted_object_cb(uuid)
        if rsp and rsp.event_create_persisted_object_handle_cb:
            rsp.event_create_persisted_object_handle_cb()

    def create_persisted_object_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_create_persisted_object_cb(uuid)
        if rsp and rsp.event_create_persisted_object_handle_err:
            rsp.event_create_persisted_object_handle_err()

    def create_persisted_object_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_create_persisted_object_cb(cb_uuid)
        if rsp and rsp.event_create_persisted_object_handle_timeout:
            rsp.event_create_persisted_object_handle_timeout()

    def try_get_and_del_create_persisted_object_cb(self, uuid : int):
        rsp = self.map_create_persisted_object.get(uuid)
        del self.map_create_persisted_object[uuid]
        return rsp

    def updata_persisted_object_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_updata_persisted_object_cb(uuid)
        if rsp and rsp.event_updata_persisted_object_handle_cb:
            rsp.event_updata_persisted_object_handle_cb()

    def updata_persisted_object_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_updata_persisted_object_cb(uuid)
        if rsp and rsp.event_updata_persisted_object_handle_err:
            rsp.event_updata_persisted_object_handle_err()

    def updata_persisted_object_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_updata_persisted_object_cb(cb_uuid)
        if rsp and rsp.event_updata_persisted_object_handle_timeout:
            rsp.event_updata_persisted_object_handle_timeout()

    def try_get_and_del_updata_persisted_object_cb(self, uuid : int):
        rsp = self.map_updata_persisted_object.get(uuid)
        del self.map_updata_persisted_object[uuid]
        return rsp

    def find_and_modify_rsp(self, inArray:list):
        uuid = inArray[0]
        _object_info = inArray[1]
        rsp = self.try_get_and_del_find_and_modify_cb(uuid)
        if rsp and rsp.event_find_and_modify_handle_cb:
            rsp.event_find_and_modify_handle_cb(_object_info)

    def find_and_modify_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_find_and_modify_cb(uuid)
        if rsp and rsp.event_find_and_modify_handle_err:
            rsp.event_find_and_modify_handle_err()

    def find_and_modify_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_find_and_modify_cb(cb_uuid)
        if rsp and rsp.event_find_and_modify_handle_timeout:
            rsp.event_find_and_modify_handle_timeout()

    def try_get_and_del_find_and_modify_cb(self, uuid : int):
        rsp = self.map_find_and_modify.get(uuid)
        del self.map_find_and_modify[uuid]
        return rsp

    def remove_object_rsp(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_remove_object_cb(uuid)
        if rsp and rsp.event_remove_object_handle_cb:
            rsp.event_remove_object_handle_cb()

    def remove_object_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_remove_object_cb(uuid)
        if rsp and rsp.event_remove_object_handle_err:
            rsp.event_remove_object_handle_err()

    def remove_object_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_remove_object_cb(cb_uuid)
        if rsp and rsp.event_remove_object_handle_timeout:
            rsp.event_remove_object_handle_timeout()

    def try_get_and_del_remove_object_cb(self, uuid : int):
        rsp = self.map_remove_object.get(uuid)
        del self.map_remove_object[uuid]
        return rsp

    def get_object_count_rsp(self, inArray:list):
        uuid = inArray[0]
        _count = inArray[1]
        rsp = self.try_get_and_del_get_object_count_cb(uuid)
        if rsp and rsp.event_get_object_count_handle_cb:
            rsp.event_get_object_count_handle_cb(_count)

    def get_object_count_err(self, inArray:list):
        uuid = inArray[0]
        rsp = self.try_get_and_del_get_object_count_cb(uuid)
        if rsp and rsp.event_get_object_count_handle_err:
            rsp.event_get_object_count_handle_err()

    def get_object_count_timeout(self, cb_uuid : int):
        rsp = self.try_get_and_del_get_object_count_cb(cb_uuid)
        if rsp and rsp.event_get_object_count_handle_timeout:
            rsp.event_get_object_count_handle_timeout()

    def try_get_and_del_get_object_count_cb(self, uuid : int):
        rsp = self.map_get_object_count.get(uuid)
        del self.map_get_object_count[uuid]
        return rsp

class hub_call_dbproxy_reg_hub_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_dbproxy_rsp_cb):
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

class hub_call_dbproxy_get_guid_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_dbproxy_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_get_guid_handle_cb:Callable[[int]] = None
        self.event_get_guid_handle_err:Callable[[]] = None
        self.event_get_guid_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[int]], _err:Callable[[]]):
        self.event_get_guid_handle_cb = _cb
        self.event_get_guid_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.get_guid_timeout(self.cb_uuid))
        t.start()
        self.event_get_guid_handle_timeout = timeout_cb

class hub_call_dbproxy_create_persisted_object_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_dbproxy_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_create_persisted_object_handle_cb:Callable[[]] = None
        self.event_create_persisted_object_handle_err:Callable[[]] = None
        self.event_create_persisted_object_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[]]):
        self.event_create_persisted_object_handle_cb = _cb
        self.event_create_persisted_object_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.create_persisted_object_timeout(self.cb_uuid))
        t.start()
        self.event_create_persisted_object_handle_timeout = timeout_cb

class hub_call_dbproxy_updata_persisted_object_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_dbproxy_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_updata_persisted_object_handle_cb:Callable[[]] = None
        self.event_updata_persisted_object_handle_err:Callable[[]] = None
        self.event_updata_persisted_object_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[]]):
        self.event_updata_persisted_object_handle_cb = _cb
        self.event_updata_persisted_object_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.updata_persisted_object_timeout(self.cb_uuid))
        t.start()
        self.event_updata_persisted_object_handle_timeout = timeout_cb

class hub_call_dbproxy_find_and_modify_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_dbproxy_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_find_and_modify_handle_cb:Callable[[bytes]] = None
        self.event_find_and_modify_handle_err:Callable[[]] = None
        self.event_find_and_modify_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[bytes]], _err:Callable[[]]):
        self.event_find_and_modify_handle_cb = _cb
        self.event_find_and_modify_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.find_and_modify_timeout(self.cb_uuid))
        t.start()
        self.event_find_and_modify_handle_timeout = timeout_cb

class hub_call_dbproxy_remove_object_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_dbproxy_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_remove_object_handle_cb:Callable[[]] = None
        self.event_remove_object_handle_err:Callable[[]] = None
        self.event_remove_object_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[]], _err:Callable[[]]):
        self.event_remove_object_handle_cb = _cb
        self.event_remove_object_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.remove_object_timeout(self.cb_uuid))
        t.start()
        self.event_remove_object_handle_timeout = timeout_cb

class hub_call_dbproxy_get_object_count_cb:
    def __init__(self, _cb_uuid : int, _module_rsp_cb : hub_call_dbproxy_rsp_cb):
        self.cb_uuid = _cb_uuid
        self.module_rsp_cb = _module_rsp_cb
        self.event_get_object_count_handle_cb:Callable[[int]] = None
        self.event_get_object_count_handle_err:Callable[[]] = None
        self.event_get_object_count_handle_timeout:Callable[...] = None

    def callBack(self, _cb:Callable[[int]], _err:Callable[[]]):
        self.event_get_object_count_handle_cb = _cb
        self.event_get_object_count_handle_err = _err
        return self

    def timeout(self, tick:int, timeout_cb:Callable[...]):
        t = Timer(tick, lambda : self.module_rsp_cb.get_object_count_timeout(self.cb_uuid))
        t.start()
        self.event_get_object_count_handle_timeout = timeout_cb

rsp_cb_hub_call_dbproxy_handle = None
class hub_call_dbproxy_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(hub_call_dbproxy_caller, self).__init__(_ch)

        self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 = RandomUUID()

        global rsp_cb_hub_call_dbproxy_handle
        if not rsp_cb_hub_call_dbproxy_handle:
            rsp_cb_hub_call_dbproxy_handle = hub_call_dbproxy_rsp_cb(modules)

    def reg_hub(self, hub_name:str):
        self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 = (self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 + 1) & 0x7fffffff
        uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = self.uuid_e713438c_e791_3714_ad31_4ccbddee2554

        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106]
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.append(hub_name)
        self.call_module_method("hub_call_dbproxy_reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

        global rsp_cb_hub_call_dbproxy_handle
        cb_reg_hub_obj = hub_call_dbproxy_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_dbproxy_handle)
        if rsp_cb_hub_call_dbproxy_handle:
            rsp_cb_hub_call_dbproxy_handle.map_reg_hub[uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106] = cb_reg_hub_obj
        return cb_reg_hub_obj

    def get_guid(self, db:str, collection:str):
        self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 = (self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 + 1) & 0x7fffffff
        uuid_efe126e5_91e4_5df4_975c_18c91b6a6634 = self.uuid_e713438c_e791_3714_ad31_4ccbddee2554

        _argv_8b362c4a_74a5_366e_a6af_37474d7fa521 = [uuid_efe126e5_91e4_5df4_975c_18c91b6a6634]
        _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.append(db)
        _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.append(collection)
        self.call_module_method("hub_call_dbproxy_get_guid", _argv_8b362c4a_74a5_366e_a6af_37474d7fa521)

        global rsp_cb_hub_call_dbproxy_handle
        cb_get_guid_obj = hub_call_dbproxy_get_guid_cb(uuid_efe126e5_91e4_5df4_975c_18c91b6a6634, rsp_cb_hub_call_dbproxy_handle)
        if rsp_cb_hub_call_dbproxy_handle:
            rsp_cb_hub_call_dbproxy_handle.map_get_guid[uuid_efe126e5_91e4_5df4_975c_18c91b6a6634] = cb_get_guid_obj
        return cb_get_guid_obj

    def create_persisted_object(self, db:str, collection:str, object_info:bytes):
        self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 = (self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 + 1) & 0x7fffffff
        uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb = self.uuid_e713438c_e791_3714_ad31_4ccbddee2554

        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = [uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb]
        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.append(db)
        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.append(collection)
        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.append(object_info)
        self.call_module_method("hub_call_dbproxy_create_persisted_object", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607)

        global rsp_cb_hub_call_dbproxy_handle
        cb_create_persisted_object_obj = hub_call_dbproxy_create_persisted_object_cb(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb, rsp_cb_hub_call_dbproxy_handle)
        if rsp_cb_hub_call_dbproxy_handle:
            rsp_cb_hub_call_dbproxy_handle.map_create_persisted_object[uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb] = cb_create_persisted_object_obj
        return cb_create_persisted_object_obj

    def updata_persisted_object(self, db:str, collection:str, query_info:bytes, updata_info:bytes, _upsert:bool):
        self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 = (self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 + 1) & 0x7fffffff
        uuid_7864a402_2d75_5c02_b24b_50287a06732f = self.uuid_e713438c_e791_3714_ad31_4ccbddee2554

        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = [uuid_7864a402_2d75_5c02_b24b_50287a06732f]
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.append(db)
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.append(collection)
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.append(query_info)
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.append(updata_info)
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.append(_upsert)
        self.call_module_method("hub_call_dbproxy_updata_persisted_object", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425)

        global rsp_cb_hub_call_dbproxy_handle
        cb_updata_persisted_object_obj = hub_call_dbproxy_updata_persisted_object_cb(uuid_7864a402_2d75_5c02_b24b_50287a06732f, rsp_cb_hub_call_dbproxy_handle)
        if rsp_cb_hub_call_dbproxy_handle:
            rsp_cb_hub_call_dbproxy_handle.map_updata_persisted_object[uuid_7864a402_2d75_5c02_b24b_50287a06732f] = cb_updata_persisted_object_obj
        return cb_updata_persisted_object_obj

    def find_and_modify(self, db:str, collection:str, query_info:bytes, updata_info:bytes, _new:bool, _upsert:bool):
        self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 = (self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 + 1) & 0x7fffffff
        uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a = self.uuid_e713438c_e791_3714_ad31_4ccbddee2554

        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58 = [uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a]
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.append(db)
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.append(collection)
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.append(query_info)
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.append(updata_info)
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.append(_new)
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.append(_upsert)
        self.call_module_method("hub_call_dbproxy_find_and_modify", _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58)

        global rsp_cb_hub_call_dbproxy_handle
        cb_find_and_modify_obj = hub_call_dbproxy_find_and_modify_cb(uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a, rsp_cb_hub_call_dbproxy_handle)
        if rsp_cb_hub_call_dbproxy_handle:
            rsp_cb_hub_call_dbproxy_handle.map_find_and_modify[uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a] = cb_find_and_modify_obj
        return cb_find_and_modify_obj

    def remove_object(self, db:str, collection:str, query_info:bytes):
        self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 = (self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 + 1) & 0x7fffffff
        uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f = self.uuid_e713438c_e791_3714_ad31_4ccbddee2554

        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = [uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f]
        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.append(db)
        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.append(collection)
        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.append(query_info)
        self.call_module_method("hub_call_dbproxy_remove_object", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da)

        global rsp_cb_hub_call_dbproxy_handle
        cb_remove_object_obj = hub_call_dbproxy_remove_object_cb(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f, rsp_cb_hub_call_dbproxy_handle)
        if rsp_cb_hub_call_dbproxy_handle:
            rsp_cb_hub_call_dbproxy_handle.map_remove_object[uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f] = cb_remove_object_obj
        return cb_remove_object_obj

    def get_object_info(self, db:str, collection:str, query_info:bytes, _skip:int, _limit:int, _sort:str, _Ascending_:bool, callbackid:str):
        _argv_1f17e6de_d423_391b_a599_7268e665a53f = []
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.append(db)
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.append(collection)
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.append(query_info)
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.append(_skip)
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.append(_limit)
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.append(_sort)
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.append(_Ascending_)
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.append(callbackid)
        self.call_module_method("hub_call_dbproxy_get_object_info", _argv_1f17e6de_d423_391b_a599_7268e665a53f)

    def get_object_count(self, db:str, collection:str, query_info:bytes):
        self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 = (self.uuid_e713438c_e791_3714_ad31_4ccbddee2554 + 1) & 0x7fffffff
        uuid_975425f5_8baf_5905_beeb_4454e78907f6 = self.uuid_e713438c_e791_3714_ad31_4ccbddee2554

        _argv_2632cded_162c_3a9b_86ee_462b614cbeea = [uuid_975425f5_8baf_5905_beeb_4454e78907f6]
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.append(db)
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.append(collection)
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.append(query_info)
        self.call_module_method("hub_call_dbproxy_get_object_count", _argv_2632cded_162c_3a9b_86ee_462b614cbeea)

        global rsp_cb_hub_call_dbproxy_handle
        cb_get_object_count_obj = hub_call_dbproxy_get_object_count_cb(uuid_975425f5_8baf_5905_beeb_4454e78907f6, rsp_cb_hub_call_dbproxy_handle)
        if rsp_cb_hub_call_dbproxy_handle:
            rsp_cb_hub_call_dbproxy_handle.map_get_object_count[uuid_975425f5_8baf_5905_beeb_4454e78907f6] = cb_get_object_count_obj
        return cb_get_object_count_obj

#this cb code is codegen by abelkhan for python
class dbproxy_call_hub_rsp_cb(Imodule):
    def __init__(self, modules:modulemng):
        super(dbproxy_call_hub_rsp_cb, self).__init__()

rsp_cb_dbproxy_call_hub_handle = None
class dbproxy_call_hub_caller(Icaller):
    def __init_(self, _ch:Ichannel, modules:modulemng):
        super(dbproxy_call_hub_caller, self).__init__(_ch)

        self.uuid_7a1d0ce9_a121_3019_b67a_319998ea37c8 = RandomUUID()

        global rsp_cb_dbproxy_call_hub_handle
        if not rsp_cb_dbproxy_call_hub_handle:
            rsp_cb_dbproxy_call_hub_handle = dbproxy_call_hub_rsp_cb(modules)

    def ack_get_object_info(self, callbackid:str, object_info:bytes):
        _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7 = []
        _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.append(callbackid)
        _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.append(object_info)
        self.call_module_method("dbproxy_call_hub_ack_get_object_info", _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7)

    def ack_get_object_info_end(self, callbackid:str):
        _argv_e4756ccf_94e2_3b4f_958a_701f7076e607 = []
        _argv_e4756ccf_94e2_3b4f_958a_701f7076e607.append(callbackid)
        self.call_module_method("dbproxy_call_hub_ack_get_object_info_end", _argv_e4756ccf_94e2_3b4f_958a_701f7076e607)

#this module code is codegen by abelkhan codegen for python
class hub_call_dbproxy_reg_hub_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_dbproxy_reg_hub_rsp, self).__init(_ch, _uuid)
        self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid

    def rsp(self, ):
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67]
        self.call_module_method("hub_call_dbproxy_rsp_cb_reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

    def err(self, ):
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [self.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67]
        self.call_module_method("hub_call_dbproxy_rsp_cb_reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7)

class hub_call_dbproxy_get_guid_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_dbproxy_get_guid_rsp, self).__init(_ch, _uuid)
        self.uuid_ed8b33be_8d91_3840_a2fc_8a3c7dbb6948 = _uuid

    def rsp(self, guid:int):
        _argv_8b362c4a_74a5_366e_a6af_37474d7fa521 = [self.uuid_ed8b33be_8d91_3840_a2fc_8a3c7dbb6948]
        _argv_8b362c4a_74a5_366e_a6af_37474d7fa521.append(guid)
        self.call_module_method("hub_call_dbproxy_rsp_cb_get_guid_rsp", _argv_8b362c4a_74a5_366e_a6af_37474d7fa521)

    def err(self, ):
        _argv_8b362c4a_74a5_366e_a6af_37474d7fa521 = [self.uuid_ed8b33be_8d91_3840_a2fc_8a3c7dbb6948]
        self.call_module_method("hub_call_dbproxy_rsp_cb_get_guid_err", _argv_8b362c4a_74a5_366e_a6af_37474d7fa521)

class hub_call_dbproxy_create_persisted_object_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_dbproxy_create_persisted_object_rsp, self).__init(_ch, _uuid)
        self.uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b = _uuid

    def rsp(self, ):
        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = [self.uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b]
        self.call_module_method("hub_call_dbproxy_rsp_cb_create_persisted_object_rsp", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607)

    def err(self, ):
        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = [self.uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b]
        self.call_module_method("hub_call_dbproxy_rsp_cb_create_persisted_object_err", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607)

class hub_call_dbproxy_updata_persisted_object_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_dbproxy_updata_persisted_object_rsp, self).__init(_ch, _uuid)
        self.uuid_16267d40_cddc_312f_87c0_185a55b79ad2 = _uuid

    def rsp(self, ):
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = [self.uuid_16267d40_cddc_312f_87c0_185a55b79ad2]
        self.call_module_method("hub_call_dbproxy_rsp_cb_updata_persisted_object_rsp", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425)

    def err(self, ):
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = [self.uuid_16267d40_cddc_312f_87c0_185a55b79ad2]
        self.call_module_method("hub_call_dbproxy_rsp_cb_updata_persisted_object_err", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425)

class hub_call_dbproxy_find_and_modify_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_dbproxy_find_and_modify_rsp, self).__init(_ch, _uuid)
        self.uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae = _uuid

    def rsp(self, object_info:bytes):
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58 = [self.uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae]
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.append(object_info)
        self.call_module_method("hub_call_dbproxy_rsp_cb_find_and_modify_rsp", _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58)

    def err(self, ):
        _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58 = [self.uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae]
        self.call_module_method("hub_call_dbproxy_rsp_cb_find_and_modify_err", _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58)

class hub_call_dbproxy_remove_object_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_dbproxy_remove_object_rsp, self).__init(_ch, _uuid)
        self.uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1 = _uuid

    def rsp(self, ):
        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = [self.uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1]
        self.call_module_method("hub_call_dbproxy_rsp_cb_remove_object_rsp", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da)

    def err(self, ):
        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = [self.uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1]
        self.call_module_method("hub_call_dbproxy_rsp_cb_remove_object_err", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da)

class hub_call_dbproxy_get_object_count_rsp(Response):
    def __init__(self, _ch:Ichannel, _uuid:int):
        super(hub_call_dbproxy_get_object_count_rsp, self).__init(_ch, _uuid)
        self.uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2 = _uuid

    def rsp(self, count:int):
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea = [self.uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2]
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.append(count)
        self.call_module_method("hub_call_dbproxy_rsp_cb_get_object_count_rsp", _argv_2632cded_162c_3a9b_86ee_462b614cbeea)

    def err(self, ):
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea = [self.uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2]
        self.call_module_method("hub_call_dbproxy_rsp_cb_get_object_count_err", _argv_2632cded_162c_3a9b_86ee_462b614cbeea)

class hub_call_dbproxy_module(Imodule):
    def __init__(self, modules:modulemng):
        super(hub_call_dbproxy_module, self)
        self.modules = modules
        self.modules.reg_method("hub_call_dbproxy_reg_hub", [self, self.reg_hub])
        self.modules.reg_method("hub_call_dbproxy_get_guid", [self, self.get_guid])
        self.modules.reg_method("hub_call_dbproxy_create_persisted_object", [self, self.create_persisted_object])
        self.modules.reg_method("hub_call_dbproxy_updata_persisted_object", [self, self.updata_persisted_object])
        self.modules.reg_method("hub_call_dbproxy_find_and_modify", [self, self.find_and_modify])
        self.modules.reg_method("hub_call_dbproxy_remove_object", [self, self.remove_object])
        self.modules.reg_method("hub_call_dbproxy_get_object_info", [self, self.get_object_info])
        self.modules.reg_method("hub_call_dbproxy_get_object_count", [self, self.get_object_count])

        self.cb_reg_hub : Callable[[str]] = None
        self.cb_get_guid : Callable[[str, str]] = None
        self.cb_create_persisted_object : Callable[[str, str, bytes]] = None
        self.cb_updata_persisted_object : Callable[[str, str, bytes, bytes, bool]] = None
        self.cb_find_and_modify : Callable[[str, str, bytes, bytes, bool, bool]] = None
        self.cb_remove_object : Callable[[str, str, bytes]] = None
        self.cb_get_object_info : Callable[[str, str, bytes, int, int, str, bool, str]] = None
        self.cb_get_object_count : Callable[[str, str, bytes]] = None

    def reg_hub(self, inArray:list):
        _cb_uuid = inArray[0]
        _hub_name = inArray[1]
        self.rsp = hub_call_dbproxy_reg_hub_rsp(self.current_ch, _cb_uuid)
        if self.cb_reg_hub:
            self.cb_reg_hub(_hub_name)
        self.rsp = None

    def get_guid(self, inArray:list):
        _cb_uuid = inArray[0]
        _db = inArray[1]
        _collection = inArray[2]
        self.rsp = hub_call_dbproxy_get_guid_rsp(self.current_ch, _cb_uuid)
        if self.cb_get_guid:
            self.cb_get_guid(_db, _collection)
        self.rsp = None

    def create_persisted_object(self, inArray:list):
        _cb_uuid = inArray[0]
        _db = inArray[1]
        _collection = inArray[2]
        _object_info = inArray[3]
        self.rsp = hub_call_dbproxy_create_persisted_object_rsp(self.current_ch, _cb_uuid)
        if self.cb_create_persisted_object:
            self.cb_create_persisted_object(_db, _collection, _object_info)
        self.rsp = None

    def updata_persisted_object(self, inArray:list):
        _cb_uuid = inArray[0]
        _db = inArray[1]
        _collection = inArray[2]
        _query_info = inArray[3]
        _updata_info = inArray[4]
        __upsert = inArray[5]
        self.rsp = hub_call_dbproxy_updata_persisted_object_rsp(self.current_ch, _cb_uuid)
        if self.cb_updata_persisted_object:
            self.cb_updata_persisted_object(_db, _collection, _query_info, _updata_info, __upsert)
        self.rsp = None

    def find_and_modify(self, inArray:list):
        _cb_uuid = inArray[0]
        _db = inArray[1]
        _collection = inArray[2]
        _query_info = inArray[3]
        _updata_info = inArray[4]
        __new = inArray[5]
        __upsert = inArray[6]
        self.rsp = hub_call_dbproxy_find_and_modify_rsp(self.current_ch, _cb_uuid)
        if self.cb_find_and_modify:
            self.cb_find_and_modify(_db, _collection, _query_info, _updata_info, __new, __upsert)
        self.rsp = None

    def remove_object(self, inArray:list):
        _cb_uuid = inArray[0]
        _db = inArray[1]
        _collection = inArray[2]
        _query_info = inArray[3]
        self.rsp = hub_call_dbproxy_remove_object_rsp(self.current_ch, _cb_uuid)
        if self.cb_remove_object:
            self.cb_remove_object(_db, _collection, _query_info)
        self.rsp = None

    def get_object_info(self, inArray:list):
        _db = inArray[0]
        _collection = inArray[1]
        _query_info = inArray[2]
        __skip = inArray[3]
        __limit = inArray[4]
        __sort = inArray[5]
        __Ascending_ = inArray[6]
        _callbackid = inArray[7]
        if self.cb_get_object_info:
            self.cb_get_object_info(_db, _collection, _query_info, __skip, __limit, __sort, __Ascending_, _callbackid)

    def get_object_count(self, inArray:list):
        _cb_uuid = inArray[0]
        _db = inArray[1]
        _collection = inArray[2]
        _query_info = inArray[3]
        self.rsp = hub_call_dbproxy_get_object_count_rsp(self.current_ch, _cb_uuid)
        if self.cb_get_object_count:
            self.cb_get_object_count(_db, _collection, _query_info)
        self.rsp = None

class dbproxy_call_hub_module(Imodule):
    def __init__(self, modules:modulemng):
        super(dbproxy_call_hub_module, self)
        self.modules = modules
        self.modules.reg_method("dbproxy_call_hub_ack_get_object_info", [self, self.ack_get_object_info])
        self.modules.reg_method("dbproxy_call_hub_ack_get_object_info_end", [self, self.ack_get_object_info_end])

        self.cb_ack_get_object_info : Callable[[str, bytes]] = None
        self.cb_ack_get_object_info_end : Callable[[str]] = None

    def ack_get_object_info(self, inArray:list):
        _callbackid = inArray[0]
        _object_info = inArray[1]
        if self.cb_ack_get_object_info:
            self.cb_ack_get_object_info(_callbackid, _object_info)

    def ack_get_object_info_end(self, inArray:list):
        _callbackid = inArray[0]
        if self.cb_ack_get_object_info_end:
            self.cb_ack_get_object_info_end(_callbackid)

