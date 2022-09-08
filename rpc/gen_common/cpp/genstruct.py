#coding:utf-8
# 2019-12-27
# build by qianqians
# genstruct

import tools
import uuid

def genmainstruct(struct_name, elems, dependent_struct, dependent_enum, enum):
    code = "    class " + struct_name + " {\n"
    code += "    public:\n"
    names = []
    for key, value, parameter in elems:
        if value in names:
            raise Exception("repeat struct elem:%s in struct:%s" % (key, struct_name))
        names.append(value)
        if parameter == None:
            code += "        " + tools.convert_type(key, dependent_struct, dependent_enum) + " " + value + ";\n"
        else:
            code += "        " + tools.convert_type(key, dependent_struct, dependent_enum) + " " + value + " = " + tools.convert_parameter(key, parameter, dependent_enum, enum) + ";\n"
    code += "\n"
    return code

def genstructprotocol(struct_name, elems, dependent_struct, dependent_enum):
    code = "    public:\n"
    code += "        static msgpack11::MsgPack::object " + struct_name + "_to_protcol(" + struct_name + " _struct){\n"
    code += "            msgpack11::MsgPack::object _protocol;\n"
    
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        if type_ in tools.OriginalTypeList:
            code += "            _protocol.insert(std::make_pair(\"" + value + "\", _struct." + value + "));\n"
        elif type_ == tools.TypeType.Enum:
            code += "            _protocol.insert(std::make_pair(\"" + value + "\", (int)_struct." + value + "));\n"
        elif type_ == tools.TypeType.Custom:
            code += "            _protocol.insert(std::make_pair(\"" + value + "\", " + key + "::" + key + "_to_protcol(_struct." + value + ")));\n"
        elif type_ == tools.TypeType.Array:
            code += "            msgpack11::MsgPack::array _array_" + value + ";\n"
            code += "            for(var v_ : _struct." + value + "){\n"
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            if array_type_ in tools.OriginalTypeList:
                code += "                _array_" + value + ".push_back(v_);\n"
            elif array_type_ == tools.TypeType.Enum:
                code += "                _array_" + value + ".push_back((int)v_);\n"
            elif array_type_ == tools.TypeType.Custom:
                code += "                _array_" + value + ".push_back(" + array_type + "::" + array_type + "_to_protcol(v_));\n"
            elif array_type_ == tools.TypeType.Array:
                raise Exception("not support nested array:%s in struct:%s" % (key, struct_name))
            code += "            }\n"
            code += "            _protocol.insert(std::make_pair(\"" + value + "\", _array_" + value + ");\n"
    code += "            return _protocol;\n"
    code += "        }\n\n"
    return code

def genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum):
    code = "        static " + struct_name + " protcol_to_" + struct_name + "(const msgpack11::MsgPack::object& _protocol){\n"
    _struct_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, struct_name)).split('-'))
    code += "            " + struct_name + " _struct" + _struct_uuid + ";\n"
    code += "            for(auto i : _protocol){\n"
    count = 0
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        _type_ = tools.convert_type(key, dependent_struct, dependent_enum)
        if count == 0:
            code += "                if (i.first == \"" + value + "\"){\n"
        else:
            code += "                else if (i.first == \"" + value + "\"){\n"
        if type_ == tools.TypeType.Int8:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.int8_value();\n"
        elif type_ == tools.TypeType.Int16:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.int16_value();\n"
        elif type_ == tools.TypeType.Int32:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.int32_value();\n"
        elif type_ == tools.TypeType.Int64:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.int64_value();\n"
        elif type_ == tools.TypeType.Uint8:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.uint8_value();\n"
        elif type_ == tools.TypeType.Uint16:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.uint16_value();\n"
        elif type_ == tools.TypeType.Uint32:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.uint32_value();\n"
        elif type_ == tools.TypeType.Uint64:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.uint64_value();\n"
        elif type_ == tools.TypeType.Enum:
            code += "                    _struct" + _struct_uuid + "." + value + " = (" + _type_ + ")i.second.int32_value();\n"
        elif type_ == tools.TypeType.Float:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.float32_value();\n"
        elif type_ == tools.TypeType.Double:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.float64_value();\n"
        elif type_ == tools.TypeType.Bool:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.bool_value();\n"
        elif type_ == tools.TypeType.String:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.string_value();\n"
        elif type_ == tools.TypeType.Bin:
            code += "                    _struct" + _struct_uuid + "." + value + " = i.second.binary_items();\n"
        elif type_ == tools.TypeType.Custom:
            code += "                    _struct" + _struct_uuid + "." + value + " = " + key + "::protcol_to_" + key + "(i.second.object_items());\n"
        elif type_ == tools.TypeType.Array:
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
            code += "                    auto _protocol_array = i.second.array_items();n"
            code += "                    for(auto it_ : _protocol_array){\n"
            if array_type_ == tools.TypeType.Int8:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.int8_value());\n"
            elif array_type_ == tools.TypeType.Int16:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.int16_value());\n"
            elif array_type_ == tools.TypeType.Int32:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.int32_value());\n"
            elif array_type_ == tools.TypeType.Int64:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.int64_value());\n"
            elif array_type_ == tools.TypeType.Uint8:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.uint8_value());\n"
            elif array_type_ == tools.TypeType.Uint16:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.uint16_value());\n"
            elif array_type_ == tools.TypeType.Uint32:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.uint32_value());\n"
            elif array_type_ == tools.TypeType.Uint64:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.uint64_value());\n"
            elif array_type_ == tools.TypeType.Enum:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back((" + _type_ + ")it_.int32_value());\n"
            elif array_type_ == tools.TypeType.Float:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.float32_value());\n"
            elif array_type_ == tools.TypeType.Double:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.float64_value());\n"
            elif array_type_ == tools.TypeType.Bool:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.bool_value());\n"
            elif array_type_ == tools.TypeType.String:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.string_value());\n"
            elif array_type_ == tools.TypeType.Bin:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(it_.binary_items());\n"
            elif array_type_ == tools.TypeType.Custom:
                code += "                        _struct" + _struct_uuid + "." + value + ".push_back(" + array_type + "::protcol_to_" + array_type + "(it_.array_items()));\n"
            elif array_type_ == tools.TypeType.Array:
                raise Exception("not support nested array:%s in struct:%s" % (key, struct_name))
            code += "            }\n"
        code += "                }\n"
        count += 1
    code += "            }\n"
    code += "            return _struct" + _struct_uuid + ";\n"
    code += "        }\n"
    return code

def sort_struct_dependent(struct, dependent_struct, dependent_enum):
    sorted_struct = []

    for struct_name, elems in struct.items():
        try:
            current_struct_index = sorted_struct.index(struct_name)
        except ValueError:
            sorted_struct.append(struct_name)
            current_struct_index = sorted_struct.index(struct_name)
        for key, value, parameter in elems:
            type_ = tools.check_type(key, dependent_struct, dependent_enum)
            if type_ == tools.TypeType.Custom:
                try:
                    dependent_struct_index = sorted_struct.index(key)
                except ValueError:
                    sorted_struct.insert(current_struct_index, key)

                current_struct_index = sorted_struct.index(struct_name)

                if dependent_struct_index > current_struct_index:
                    raise Exception("cycle dependent %s, %s" % (struct_name, key))

    return sorted_struct


def genstruct(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    struct = pretreatment.struct
    sorted_struct = sort_struct_dependent(struct, dependent_struct, dependent_enum)
    
    code = "/*this struct code is codegen by abelkhan codegen for cpp*/\n"
    for struct_name in sorted_struct:
        elems = struct[struct_name]
        code += genmainstruct(struct_name, elems, dependent_struct, dependent_enum, pretreatment.enum)
        code += genstructprotocol(struct_name, elems, dependent_struct, dependent_enum)
        code += genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum)
        code += "    };\n\n"

    return code
