#coding:utf-8
# 2020-1-21
# build by qianqians
# gencaller

import uuid
import tools

def gen_module_caller(module_name, funcs, dependent_struct, dependent_enum, enum):
    cb_func = ""

    cb_code = "/*this cb code is codegen by abelkhan for c#*/\n"
    cb_code += "    public class " + module_name + "_rsp_cb : common.imodule {\n"
    cb_code_constructor = "        public " + module_name + "_rsp_cb() \n"
    cb_code_constructor += "        {\n"
    cb_code_constructor += "            hub.hub._modules.add_module(\"" + module_name + "_rsp_cb\", this);\n"
    cb_code_section = ""

    code = "    public class " + module_name + "_caller {\n"
    code += "        public static " + module_name + "_rsp_cb rsp_cb_" + module_name + "_handle = null;\n"
    code += "        private " + module_name + "_clientproxy _clientproxy;\n"
    code += "        private " + module_name + "_multicast _multicast;\n"
    code += "        private " + module_name + "_broadcast _broadcast;\n\n"
    code += "        public " + module_name + "_caller() \n"
    code += "        {\n"
    code += "            if (rsp_cb_" + module_name + "_handle == null)\n            {\n"
    code += "                rsp_cb_" + module_name + "_handle = new " + module_name + "_rsp_cb();\n"
    code += "            }\n\n"
    code += "            _clientproxy = new " + module_name + "_clientproxy(rsp_cb_" + module_name + "_handle);\n"
    code += "            _multicast = new " + module_name + "_multicast(rsp_cb_" + module_name + "_handle);\n"
    code += "            _broadcast = new " + module_name + "_broadcast(rsp_cb_" + module_name + "_handle);\n"
    code += "        }\n\n"
    _client_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, module_name)).split('-'))
    code += "        public " + module_name + "_clientproxy get_client(string client_uuid) {\n"
    code += "            _clientproxy.client_uuid_" + _client_uuid + " = client_uuid;\n"
    code += "            return _clientproxy;\n"
    code += "        }\n\n"
    code += "        public " + module_name + "_multicast get_multicast(List<string> client_uuids) {\n"
    code += "            _multicast.client_uuids_" + _client_uuid + " = client_uuids;\n"
    code += "            return _multicast;\n"
    code += "        }\n\n"
    code += "        public " + module_name + "_broadcast get_broadcast() {\n"
    code += "            return _broadcast;\n"
    code += "        }\n"
    code += "    }\n\n"

    cp_code = "    public class " + module_name + "_clientproxy {\n"
    cp_code += "        public string client_uuid_" + _client_uuid + ";\n"
    _uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, module_name)).split('-'))
    cp_code += "        private Int64 uuid_" + _uuid + " = (Int64)RandomUUID.random();\n\n"
    cp_code += "        public " + module_name + "_rsp_cb rsp_cb_" + module_name + "_handle;\n\n"
    cp_code += "        public " + module_name + "_clientproxy(" + module_name + "_rsp_cb rsp_cb_" + module_name + "_handle_)\n"
    cp_code += "        {\n"
    cp_code += "            rsp_cb_" + module_name + "_handle = rsp_cb_" + module_name + "_handle_;\n"
    cp_code += "        }\n\n"

    cm_code = "    public class " + module_name + "_multicast {\n"
    cm_code += "        public List<string> client_uuids_" + _client_uuid + ";\n"
    cm_code += "        public " + module_name + "_rsp_cb rsp_cb_" + module_name + "_handle;\n\n"
    cm_code += "        public " + module_name + "_multicast(" + module_name + "_rsp_cb rsp_cb_" + module_name + "_handle_)\n"
    cm_code += "        {\n"
    cm_code += "            rsp_cb_" + module_name + "_handle = rsp_cb_" + module_name + "_handle_;\n"
    cm_code += "        }\n\n"

    cbc_code = "    public class " + module_name + "_broadcast {\n"
    cbc_code += "        public " + module_name + "_rsp_cb rsp_cb_" + module_name + "_handle;\n\n"
    cbc_code += "        public " + module_name + "_broadcast(" + module_name + "_rsp_cb rsp_cb_" + module_name + "_handle_)\n"
    cbc_code += "        {\n"
    cbc_code += "            rsp_cb_" + module_name + "_handle = rsp_cb_" + module_name + "_handle_;\n"
    cbc_code += "        }\n\n"

    for i in funcs:
        func_name = i[0]

        if i[1] == "ntf" or i[1] ==  "multicast" or i[1] == "broadcast":
            tmp_code = "        public void " + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                if _parameter == None:
                    tmp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                else:
                    tmp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[2]):
                    tmp_code += ", "
            tmp_code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            tmp_code += "            var _argv_" + _argv_uuid + " = new ArrayList();\n"
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    tmp_code += "            _argv_" + _argv_uuid + ".Add(" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    tmp_code += "            _argv_" + _argv_uuid + ".Add(" + _type + "." + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    tmp_code += "            var _array_" + _array_uuid + " = new ArrayList();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_X500, _name)).split('-'))
                    tmp_code += "            foreach(var v_" + _v_uuid + " in " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        tmp_code += "                _array_" + _array_uuid + ".Add(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        tmp_code += "                _array_" + _array_uuid + ".Add(" + array_type + "." + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    tmp_code += "            }\n"                                                     
                    tmp_code += "            _argv_" + _argv_uuid + ".Add(_array_" + _array_uuid + ");\n"
            if i[1] == "ntf":
                cp_code += tmp_code
                cp_code += "            hub.hub._gates.call_client(client_uuid_" + _client_uuid + ", \"" + module_name + "\", \"" + func_name + "\", _argv_" + _argv_uuid + ");\n"
                cp_code += "        }\n\n"
            elif i[1] == "multicast":
                cm_code += tmp_code
                cm_code += "            hub.hub._gates.call_group_client(client_uuids_" + _client_uuid + ", \"" + module_name + "\", \"" + func_name + "\", _argv_" + _argv_uuid + ");\n"
                cm_code += "        }\n\n"
            elif i[1] == "broadcast":
                cbc_code += tmp_code
                cbc_code += "            hub.hub._gates.call_global_client(\"" + module_name + "\", \"" + func_name + "\", _argv_" + _argv_uuid + ");\n"
                cbc_code += "        }\n\n"
        elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
            cb_func += "    public class " + module_name + "_" + func_name + "_cb\n    {\n"
            cb_func += "        private UInt64 cb_uuid;\n"
            cb_func += "        private " + module_name + "_rsp_cb module_rsp_cb;\n\n"
            cb_func += "        public " + module_name + "_" + func_name + "_cb(UInt64 _cb_uuid, " + module_name + "_rsp_cb _module_rsp_cb)\n"
            cb_func += "        {\n"
            cb_func += "            cb_uuid = _cb_uuid;\n"
            cb_func += "            module_rsp_cb = _module_rsp_cb;\n"
            cb_func += "        }\n\n"
            cb_func += "        public event Action"
            if len(i[4]) > 0:
                cb_func += "<"
            count = 0
            for _type, _name, _parameter in i[4]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count = count + 1
                if count < len(i[4]):
                    cb_func += ", "
            if len(i[4]) > 0:
                cb_func += ">"
            cb_func += " on_" + func_name + "_cb;\n"

            cb_func += "        public event Action"
            if len(i[6]) > 0:
                cb_func += "<"
            count = 0
            for _type, _name, _parameter in i[6]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count = count + 1
                if count < len(i[6]):
                    cb_func += ", "
            if len(i[6]) > 0:
                cb_func += ">"
            cb_func += " on_" + func_name + "_err;\n"

            cb_func += "        public event Action on_" + func_name + "_timeout;\n\n"

            cb_func += "        public " + module_name + "_" + func_name + "_cb callBack(Action"
            if len(i[4]) > 0:
                cb_func += "<"
            count = 0
            for _type, _name, _parameter in i[4]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count = count + 1
                if count < len(i[4]):
                    cb_func += ", "
            if len(i[4]) > 0:
                cb_func += ">"
            cb_func += " cb, Action"
            if len(i[6]) > 0:
                cb_func += "<"
            count = 0
            for _type, _name, _parameter in i[6]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count = count + 1
                if count < len(i[6]):
                    cb_func += ", "
            if len(i[6]) > 0:
                cb_func += ">"
            cb_func += " err)\n        {\n"
            cb_func += "            on_" + func_name + "_cb += cb;\n"
            cb_func += "            on_" + func_name + "_err += err;\n"
            cb_func += "            return this;\n"
            cb_func += "        }\n\n"

            cb_func += "        public void timeout(UInt64 tick, Action timeout_cb)\n        {\n"
            cb_func += "            TinyTimer.add_timer(tick, ()=>{\n"
            cb_func += "                module_rsp_cb." + func_name + "_timeout(cb_uuid);\n"
            cb_func += "            });\n"
            cb_func += "            on_" + func_name + "_timeout += timeout_cb;\n"
            cb_func += "        }\n\n"
            
            cb_func += "        public void call_cb("
            count = 0
            for _type, _name, _parameter in i[4]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                count = count + 1
                if count < len(i[4]):
                    cb_func += ", "
            cb_func += ")\n        {\n"
            cb_func += "            if (on_" + func_name + "_cb != null)\n"
            cb_func += "            {\n"
            cb_func += "                on_" + func_name + "_cb(" 
            count = 0
            for _type, _name, _parameter in i[4]:
                cb_func += _name
                count = count + 1
                if count < len(i[4]):
                    cb_func += ", "
            cb_func += ");\n"
            cb_func += "            }\n"
            cb_func += "        }\n\n"
            
            cb_func += "        public void call_err("
            count = 0
            for _type, _name, _parameter in i[6]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                count = count + 1
                if count < len(i[6]):
                    cb_func += ", "
            cb_func += ")\n        {\n"
            cb_func += "            if (on_" + func_name + "_err != null)\n"
            cb_func += "            {\n"
            cb_func += "                on_" + func_name + "_err(" 
            count = 0
            for _type, _name, _parameter in i[6]:
                cb_func += _name
                count = count + 1
                if count < len(i[6]):
                    cb_func += ", "
            cb_func += ");\n"
            cb_func += "            }\n"
            cb_func += "        }\n\n"

            cb_func += "        public void call_timeout()\n"
            cb_func += "        {\n"
            cb_func += "            if (on_" + func_name + "_timeout != null)\n"
            cb_func += "            {\n"
            cb_func += "                on_" + func_name + "_timeout();\n"
            cb_func += "            }\n"
            cb_func += "        }\n\n"

            cb_func += "    }\n\n"

            cb_code += "        public Dictionary<UInt64, " + module_name + "_" + func_name + "_cb> map_" + func_name + ";\n"
            cb_code_constructor += "            map_" + func_name + " = new Dictionary<UInt64, " + module_name + "_" + func_name + "_cb>();\n"
            cb_code_constructor += "            reg_cb(\"" + func_name + "_rsp\", " + func_name + "_rsp);\n"
            cb_code_constructor += "            reg_cb(\"" + func_name + "_err\", " + func_name + "_err);\n"

            cb_code_section += "        public void " + func_name + "_rsp(ArrayList inArray){\n"
            cb_code_section += "            var uuid = (UInt64)inArray[0];\n"
            count = 1 
            for _type, _name, _parameter in i[4]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                _type_ = tools.convert_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    cb_code_section += "            var _" + _name + " = (" + _type_ + ")inArray[" + str(count) + "];\n"
                elif type_ == tools.TypeType.Custom:
                    cb_code_section += "            var _" + _name + " = " + _type + ".protcol_to_" + _type + "((Hashtable)inArray[" + str(count) + "]);\n"
                elif type_ == tools.TypeType.Array:
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
                    cb_code_section += "            var _" + _name + " = new List<" + _array_type + ">();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    cb_code_section += "            foreach(var v_" + _v_uuid + " in (ArrayList)inArray[" + str(count) + "]){\n"
                    if array_type_ in tools.OriginalTypeList:
                        cb_code_section += "                _" + _name + ".Add((" + _array_type + ")v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        cb_code_section += "                _" + _name + ".Add(" + array_type + ".protcol_to_" + array_type + "((Hashtable)v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    cb_code_section += "            }\n"
                count += 1
            cb_code_section += "            var rsp = try_get_and_del_" + func_name + "_cb(uuid);\n"
            cb_code_section += "            if (rsp != null)\n            {\n"
            cb_code_section += "                rsp.call_cb("
            count = 0
            for _type, _name, _parameter in i[4]:
                cb_code_section += "_" + _name
                count = count + 1
                if count < len(i[4]):
                    cb_code_section += ", "
            cb_code_section += ");\n"
            cb_code_section += "            }\n"
            cb_code_section += "        }\n\n"

            cb_code_section += "        public void " + func_name + "_err(ArrayList inArray){\n"
            cb_code_section += "            var uuid = (UInt64)inArray[0];\n"
            count = 1 
            for _type, _name, _parameter in i[6]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                _type_ = tools.convert_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    cb_code_section += "            var _" + _name + " = (" + _type_ + ")inArray[" + str(count) + "];\n"
                elif type_ == tools.TypeType.Custom:
                    cb_code_section += "            var _" + _name + " = " + _type + ".protcol_to_" + _type + "((Hashtable)inArray[" + str(count) + "]);\n"
                elif type_ == tools.TypeType.Array:
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
                    cb_code_section += "            var _" + _name + " = new List<" + _array_type + ">();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    cb_code_section += "            foreach(var v_" + _v_uuid + " in (ArrayList)inArray[" + str(count) + "]){\n"
                    if array_type_ in tools.OriginalTypeList:
                        cb_code_section += "                _" + _name + ".Add((" + _array_type + ")v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        cb_code_section += "                _" + _name + ".Add(" + array_type + ".protcol_to_" + array_type + "((Hashtable)v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    cb_code_section += "            }\n"
                count += 1
            cb_code_section += "            var rsp = try_get_and_del_" + func_name + "_cb(uuid);\n"
            cb_code_section += "            if (rsp != null)\n            {\n"
            cb_code_section += "                rsp.call_err("
            count = 0
            for _type, _name, _parameter in i[6]:
                cb_code_section += "_" + _name
                count = count + 1
                if count < len(i[6]):
                    cb_code_section += ", "
            cb_code_section += ");\n"
            cb_code_section += "            }\n"
            cb_code_section += "        }\n\n"

            cb_code_section += "        public void " + func_name + "_timeout(UInt64 cb_uuid){\n"
            cb_code_section += "            var rsp = try_get_and_del_" + func_name + "_cb(cb_uuid);\n"
            cb_code_section += "            if (rsp != null){\n"
            cb_code_section += "                rsp.call_timeout();\n"
            cb_code_section += "            }\n"
            cb_code_section += "        }\n\n"

            cb_code_section += "        private " + module_name + "_" + func_name + "_cb try_get_and_del_" + func_name + "_cb(UInt64 uuid){\n"
            cb_code_section += "            lock(map_" + func_name + ")\n"
            cb_code_section += "            {\n"
            cb_code_section += "                var rsp = map_" + func_name + "[uuid];\n"
            cb_code_section += "                map_" + func_name + ".Remove(uuid);\n"
            cb_code_section += "                return rsp;\n"
            cb_code_section += "            }\n"
            cb_code_section += "        }\n\n"

            cp_code += "        public " + module_name + "_" + func_name + "_cb " + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                if _parameter == None:
                    cp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                else:
                    cp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[2]):
                    cp_code += ", "
            cp_code += "){\n"
            _cb_uuid_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, func_name)).split('-'))
            cp_code += "            Interlocked.Increment(ref uuid_" + _uuid + ");\n"
            cp_code += "            var uuid_" + _cb_uuid_uuid + " = (UInt64)uuid_" + _uuid + ";\n\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            cp_code += "            var _argv_" + _argv_uuid + " = new ArrayList();\n"
            cp_code += "            _argv_" + _argv_uuid + ".Add(uuid_" + _cb_uuid_uuid + ");\n"
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    cp_code += "            _argv_" + _argv_uuid + ".Add(" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    cp_code += "            _argv_" + _argv_uuid + ".Add(" + _type + "." + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    cp_code += "            var _array_" + _array_uuid + " = new ArrayList();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_X500, _name)).split('-'))
                    cp_code += "            foreach(var v_" + _v_uuid + " in " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        cp_code += "                _array_" + _array_uuid + ".Add(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        cp_code += "                _array_" + _array_uuid + ".Add(" + array_type + "." + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    cp_code += "            }\n"                                                     
                    cp_code += "            _argv_" + _argv_uuid + ".Add(_array_" + _array_uuid + ");\n"
            cp_code += "            hub.hub._gates.call_client(client_uuid_" + _client_uuid + ", \"" + module_name + "\", \"" + func_name + "\", _argv_" + _argv_uuid + ");\n\n"
            cp_code += "            var cb_" + func_name + "_obj = new " + module_name + "_" + func_name + "_cb(uuid_" + _cb_uuid_uuid + ", rsp_cb_" + module_name + "_handle);\n"
            cp_code += "            rsp_cb_" + module_name + "_handle.map_" + func_name + ".Add(uuid_" + _cb_uuid_uuid + ", cb_" + func_name + "_obj);\n"
            cp_code += "            return cb_" + func_name + "_obj;\n"
            cp_code += "        }\n\n"

        else:
            raise Exception("func:" + func_name + " wrong rpc type:" + str(i[1]) + ", must req or ntf")

    cb_code_constructor += "        }\n\n"
    cb_code_section += "    }\n\n"
    cp_code += "    }\n\n"
    cm_code += "    }\n\n"
    cbc_code += "    }\n\n"

    return cb_func + cb_code + cb_code_constructor + cb_code_section + cp_code + cm_code + cbc_code + code

def gencaller(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    modules = pretreatment.module
    
    code = "/*this caller code is codegen by abelkhan codegen for c#*/\n"
    for module_name, funcs in modules.items():
        code += gen_module_caller(module_name, funcs, dependent_struct, dependent_enum, pretreatment.enum)
        
    return code