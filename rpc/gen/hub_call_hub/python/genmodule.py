#coding:utf-8
# 2020-1-21
# build by qianqians
# genmodule

import uuid
import tools

def gen_module_module(module_name, funcs, dependent_struct, dependent_enum, enum):
    code_constructor = "class " + module_name + "_module(imodule):\n"
    code_constructor += "    def __init__(self, _modulemanager:modulemanager.modulemanager, _hubs:hubmanager.hubmanager):\n"
    code_constructor += "        self.modulemanager = _modulemanager\n"
    code_constructor += "        self.hubs = _hubs\n"
        
    code_constructor_cb = ""
    rsp_code = ""
    code_func = ""
    for i in funcs:
        func_name = i[0]

        func_type = "Callable[["
        count = 0
        for _type, _name, _parameter in i[2]:
            func_type += tools.convert_type(_type, dependent_struct, dependent_enum)
            count += 1
            if count < len(i[2]):
                func_type += ", "
        func_type += "]]"
        
        code_constructor += "        self.on_" + func_name + ":" + func_type + " = None\n"
        code_constructor += "        self.modulemanager.add_mothed(\"" + module_name + "_" + func_name + "\", self." + func_name + ")\n"
            
        if i[1] == "ntf":

            code_func += "    def " + func_name + "(self, inArray:list):\n"
            count = 0 
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code_func += "        _" + _name + " = inArray[" + str(count) + "]\n"
                elif type_ == tools.TypeType.Custom:
                    code_func += "        _" + _name + " = protcol_to_" + _type + "(inArray[" + str(count) + "])\n"
                elif type_ == tools.TypeType.Array:
                    code_func += "        _" + _name + " = []\n"
                    code_func += "        for v_ in inArray[" + str(count) + "]:\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        code_func += "            _" + _name + ".append(v_)\n"
                    elif array_type_ == tools.TypeType.Custom:
                        code_func += "            _" + _name + ".append(protcol_to_" + array_type + "(v_))\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                count += 1
            code_func += "        if self.on_" + func_name + ":\n"
            code_func += "            self.on_" + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += "_" + _name
                count = count + 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ")\n\n"
        elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
            
            code_func += "    def " + func_name + "(self, inArray:list):\n"
            code_func += "        _cb_uuid = inArray[0]\n"
            count = 1 
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code_func += "        _" + _name + " = inArray[" + str(count) + "]\n"
                elif type_ == tools.TypeType.Custom:
                    code_func += "        _" + _name + " = protcol_to_" + _type + "(inArray[" + str(count) + "])\n"
                elif type_ == tools.TypeType.Array:
                    code_func += "        _" + _name + " = []"
                    code_func += "        for v_ in inArray[" + str(count) + "]:\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        code_func += "            _" + _name + ".append(v_)\n"
                    elif array_type_ == tools.TypeType.Custom:
                        code_func += "            _" + _name + ".append(protcol_to_" + array_type + "(v_))\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                count += 1
            code_func += "        self.rsp = " + module_name + "_" + func_name + "_rsp(self.hubs.current_hubproxy.name, _cb_uuid, self.hubs)\n"
            code_func += "        if self.on_" + func_name + ":\n"
            code_func += "            self.on_" + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += "_" + _name
                count = count + 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ")\n"
            code_func += "        self.rsp = None\n\n"

            _hub_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            _rsp_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, func_name)).split('-'))
            rsp_code += "class " + module_name + "_" + func_name + "_rsp(Response):\n"
            rsp_code += "    def __init__(self, hub_name:str, _uuid:int, _hubs:hubmanager.hubmanager):\n"
            rsp_code += "        self.hubs = _hubs\n"
            rsp_code += "        self._hub_name_" + _hub_uuid + " = hub_name\n"
            rsp_code += "        self.uuid_" + _rsp_uuid + " = _uuid\n\n"

            rsp_code += "    def rsp(self, "
            count = 0
            for _type, _name, _parameter in i[4]:
                if _parameter == None:
                    rsp_code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                else:
                    rsp_code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum) + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[4]):
                    rsp_code += ", "
            rsp_code += "):\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "        _argv_" + _argv_uuid + " = [self.uuid_" + _rsp_uuid + "]\n"
            for _type, _name, _parameter in i[4]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    rsp_code += "        _argv_" + _argv_uuid + ".append(" + _name + ")\n"
                elif type_ == tools.TypeType.Custom:
                    rsp_code += "        _argv_" + _argv_uuid + ".append(" + _type + "_to_protcol(" + _name + "))\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "        _array_" + _array_uuid + " = []"
                    _v_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, _name)).split('-'))
                    rsp_code += "        for v_" + _v_uuid + " in " + _name + ":\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        rsp_code += "            _array_" + _array_uuid + ".append(v_" + _v_uuid + ")\n"
                    elif array_type_ == tools.TypeType.Custom:
                        rsp_code += "            _array_" + _array_uuid + ".append(" + array_type + "_to_protcol(v_" + _v_uuid + "))\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    rsp_code += "        _argv_" + _argv_uuid + ".append(_array_" + _array_uuid + ")\n"
            rsp_code += "        self.hubs.call_hub(self._hub_name_" + _hub_uuid + ", \"" + module_name + "_rsp_cb_" + func_name + "_rsp\", _argv_" + _argv_uuid + ")\n\n"

            rsp_code += "    def err(self, "
            count = 0
            for _type, _name, _parameter in i[6]:
                if _parameter == None:
                    rsp_code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                else:
                    rsp_code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum) + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[6]):
                    rsp_code += ", "
            rsp_code += "):\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "        _argv_" + _argv_uuid + " = [self.uuid_" + _rsp_uuid + "]\n"
            for _type, _name, _parameter in i[6]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    rsp_code += "        _argv_" + _argv_uuid + ".append(" + _name + ")\n"
                elif type_ == tools.TypeType.Custom:
                    rsp_code += "        _argv_" + _argv_uuid + ".append(" + _type + "_to_protcol(" + _name + "))\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "        _array_" + _array_uuid + " = []"
                    _v_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, _name)).split('-'))
                    rsp_code += "        for v_" + _v_uuid + " in " + _name + ":\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        rsp_code += "            _array_" + _array_uuid + ".append(v_" + _v_uuid + ")\n"
                    elif array_type_ == tools.TypeType.Custom:
                        rsp_code += "            _array_" + _array_uuid + ".append(" + array_type + "_to_protcol(v_" + _v_uuid + "))\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    rsp_code += "        _argv_" + _argv_uuid + ".append(_array_" + _array_uuid + ")\n"
            rsp_code += "        self.hubs.call_hub(self._hub_name_" + _hub_uuid + ", \"" + module_name + "_rsp_cb_" + func_name + "_err\", _argv_" + _argv_uuid + ")\n\n"

        else:
            raise Exception("func:%s wrong rpc type:%s must req or ntf" % (func_name, str(i[1])))

    code_constructor_end = "\n"
    code = "\n"
        
    return rsp_code + code_constructor + code_constructor_cb + code_constructor_end + code_func + code
        

def genmodule(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    modules = pretreatment.module
        
    code = "#this module code is codegen by abelkhan codegen for python\n"
    for module_name, funcs in modules.items():
        code += gen_module_module(module_name, funcs, dependent_struct, dependent_enum, pretreatment.enum)
                
    return code