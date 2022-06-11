#coding:utf-8
# 2020-1-21
# build by qianqians
# genmodule

import uuid
import tools

def gen_module_module(module_name, funcs, dependent_struct, dependent_enum, enum):
    code_constructor = "    class " + module_name + "_module : public common::imodule, public std::enable_shared_from_this<" + module_name + "_module>{\n"
    code_constructor += "    private:\n"
    code_constructor += "        std::shared_ptr<hub::hub_service> hub_handle;\n\n"
    code_constructor += "    public:\n"
    code_constructor += "        " + module_name + "_module()\n"
    code_constructor += "        {\n"
    code_constructor += "        }\n\n"
    code_constructor += "        void Init(std::shared_ptr<hub::hub_service> _hub_service){\n"
    code_constructor += "            hub_handle = _hub_service;\n"
        
    code_constructor_cb = ""
    rsp_code = ""
    code_func = ""
    for i in funcs:
        func_name = i[0]

        if i[1] == "ntf":
            code_constructor += "            _hub_service->modules.add_mothed(\"" + module_name + "_" + func_name + "\", std::bind(&" + module_name + "_module::" + func_name + ", this, std::placeholders::_1));\n"
                
            code_func += "        concurrent::signals<void("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count += 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ")> sig_" + func_name + ";\n"

            code_func += "        void " + func_name + "(const msgpack11::MsgPack::array& inArray){\n"
            count = 0 
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                _type_ = tools.convert_type(_type, dependent_struct, dependent_enum)
                if type_ == tools.TypeType.Int8:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].int8_value();\n"
                elif type_ == tools.TypeType.Int16:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].int16_value();\n"
                elif type_ == tools.TypeType.Int32:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].int32_value();\n"
                elif type_ == tools.TypeType.Int64:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].int64_value();\n"
                elif type_ == tools.TypeType.Uint8:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].uint8_value();\n"
                elif type_ == tools.TypeType.Uint16:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].uint16_value();\n"
                elif type_ == tools.TypeType.Uint32:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].uint32_value();\n"
                elif type_ == tools.TypeType.Uint64:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].uint64_value();\n"
                elif type_ == tools.TypeType.Enum:
                    code_func += "            auto _" + _name + " = (" + _type_ + ")inArray[" + str(count) + "].int32_value();\n"
                elif type_ == tools.TypeType.Float:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].float32_value();\n"
                elif type_ == tools.TypeType.Double:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].float64_value();\n"
                elif type_ == tools.TypeType.Bool:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].bool_value();\n"
                elif type_ == tools.TypeType.String:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].string_value();\n"
                elif type_ == tools.TypeType.Bin:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].binary_items();\n"
                elif type_ == tools.TypeType.Custom:
                    code_func += "            auto _" + _name + " = " + _type + "::protcol_to_" + _type + "(inArray[" + str(count) + "].object_items());\n"
                elif type_ == tools.TypeType.Array:
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
                    code_func += "            std::vector<" + _array_type + "> _" + _name + ";\n"
                    code_func += "            auto _protocol_array" + _name + " = inArray[" + str(count) + "].array_items();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code_func += "            for(auto it_" + _v_uuid + " : _protocol_array" + _name + "){\n"
                    if array_type_ == tools.TypeType.Int8:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".int8_value());\n"
                    elif array_type_ == tools.TypeType.Int16:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".int16_value());\n"
                    elif array_type_ == tools.TypeType.Int32:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".int32_value());\n"
                    elif array_type_ == tools.TypeType.Int64:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".int64_value());\n"
                    elif array_type_ == tools.TypeType.Uint8:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint8_value());\n"
                    elif array_type_ == tools.TypeType.Uint16:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint16_value());\n"
                    elif array_type_ == tools.TypeType.Uint32:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint32_value());\n"
                    elif array_type_ == tools.TypeType.Uint64:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint64_value());\n"
                    elif array_type_ == tools.TypeType.Enum:
                        code_func += "                _" + _name + ".push_back((" + _array_type + ")it_" + _v_uuid + ".int32_value());\n"
                    elif array_type_ == tools.TypeType.Float:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".float32_value());\n"
                    elif array_type_ == tools.TypeType.Double:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".float64_value());\n"
                    elif array_type_ == tools.TypeType.Bool:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".bool_value());\n"
                    elif array_type_ == tools.TypeType.String:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".string_value());\n"
                    elif array_type_ == tools.TypeType.Bin:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".binary_items());\n"
                    elif array_type_ == tools.TypeType.Custom:
                        code_func += "                _" + _name + ".push_back(" + array_type + "::protcol_to_" + array_type + "(it_" + _v_uuid + ".object_items()));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code_func += "            }\n"
                count += 1

            code_func += "            sig_" + func_name + ".emit("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += "_" + _name
                count = count + 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ");\n"
            code_func += "        }\n\n"
        elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
            code_constructor += "            _hub_service->modules.add_mothed(\"" + module_name + "_" + func_name + "\", std::bind(&" + module_name + "_module::" + func_name + ", this, std::placeholders::_1));\n"
            
            code_func += "        concurrent::signals<void("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count += 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ")> sig_" + func_name + ";\n"
            
            code_func += "        void " + func_name + "(const msgpack11::MsgPack::array& inArray){\n"
            code_func += "            auto _cb_uuid = inArray[0].uint64_value();\n"
            count = 1 
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                _type_ = tools.convert_type(_type, dependent_struct, dependent_enum)
                if type_ == tools.TypeType.Int8:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].int8_value();\n"
                elif type_ == tools.TypeType.Int16:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].int16_value();\n"
                elif type_ == tools.TypeType.Int32:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].int32_value();\n"
                elif type_ == tools.TypeType.Int64:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].int64_value();\n"
                elif type_ == tools.TypeType.Uint8:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].uint8_value();\n"
                elif type_ == tools.TypeType.Uint16:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].uint16_value();\n"
                elif type_ == tools.TypeType.Uint32:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].uint32_value();\n"
                elif type_ == tools.TypeType.Uint64:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].uint64_value();\n"
                elif type_ == tools.TypeType.Enum:
                    code_func += "            auto _" + _name + " = (" + _type_ + ")inArray[" + str(count) + "].int32_value();\n"
                elif type_ == tools.TypeType.Float:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].float32_value();\n"
                elif type_ == tools.TypeType.Double:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].float64_value();\n"
                elif type_ == tools.TypeType.Bool:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].bool_value();\n"
                elif type_ == tools.TypeType.String:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].string_value();\n"
                elif type_ == tools.TypeType.Bin:
                    code_func += "            auto _" + _name + " = inArray[" + str(count) + "].binary_items();\n"
                elif type_ == tools.TypeType.Custom:
                    code_func += "            auto _" + _name + " = " + _type + "::protcol_to_" + _type + "(inArray[" + str(count) + "].object_items());\n"
                elif type_ == tools.TypeType.Array:
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
                    code_func += "            std::vector<" + _array_type + "> _" + _name + ";\n"
                    code_func += "            auto _protocol_array" + _name + " = inArray[" + str(count) + "].array_items();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code_func += "            for(auto it_" + _v_uuid + " : _protocol_array" + _name + "){\n"
                    if array_type_ == tools.TypeType.Int8:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".int8_value());\n"
                    elif array_type_ == tools.TypeType.Int16:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".int16_value());\n"
                    elif array_type_ == tools.TypeType.Int32:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".int32_value());\n"
                    elif array_type_ == tools.TypeType.Int64:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".int64_value());\n"
                    elif array_type_ == tools.TypeType.Uint8:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint8_value());\n"
                    elif array_type_ == tools.TypeType.Uint16:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint16_value());\n"
                    elif array_type_ == tools.TypeType.Uint32:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint32_value());\n"
                    elif array_type_ == tools.TypeType.Uint64:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint64_value());\n"
                    elif array_type_ == tools.TypeType.Enum:
                        code_func += "                _" + _name + ".push_back((" + _array_type + ")it_" + _v_uuid + ".int32_value());\n"
                    elif array_type_ == tools.TypeType.Float:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".float32_value());\n"
                    elif array_type_ == tools.TypeType.Double:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".float64_value());\n"
                    elif array_type_ == tools.TypeType.Bool:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".bool_value());\n"
                    elif array_type_ == tools.TypeType.String:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".string_value());\n"
                    elif array_type_ == tools.TypeType.Bin:
                        code_func += "                _" + _name + ".push_back(it_" + _v_uuid + ".binary_items());\n"
                    elif array_type_ == tools.TypeType.Custom:
                        code_func += "                _" + _name + ".push_back(" + array_type + "::protcol_to_" + array_type + "(it_" + _v_uuid + ".object_items()));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code_func += "            }\n"
                count += 1

            code_func += "            rsp = std::make_shared<" + module_name + "_" + func_name + "_rsp>(hub_handle, hub_handle->_gatemng->current_client_cuuid, _cb_uuid);\n"
            code_func += "            sig_" + func_name + ".emit("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += "_" + _name
                count = count + 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ");\n"
            code_func += "            rsp = nullptr;\n"
            code_func += "        }\n\n"

            rsp_code += "    class " + module_name + "_"  + func_name + "_rsp : public common::Response {\n"
            rsp_code += "    private:\n"
            rsp_code += "        std::shared_ptr<hub::hub_service> _hub_handle;\n"
            _client_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "        std::string _client_cuuid_" + _client_uuid + ";\n"
            _rsp_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, func_name)).split('-'))
            rsp_code += "        uint64_t uuid_" + _rsp_uuid + ";\n\n"
            rsp_code += "    public:\n"
            rsp_code += "        " + module_name + "_"  + func_name + "_rsp(std::shared_ptr<hub::hub_service> _hub, std::string client_cuuid, uint64_t _uuid)\n"
            rsp_code += "        {\n"
            rsp_code += "            _hub_handle = _hub;\n"
            rsp_code += "            _client_cuuid_" + _client_uuid + " = client_cuuid;\n"
            rsp_code += "            uuid_" + _rsp_uuid + " = _uuid;\n"
            rsp_code += "        }\n\n"

            rsp_code += "        void rsp("
            count = 0
            for _type, _name, _parameter in i[4]:
                if _parameter == None:
                    rsp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                else:
                    rsp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[4]):
                    rsp_code += ", "
            rsp_code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "            msgpack11::MsgPack::array _argv_" + _argv_uuid + ";\n"
            rsp_code += "            _argv_" + _argv_uuid + ".push_back(uuid_" + _rsp_uuid + ");\n"
            for _type, _name, _parameter in i[4]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    rsp_code += "            _argv_" + _argv_uuid + ".push_back(" + _name + ");\n"
                elif type_ == tools.TypeType.Enum:
                    rsp_code += "            _argv_" + _argv_uuid + ".push_back((int)" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    rsp_code += "            _argv_" + _argv_uuid + ".push_back(" + _type + "::" + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "            msgpack11::MsgPack::array _array_" + _array_uuid + ";\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "            for(auto v_" + _v_uuid + " : " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        rsp_code += "                _array_" + _array_uuid + ".push_back(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Enum:
                        rsp_code += "                _array_" + _array_uuid + ".push_back((int)v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        rsp_code += "                _array_" + _array_uuid + ".push_back(" + array_type + "::" + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    rsp_code += "            }\n"                                                     
                    rsp_code += "            _argv_" + _argv_uuid + ".push_back(_array_" + _array_uuid + ");\n"
            rsp_code += "            _hub_handle->_gatemng->call_client(_client_cuuid_" + _client_uuid + ", \"" + module_name + "_rsp_cb_" + func_name + "_rsp\", _argv_" + _argv_uuid + ");\n"
            rsp_code += "        }\n\n"

            rsp_code += "        void err("
            count = 0
            for _type, _name, _parameter in i[6]:
                if _parameter == None:
                    rsp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                else:
                    rsp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[6]):
                    rsp_code += ", "
            rsp_code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "            msgpack11::MsgPack::array _argv_" + _argv_uuid + ";\n"
            rsp_code += "            _argv_" + _argv_uuid + ".push_back(uuid_" + _rsp_uuid + ");\n"
            for _type, _name, _parameter in i[6]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    rsp_code += "            _argv_" + _argv_uuid + ".push_back(" + _name + ");\n"
                elif type_ == tools.TypeType.Enum:
                    rsp_code += "            _argv_" + _argv_uuid + ".push_back((int)" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    rsp_code += "            _argv_" + _argv_uuid + ".push_back(" + _type + "::" + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "            msgpack11::MsgPack::array _array_" + _array_uuid + ";\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "            for(auto v_" + _v_uuid + " : " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        rsp_code += "                _array_" + _array_uuid + ".push_back(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Enum:
                        rsp_code += "                _array_" + _array_uuid + ".push_back((int)v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        rsp_code += "                _array_" + _array_uuid + ".push_back(" + array_type + "::" + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    rsp_code += "            }\n"                                                     
                    rsp_code += "            _argv_" + _argv_uuid + ".push_back(_array_" + _array_uuid + ");\n"
            rsp_code += "            _hub_handle->_gatemng->call_client(_client_cuuid_" + _client_uuid + ", \"" + module_name + "_rsp_cb_" + func_name + "_err\", _argv_" + _argv_uuid + ");\n"
            rsp_code += "        }\n\n"
            rsp_code += "    };\n\n"

        else:
            raise Exception("func:%s wrong rpc type:%s must req or ntf" % (func_name, str(i[1])))

    code_constructor_end = "        }\n\n"
    code = "    };\n"
        
    return rsp_code + code_constructor + code_constructor_cb + code_constructor_end + code_func + code
        

def genmodule(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    modules = pretreatment.module
        
    code = "/*this module code is codegen by abelkhan codegen for cpp*/\n"
    for module_name, funcs in modules.items():
        code += gen_module_module(module_name, funcs, dependent_struct, dependent_enum, pretreatment.enum)
                
    return code