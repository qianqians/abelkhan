#coding:utf-8
# 2020-1-21
# build by qianqians
# genmodule

import uuid
import tools

def gen_module_module(module_name, funcs, dependent_struct, dependent_enum, enum):
    code_constructor = "    public class " + module_name + "_module : common.imodule {\n"
    code_constructor += "        public client.client _client_handle;\n"
    code_constructor += "        public " + module_name + "_module(client.client client_handle_) \n"
    code_constructor += "        {\n"
    code_constructor += "            _client_handle = client_handle_;\n"
    code_constructor += "            _client_handle.modulemanager.add_module(\"" + module_name + "\", this);\n\n"
        
    code_constructor_cb = ""
    rsp_code = ""
    code_func = ""
    for i in funcs:
        func_name = i[0]

        if i[1] == "ntf" or i[1] ==  "multicast" or i[1] == "broadcast":
            code_constructor += "            reg_cb(\"" + func_name + "\", " + func_name + ");\n"
                
            code_func += "        public event Action"
            if len(i[2]) > 0:
                code_func += "<"
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count += 1
                if count < len(i[2]):
                    code_func += ", "
            if len(i[2]) > 0:
                code_func += ">"
            code_func += " on_" + func_name + ";\n"

            code_func += "        public void " + func_name + "(ArrayList inArray){\n"
            count = 0 
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                _type_ = tools.convert_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code_func += "            var _" + _name + " = (" + _type_ + ")inArray[" + str(count) + "];\n"
                elif type_ == tools.TypeType.Custom:
                    code_func += "            var _" + _name + " = " + _type + ".protcol_to_" + _type + "((Hashtable)inArray[" + str(count) + "]);\n"
                elif type_ == tools.TypeType.Array:
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
                    code_func += "            var _" + _name + " = new List<" + _array_type + ">();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code_func += "            foreach(var v_" + _v_uuid + " in (ArrayList)inArray[" + str(count) + "]){\n"
                    if array_type_ in tools.OriginalTypeList:
                        code_func += "                _" + _name + ".Add((" + _array_type + ")v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        code_func += "                _" + _name + ".Add(" + array_type + ".protcol_to_" + array_type + "((Hashtable)v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code_func += "            }\n"                                                     
                count += 1

            code_func += "            if (on_" + func_name + " != null){\n"
            code_func += "                on_" + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += "_" + _name
                count = count + 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ");\n"
            code_func += "            }\n"
            code_func += "        }\n\n"
        elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
            code_constructor += "            reg_cb(\"" + func_name + "\", " + func_name + ");\n"
            
            code_func += "        public event Action"
            if len(i[2]) > 0:
                code_func += "<"
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count += 1
                if count < len(i[2]):
                    code_func += ", "
            if len(i[2]) > 0:
                code_func += ">"
            code_func += " on_" + func_name + ";\n"
            
            code_func += "        public void " + func_name + "(ArrayList inArray){\n"
            code_func += "            var _cb_uuid = (UInt64)inArray[0];\n"
            count = 1 
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                _type_ = tools.convert_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code_func += "            var _" + _name + " = (" + _type_ + ")inArray[" + str(count) + "];\n"
                elif type_ == tools.TypeType.Custom:
                    code_func += "            var _" + _name + " = " + _type + ".protcol_to_" + _type + "((Hashtable)inArray[" + str(count) + "]);\n"
                elif type_ == tools.TypeType.Array:
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
                    code_func += "            var _" + _name + " = new List<" + _array_type + ">();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code_func += "            foreach(var v_" + _v_uuid + " in (ArrayList)inArray[" + str(count) + "]){\n"
                    if array_type_ in tools.OriginalTypeList:
                        code_func += "                _" + _name + ".Add((" + _array_type + ")v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        code_func += "                _" + _name + ".Add(" + array_type + ".protcol_to_" + array_type + "((Hashtable)v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code_func += "            }\n"                                                     
                count += 1

            code_func += "            rsp = new " + module_name + "_" + func_name + "_rsp(_client_handle, _client_handle.current_hub, _cb_uuid);\n"
            code_func += "            if (on_" + func_name + " != null){\n"
            code_func += "                on_" + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += "_" + _name
                count = count + 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ");\n"
            code_func += "            }\n"
            code_func += "            rsp = null;\n"
            code_func += "        }\n\n"

            rsp_code += "    public class " + module_name + "_" + func_name + "_rsp : common.Response {\n"
            _rsp_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, func_name)).split('-'))
            rsp_code += "        private UInt64 uuid_" + _rsp_uuid + ";\n"
            _hub_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "        private string hub_name_" + _hub_uuid + ";\n"
            rsp_code += "        private client.client _client_handle;\n"
            rsp_code += "        public " + module_name + "_" + func_name + "_rsp(client.client client_handle_, string current_hub, UInt64 _uuid) : base(\"" + module_name + "_rsp_cb\", _ch)\n"
            rsp_code += "        {\n"
            rsp_code += "            _client_handle = client_handle_;\n"
            rsp_code += "            hub_name_" + _hub_uuid + " = current_hub;\n"
            rsp_code += "            uuid_" + _rsp_uuid + " = _uuid;\n"
            rsp_code += "        }\n\n"

            rsp_code += "        public void rsp("
            count = 0
            for _type, _name, _parameter in i[4]:
                _name_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                if _parameter == None:
                    rsp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + "_" + _name_uuid
                else:
                    rsp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + "_" + _name_uuid + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[4]):
                    rsp_code += ", "
            rsp_code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "            var _argv_" + _argv_uuid + " = new ArrayList();\n"
            rsp_code += "            _argv_" + _argv_uuid + ".Add(uuid_" + _rsp_uuid + ");\n"
            for _type, _name, _parameter in i[4]:
                _name_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    rsp_code += "            _argv_" + _argv_uuid + ".Add(" + _name + "_" + _name_uuid + ");\n"
                elif type_ == tools.TypeType.Custom:
                    rsp_code += "            _argv_" + _argv_uuid + ".Add(" + _type + "." + _type + "_to_protcol(" + _name + "_" + _name_uuid + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "            var _array_" + _array_uuid + " = new ArrayList();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "            foreach(var v_" + _v_uuid + " in " + _name + "_" + _name_uuid + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        rsp_code += "                _array_" + _array_uuid + ".Add(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        rsp_code += "                _array_" + _array_uuid + ".Add(" + array_type + "." + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    rsp_code += "            }\n"                                                     
                    rsp_code += "            _argv_" + _argv_uuid + ".Add(_array_" + _array_uuid + ");\n"
            rsp_code += "            _client_handle.call_hub(hub_name_" + _hub_uuid + ", \"" + module_name + "_rsp_cb\", \"" + func_name + "_rsp\", _argv_" + _argv_uuid + ");\n"
            rsp_code += "        }\n\n"

            rsp_code += "        public void err("
            count = 0
            for _type, _name, _parameter in i[6]:
                _name_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                if _parameter == None:
                    rsp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + "_" + _name_uuid
                else:
                    rsp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + "_" + _name_uuid + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[6]):
                    rsp_code += ", "
            rsp_code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "            var _argv_" + _argv_uuid + " = new ArrayList();\n"
            rsp_code += "            _argv_" + _argv_uuid + ".Add(uuid_" + _rsp_uuid + ");\n"
            for _type, _name, _parameter in i[6]:
                _name_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    rsp_code += "            _argv_" + _argv_uuid + ".Add(" + _name + "_" + _name_uuid + ");\n"
                elif type_ == tools.TypeType.Custom:
                    rsp_code += "            _argv_" + _argv_uuid + ".Add(" + _type + "." + _type + "_to_protcol(" + _name + "_" + _name_uuid + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "            var _array_" + _array_uuid + " = new ArrayList();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "            foreach(var v_" + _v_uuid + " in " + _name + "_" + _name_uuid + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        rsp_code += "                _array_" + _array_uuid + ".Add(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        rsp_code += "                _array_" + _array_uuid + ".Add(" + array_type + "." + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    rsp_code += "            }\n"                                                     
                    rsp_code += "            _argv_" + _argv_uuid + ".Add(_array_" + _array_uuid + ");\n"
            rsp_code += "            _client_handle.call_hub(hub_name_" + _hub_uuid + ", \"" + module_name + "_rsp_cb\", \"" + func_name + "_err\", _argv_" + _argv_uuid + ");\n"
            rsp_code += "        }\n\n"
            rsp_code += "    }\n\n"

        else:
            raise Exception("func:%s wrong rpc type:%s must req or ntf" % (func_name, str(i[1])))

    code_constructor_end = "        }\n\n"
    code = "    }\n"
        
    return rsp_code + code_constructor + code_constructor_cb + code_constructor_end + code_func + code
        

def genmodule(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    modules = pretreatment.module
        
    code = "/*this module code is codegen by abelkhan codegen for c#*/\n"
    for module_name, funcs in modules.items():
        code += gen_module_module(module_name, funcs, dependent_struct, dependent_enum, pretreatment.enum)
                
    return code