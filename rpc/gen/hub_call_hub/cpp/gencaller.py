#coding:utf-8
# 2020-1-21
# build by qianqians
# gencaller

import uuid
import tools

def gen_module_caller(module_name, funcs, dependent_struct, dependent_enum, enum):
    cb_func = ""

    cb_code = "/*this cb code is codegen by abelkhan for cpp*/\n"
    cb_code += "    class " + module_name + "_rsp_cb : public common::imodule, public std::enable_shared_from_this<" + module_name + "_rsp_cb>{\n"
    cb_code += "    public:\n"
    cb_code_constructor = "        " + module_name + "_rsp_cb() \n"
    cb_code_constructor += "        {\n"
    cb_code_constructor += "        }\n\n"
    cb_code_constructor += "        void Init(std::shared_ptr<hub::hub_service> _hub_service){\n"
    cb_code_constructor += "            _hub_service->modules.add_module(\"" + module_name + "_rsp_cb\", std::static_pointer_cast<common::imodule>(shared_from_this()));\n\n"
    cb_code_section = ""

    code = "    class " + module_name + "_caller {\n"
    code += "    private:\n"
    code += "        static std::shared_ptr<" + module_name + "_rsp_cb> rsp_cb_" + module_name + "_handle;\n\n"
    code += "    private:\n"
    _uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, module_name)).split('-'))
    code += "        std::atomic<uint64_t> uuid_" + _uuid + ";\n"
    code += "        std::shared_ptr<" + module_name + "_hubproxy> _hubproxy;\n\n"
    code += "    public:\n"
    code += "        " + module_name + "_caller(std::shared_ptr<hub::hub_service> _hub_service)\n"
    code += "        {\n"
    code += "            if (rsp_cb_" + module_name + "_handle == nullptr){\n"
    code += "                rsp_cb_" + module_name + "_handle = std::make_shared<" + module_name + "_rsp_cb>();\n"
    code += "                rsp_cb_" + module_name + "_handle->Init(_hub_service);\n"
    code += "            }\n"
    code += "            uuid_" + _uuid + ".store(random());\n"
    code += "            _hubproxy = std::make_shared<" + module_name + "_hubproxy>(_hub_service, rsp_cb_" + module_name + "_handle);\n"
    code += "        }\n\n"
    code += "        std::shared_ptr<" + module_name + "_hubproxy> get_hub(std::string hub_name) {\n"
    _hub_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, module_name)).split('-'))
    code += "            _hubproxy->hub_name_" + _hub_uuid + " = hub_name;\n"
    code += "            return _hubproxy;\n"
    code += "        }\n\n"
    cpp_code = "std::shared_ptr<" + module_name + "_rsp_cb> " + module_name + "_caller::rsp_cb_" + module_name + "_handle = nullptr;\n"

    code += "    class " + module_name + "_hubproxy {\n"
    code += "    public:\n"
    code += "        std::string hub_name_" + _hub_uuid + ";\n\n"
    code += "    private:\n"
    code += "        std::shared_ptr<hub::hub_service> _hub_service;\n"
    code += "        std::shared_ptr<" + module_name + "_rsp_cb> rsp_cb_" + module_name + "_handle;\n\n"
    code += "    public:\n"
    code += "        " + module_name + "_hubproxy(std::shared_ptr<hub::hub_service> hub_service_, std::shared_ptr<" + module_name + "_rsp_cb> rsp_cb_" + module_name + "_handle_)\n"
    code += "        {\n"
    code += "            _hub_service = hub_service_;\n"
    code += "            rsp_cb_" + module_name + "_handle = rsp_cb_" + module_name + "_handle_;\n"
    code += "        }\n\n"

    for i in funcs:
        func_name = i[0]

        if i[1] == "ntf":
            code += "        void " + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                if _parameter == None:
                    code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                else:
                    code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[2]):
                    code += ", "
            code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            code += "            msgpack11::MsgPack::array _argv_" + _argv_uuid + ";\n"
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code += "            _argv_" + _argv_uuid + ".push_back(" + _name + ");\n"
                elif type_ == tools.TypeType.Enum:
                    code += "            _argv_" + _argv_uuid + ".push_back((int)" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    code += "            _argv_" + _argv_uuid + ".push_back(" + _type + "::" + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code += "            msgpack11::MsgPack::array _array_" + _array_uuid + ";\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_X500, _name)).split('-'))
                    code += "            for(auto v_" + _v_uuid + " : " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        code += "                _array_" + _array_uuid + ".push_back(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Enum:
                        code += "                _array_" + _array_uuid + ".push_back((int)v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        code += "                _array_" + _array_uuid + ".push_back(" + array_type + "::" + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code += "            }\n"                                                     
                    code += "            _argv_" + _argv_uuid + ".push_back(_array_" + _array_uuid + ");\n"
            code += "            _hub_service->_hubmng->call_hub(hub_name_" + _hub_uuid + ", \"" + module_name + "\", \"" + func_name + "\", _argv_" + _argv_uuid + ");\n"
            code += "        }\n\n"
        elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
            cb_func += "    class " + module_name + "_rsp_cb;\n"
            cb_func += "    class " + module_name + "_" + func_name + "_cb : public std::enable_shared_from_this<" +  module_name + "_" + func_name + "_cb>{\n"
            cb_func += "    private:\n"
            cb_func += "        uint64_t cb_uuid;\n"
            cb_func += "        std::shared_ptr<" + module_name + "_rsp_cb> module_rsp_cb;\n\n"
            cb_func += "    public:\n"
            cb_func += "        " + module_name + "_" + func_name + "_cb(uint64_t _cb_uuid, std::shared_ptr<" + module_name + "_rsp_cb> _module_rsp_cb);\n"
            cpp_code += module_name + "_" + func_name + "_cb::" + module_name + "_" + func_name + "_cb(uint64_t _cb_uuid, std::shared_ptr<" + module_name + "_rsp_cb> _module_rsp_cb) {\n"
            cpp_code += "    cb_uuid = _cb_uuid;\n"
            cpp_code += "    module_rsp_cb = _module_rsp_cb;\n"
            cpp_code += "}\n\n"

            cb_func += "    public:\n"
            cb_func += "        concurrent::signals<void("
            count = 0
            for _type, _name, _parameter in i[4]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count = count + 1
                if count < len(i[4]):
                    cb_func += ", "
            cb_func += ")> sig_" + func_name + "_cb;\n"
            
            cb_func += "        concurrent::signals<void("
            count = 0
            for _type, _name, _parameter in i[6]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum)
                count = count + 1
                if count < len(i[6]):
                    cb_func += ", "
            cb_func += ")> sig_" + func_name + "_err;\n"

            cb_func += "        concurrent::signals<void()> sig_" + func_name + "_timeout;\n\n"
            
            cb_func += "        std::shared_ptr<" + module_name + "_"  + func_name + "_cb> callBack(std::function<void("
            count = 0
            for _type, _name, _parameter in i[4]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                count = count + 1
                if count < len(i[4]):
                    cb_func += ", "
            cb_func += ")> cb, std::function<void("
            count = 0
            for _type, _name, _parameter in i[6]:
                cb_func += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name
                count = count + 1
                if count < len(i[6]):
                    cb_func += ", "
            cb_func += ")> err);\n"
            cpp_code += "std::shared_ptr<" + module_name + "_"  + func_name + "_cb> " + module_name + "_" + func_name + "_cb::callBack(std::function<void("
            count = 0
            for _type, _name, _parameter in i[4]:
                cpp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                count = count + 1
                if count < len(i[4]):
                    cpp_code += ", "
            cpp_code += ")> cb, std::function<void("
            count = 0
            for _type, _name, _parameter in i[6]:
                cpp_code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name
                count = count + 1
                if count < len(i[6]):
                    cpp_code += ", "
            cpp_code += ")> err) {\n"
            cpp_code += "    sig_" + func_name + "_cb.connect(cb);\n"
            cpp_code += "    sig_" + func_name + "_err.connect(err);\n"
            cpp_code += "    return shared_from_this();\n"
            cpp_code += "}\n\n"

            cb_func += "        void timeout(uint64_t tick, std::function<void()> timeout_cb);\n"
            cb_func += "    };\n\n"

            cpp_code += "void " + module_name + "_" + func_name + "_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {\n"
            cpp_code += "    TinyTimer::add_timer(tick, [this](){\n"
            cpp_code += "        module_rsp_cb->" + func_name + "_timeout(cb_uuid);\n"
            cpp_code += "    });\n"
            cpp_code += "    sig_" + func_name + "_timeout.connect(timeout_cb);\n"
            cpp_code += "}\n\n"
            

            cb_code += "        std::mutex mutex_map_" + func_name + ";\n"
            cb_code += "        std::map<uint64_t, std::shared_ptr<" + module_name + "_"  + func_name + "_cb> > map_" + func_name + ";\n"
            cb_code_constructor += "            reg_cb(\"" + func_name + "_rsp\", std::bind(&" + module_name + "_rsp_cb::" + func_name + "_rsp, this, std::placeholders::_1));\n"
            cb_code_constructor += "            reg_cb(\"" + func_name + "_err\", std::bind(&" + module_name + "_rsp_cb::" + func_name + "_err, this, std::placeholders::_1));\n"

            cb_code_section += "        void " + func_name + "_rsp(const msgpack11::MsgPack::array& inArray){\n"
            cb_code_section += "            auto uuid = inArray[0].uint64_value();\n"
            count = 1 
            for _type, _name, _parameter in i[4]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                _type_ = tools.convert_type(_type, dependent_struct, dependent_enum)
                if type_ == tools.TypeType.Int8:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].int8_value();\n"
                elif type_ == tools.TypeType.Int16:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].int16_value();\n"
                elif type_ == tools.TypeType.Int32:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].int32_value();\n"
                elif type_ == tools.TypeType.Int64:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].int64_value();\n"
                elif type_ == tools.TypeType.Uint8:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].uint8_value();\n"
                elif type_ == tools.TypeType.Uint16:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].uint16_value();\n"
                elif type_ == tools.TypeType.Uint32:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].uint32_value();\n"
                elif type_ == tools.TypeType.Uint64:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].uint64_value();\n"
                elif type_ == tools.TypeType.Enum:
                    cb_code_section += "            auto _" + _name + " = (" + _type_ + ")inArray[" + str(count) + "].int32_value();\n"
                elif type_ == tools.TypeType.Float:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].float32_value();\n"
                elif type_ == tools.TypeType.Double:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].float64_value();\n"
                elif type_ == tools.TypeType.Bool:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].bool_value();\n"
                elif type_ == tools.TypeType.String:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].string_value();\n"
                elif type_ == tools.TypeType.Bin:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].binary_items();\n"
                elif type_ == tools.TypeType.Custom:
                    cb_code_section += "            auto _" + _name + " = " + _type + "::protcol_to_" + _type + "(inArray[" + str(count) + "].object_items());\n"
                elif type_ == tools.TypeType.Array:
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
                    cb_code_section += "            std::vector<" + _array_type + "> _" + _name + ";\n"
                    cb_code_section += "            auto _protocol_array = inArray[" + str(count) + "].array_items();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    cb_code_section += "            for(auto it_" + _v_uuid + " : _protocol_array){\n"
                    if array_type_ == tools.TypeType.Int8:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".int8_value());\n"
                    elif array_type_ == tools.TypeType.Int16:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".int16_value());\n"
                    elif array_type_ == tools.TypeType.Int32:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".int32_value());\n"
                    elif array_type_ == tools.TypeType.Int64:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".int64_value());\n"
                    elif array_type_ == tools.TypeType.Uint8:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint8_value());\n"
                    elif array_type_ == tools.TypeType.Uint16:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint16_value());\n"
                    elif array_type_ == tools.TypeType.Uint32:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint32_value());\n"
                    elif array_type_ == tools.TypeType.Uint64:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint64_value());\n"
                    elif array_type_ == tools.TypeType.Enum:
                        cb_code_section += "                _" + _name + ".push_back((" + _array_type + ")it_" + _v_uuid + ".int32_value());\n"
                    elif array_type_ == tools.TypeType.Float:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".float32_value());\n"
                    elif array_type_ == tools.TypeType.Double:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".float64_value());\n"
                    elif array_type_ == tools.TypeType.Bool:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".bool_value());\n"
                    elif array_type_ == tools.TypeType.String:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".string_value());\n"
                    elif array_type_ == tools.TypeType.String:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".binary_items());\n"
                    elif array_type_ == tools.TypeType.Custom:
                        cb_code_section += "                _" + _name + ".push_back(" + array_type + "::protcol_to_" + array_type + "(it_" + _v_uuid + ".object_items()));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    cb_code_section += "            }\n"
                count += 1
            cb_code_section += "            auto rsp = try_get_and_del_" + func_name + "_cb(uuid);\n"
            cb_code_section += "            if (rsp != nullptr){\n"
            cb_code_section += "                rsp->sig_" + func_name + "_cb.emit("
            count = 0
            for _type, _name, _parameter in i[4]:
                cb_code_section += "_" + _name
                count = count + 1
                if count < len(i[4]):
                    cb_code_section += ", "
            cb_code_section += ");\n"
            cb_code_section += "            }\n"
            cb_code_section += "        }\n\n"

            cb_code_section += "        void " + func_name + "_err(const msgpack11::MsgPack::array& inArray){\n"
            cb_code_section += "            auto uuid = inArray[0].uint64_value();\n"
            count = 1 
            for _type, _name, _parameter in i[6]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                _type_ = tools.convert_type(_type, dependent_struct, dependent_enum)
                if type_ == tools.TypeType.Int8:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].int8_value();\n"
                elif type_ == tools.TypeType.Int16:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].int16_value();\n"
                elif type_ == tools.TypeType.Int32:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].int32_value();\n"
                elif type_ == tools.TypeType.Int64:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].int64_value();\n"
                elif type_ == tools.TypeType.Uint8:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].uint8_value();\n"
                elif type_ == tools.TypeType.Uint16:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].uint16_value();\n"
                elif type_ == tools.TypeType.Uint32:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].uint32_value();\n"
                elif type_ == tools.TypeType.Uint64:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].uint64_value();\n"
                elif type_ == tools.TypeType.Enum:
                    cb_code_section += "            auto _" + _name + " = (" + _type_ + ")inArray[" + str(count) + "].int32_value();\n"
                elif type_ == tools.TypeType.Float:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].float32_value();\n"
                elif type_ == tools.TypeType.Double:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].float64_value();\n"
                elif type_ == tools.TypeType.Bool:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].bool_value();\n"
                elif type_ == tools.TypeType.String:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].string_value();\n"
                elif type_ == tools.TypeType.Bin:
                    cb_code_section += "            auto _" + _name + " = inArray[" + str(count) + "].binary_items();\n"
                elif type_ == tools.TypeType.Custom:
                    cb_code_section += "            auto _" + _name + " = " + _type + "::protcol_to_" + _type + "(inArray[" + str(count) + "].object_items());\n"
                elif type_ == tools.TypeType.Array:
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
                    cb_code_section += "            std::vector<" + _array_type + "> _" + _name + ";\n"
                    cb_code_section += "            auto _protocol_array = inArray[" + str(count) + "].array_items();\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    cb_code_section += "            for(auto it_" + _v_uuid + " : _protocol_array){\n"
                    if array_type_ == tools.TypeType.Int8:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".int8_value());\n"
                    elif array_type_ == tools.TypeType.Int16:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".int16_value());\n"
                    elif array_type_ == tools.TypeType.Int32:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".int32_value());\n"
                    elif array_type_ == tools.TypeType.Int64:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".int64_value());\n"
                    elif array_type_ == tools.TypeType.Uint8:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint8_value());\n"
                    elif array_type_ == tools.TypeType.Uint16:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint16_value());\n"
                    elif array_type_ == tools.TypeType.Uint32:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint32_value());\n"
                    elif array_type_ == tools.TypeType.Uint64:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".uint64_value());\n"
                    elif array_type_ == tools.TypeType.Enum:
                        cb_code_section += "                _" + _name + ".push_back((" + _array_type + ")it_" + _v_uuid + ".int32_value());\n"
                    elif array_type_ == tools.TypeType.Float:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".float32_value());\n"
                    elif array_type_ == tools.TypeType.Double:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".float64_value());\n"
                    elif array_type_ == tools.TypeType.Bool:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".bool_value());\n"
                    elif array_type_ == tools.TypeType.String:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".string_value());\n"
                    elif array_type_ == tools.TypeType.String:
                        cb_code_section += "                _" + _name + ".push_back(it_" + _v_uuid + ".binary_items());\n"
                    elif array_type_ == tools.TypeType.Custom:
                        cb_code_section += "                _" + _name + ".push_back(" + array_type + "::protcol_to_" + array_type + "(it_" + _v_uuid + ".object_items()));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    cb_code_section += "            }\n"
                count += 1
            cb_code_section += "            auto rsp = try_get_and_del_" + func_name + "_cb(uuid);\n"
            cb_code_section += "            if (rsp != nullptr){\n"
            cb_code_section += "                rsp->sig_" + func_name + "_err.emit("
            count = 0
            for _type, _name, _parameter in i[6]:
                cb_code_section += "_" + _name
                count = count + 1
                if count < len(i[6]):
                    cb_code_section += ", "
            cb_code_section += ");\n"
            cb_code_section += "            }\n"
            cb_code_section += "        }\n\n"

            cb_code_section += "        void " + func_name + "_timeout(uint64_t cb_uuid){\n"
            cb_code_section += "            auto rsp = try_get_and_del_" + func_name + "_cb(cb_uuid);\n"
            cb_code_section += "            if (rsp != nullptr){\n"
            cb_code_section += "                rsp->sig_" + func_name + "_timeout.emit();\n"
            cb_code_section += "            }\n"
            cb_code_section += "        }\n\n"

            cb_code_section += "        std::shared_ptr<" + module_name + "_"  + func_name + "_cb> try_get_and_del_" + func_name + "_cb(uint64_t uuid){\n"
            cb_code_section += "            std::lock_guard<std::mutex> l(mutex_map_" + func_name + ");\n"
            cb_code_section += "            auto rsp = map_" + func_name + "[uuid];\n"
            cb_code_section += "            map_" + func_name + ".erase(uuid);\n"
            cb_code_section += "            return rsp;\n"
            cb_code_section += "        }\n\n"

            code += "        std::shared_ptr<" + module_name + "_"  + func_name + "_cb> " + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                if _parameter == None:
                    code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name 
                else:
                    code += tools.convert_type(_type, dependent_struct, dependent_enum) + " " + _name + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[2]):
                    code += ", "
            code += "){\n"
            _cb_uuid_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, func_name)).split('-'))
            code += "            auto uuid_" + _cb_uuid_uuid + " = uuid_" + _uuid + "++;\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, func_name)).split('-'))
            code += "            msgpack11::MsgPack::array _argv_" + _argv_uuid + ";\n"
            code += "            _argv_" + _argv_uuid + ".push_back(uuid_" + _cb_uuid_uuid + ");\n"
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code += "            _argv_" + _argv_uuid + ".push_back(" + _name + ");\n"
                elif type_ == tools.TypeType.Enum:
                    code += "            _argv_" + _argv_uuid + ".push_back((int)" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    code += "            _argv_" + _argv_uuid + ".push_back(" + _type + "::" + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code += "            msgpack11::MsgPack::array _array_" + _array_uuid + ";\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_X500, _name)).split('-'))
                    code += "            for(auto v_" + _v_uuid + " : " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        code += "                _array_" + _array_uuid + ".push_back(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Enum:
                        code += "                _array_" + _array_uuid + ".push_back((int)v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        code += "                _array_" + _array_uuid + ".push_back(" + array_type + "::" + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code += "            }\n"                                                     
                    code += "            _argv_" + _argv_uuid + ".push_back(_array_" + _array_uuid + ");\n"
            code += "            _hub_service->_hubmng->call_hub(hub_name_" + _hub_uuid + ", \"" + module_name + "\", \"" + func_name + "\", _argv_" + _argv_uuid + ");\n\n"
            code += "            auto cb_" + func_name + "_obj = std::make_shared<" + module_name + "_"  + func_name + "_cb>(uuid_" + _cb_uuid_uuid + ", rsp_cb_" + module_name + "_handle);\n"
            code += "            std::lock_guard<std::mutex> l(rsp_cb_" + module_name + "_handle->mutex_map_" + func_name + ");\n"
            code += "            rsp_cb_" + module_name + "_handle->map_" + func_name + ".insert(std::make_pair(uuid_" + _cb_uuid_uuid + ", cb_" + func_name + "_obj));\n"
            code += "            return cb_" + func_name + "_obj;\n"
            code += "        }\n\n"

        else:
            raise Exception("func:" + func_name + " wrong rpc type:" + str(i[1]) + ", must req or ntf")

    cb_code_constructor += "        }\n\n"
    cb_code_section += "    };\n\n"
    code += "    };\n"

    h_code = cb_func + cb_code + cb_code_constructor + cb_code_section + code

    return h_code, cpp_code

def gencaller(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    modules = pretreatment.module
    
    h_code = "/*this caller code is codegen by abelkhan codegen for cpp*/\n"
    cpp_cpde = "/*this caller code is codegen by abelkhan codegen for cpp*/\n"
    for module_name, funcs in modules.items():
        h_code_tmp, cpp_cpde_tmp = gen_module_caller(module_name, funcs, dependent_struct, dependent_enum, pretreatment.enum)
        h_code += h_code_tmp
        cpp_cpde += cpp_cpde_tmp

    return h_code, cpp_cpde