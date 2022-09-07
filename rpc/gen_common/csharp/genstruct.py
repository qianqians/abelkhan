#coding:utf-8
# 2019-12-27
# build by qianqians
# genstruct

import tools
import uuid

def genmainstruct(struct_name, elems, dependent_struct, dependent_enum, enum):
    code = "    public class " + struct_name + "\n    {\n"
    names = []
    for key, value, parameter in elems:
        if value in names:
            raise Exception("repeat struct elem:%s in struct:%s" % (key, struct_name))
        names.append(value)
        if parameter == None:
            code += "        public " + tools.convert_type(key, dependent_struct, dependent_enum) + " " + value + ";\n"
        else:
            code += "        public " + tools.convert_type(key, dependent_struct, dependent_enum) + " " + value + " = " + tools.convert_parameter(key, parameter, dependent_enum, enum) + ";\n"
    return code

def genstructprotocol(struct_name, elems, dependent_struct, dependent_enum):
    code = "        public static MsgPack.MessagePackObjectDictionary " + struct_name + "_to_protcol(" + struct_name + " _struct){\n"
    code += "            var _protocol = new MsgPack.MessagePackObjectDictionary();\n"
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        if type_ in tools.OriginalTypeList:
            code += "            _protocol.Add(\"" + value + "\", _struct." + value + ");\n"
        elif type_ == tools.TypeType.Custom:
            code += "            _protocol.Add(\"" + value + "\", new MsgPack.MessagePackObject(" + key + "." + key + "_to_protcol(_struct." + value + ")));\n"
        elif type_ == tools.TypeType.Array:
            code += "            var _array_" + value + " = new List<MsgPack.MessagePackObject>();\n"
            code += "            foreach(var v_ in _struct." + value + "){\n"
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            if array_type_ in tools.OriginalTypeList:
                code += "                _array_" + value + ".Add(v_);\n"
            elif array_type_ == tools.TypeType.Custom:
                code += "                _array_" + value + ".Add( new MsgPack.MessagePackObject(" + array_type + "." + array_type + "_to_protcol(v_)));\n"
            elif array_type_ == tools.TypeType.Array:
                raise Exception("not support nested array:%s in struct:%s" % (key, struct_name))
            code += "            }\n"
            code += "            _protocol.Add(\"" + value + "\", new MsgPack.MessagePackObject(_array_" + value + "));\n"
    code += "            return _protocol;\n"
    code += "        }\n"
    return code

def genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum):
    code = "        public static " + struct_name + " protcol_to_" + struct_name + "(MsgPack.MessagePackObjectDictionary _protocol){\n"
    _struct_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, struct_name)).split('-'))
    code += "            var _struct" + _struct_uuid + " = new " + struct_name + "();\n"
    code += "            foreach (var i in _protocol){\n"
    count = 0
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        _type_ = tools.convert_type(key, dependent_struct, dependent_enum)
        if count == 0:
            code += "                if (((MsgPack.MessagePackObject)i.Key).AsString() == \"" + value + "\"){\n"
        else:
            code += "                else if (((MsgPack.MessagePackObject)i.Key).AsString() == \"" + value + "\"){\n"
        if type_ == tools.TypeType.Int8:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsSByte();\n"
        elif type_ == tools.TypeType.Int16:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsInt16();\n"
        elif type_ == tools.TypeType.Int32:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsInt32();\n"
        elif type_ == tools.TypeType.Int64:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsInt64();\n"
        elif type_ == tools.TypeType.Uint8:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsByte();\n"
        elif type_ == tools.TypeType.Uint16:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsUInt16();\n"
        elif type_ == tools.TypeType.Uint32:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsUInt32();\n"
        elif type_ == tools.TypeType.Uint64:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsUInt64();\n"
        elif type_ == tools.TypeType.Enum:
            code += "                    _struct" + _struct_uuid + "." + value + " = (" + _type_ + ")((MsgPack.MessagePackObject)i.Value).AsInt32();\n"
        elif type_ == tools.TypeType.Float:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsSingle();\n"
        elif type_ == tools.TypeType.Double:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsDouble();\n"
        elif type_ == tools.TypeType.Bool:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsBoolean();\n"
        elif type_ == tools.TypeType.String:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsString();\n"
        elif type_ == tools.TypeType.Bin:
            code += "                    _struct" + _struct_uuid + "." + value + " = ((MsgPack.MessagePackObject)i.Value).AsBinary();\n"
        elif type_ == tools.TypeType.Custom:
            code += "                    _struct" + _struct_uuid + "." + value + " = " + key + ".protcol_to_" + key + "(((MsgPack.MessagePackObject)i.Value).AsDictionary());\n"
        elif type_ == tools.TypeType.Array:
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
            code += "                    _struct" + _struct_uuid + "." + value + " = new();\n"
            code += "                    var _protocol_array = ((MsgPack.MessagePackObject)i.Value).AsList();\n"
            code += "                    foreach (var v_ in _protocol_array){\n"
            if array_type_ == tools.TypeType.Int8:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsSByte());\n"
            elif array_type_ == tools.TypeType.Int16:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsInt16());\n"
            elif array_type_ == tools.TypeType.Int32:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsInt32());\n"
            elif array_type_ == tools.TypeType.Int64:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsInt64());\n"
            elif array_type_ == tools.TypeType.Uint8:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsByte());\n"
            elif array_type_ == tools.TypeType.Uint16:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsUInt16());\n"
            elif array_type_ == tools.TypeType.Uint32:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsUInt32());\n"
            elif array_type_ == tools.TypeType.Uint64:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsUInt64());\n"
            elif array_type_ == tools.TypeType.Enum:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add((" + _type_ + ")((MsgPack.MessagePackObject)v_).AsInt32());\n"
            elif array_type_ == tools.TypeType.Float:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsSingle());\n"
            elif array_type_ == tools.TypeType.Double:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsDouble());\n"
            elif array_type_ == tools.TypeType.Bool:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsBoolean());\n"
            elif array_type_ == tools.TypeType.String:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsString());\n"
            elif array_type_ == tools.TypeType.Bin:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(((MsgPack.MessagePackObject)v_).AsBinary());\n"
            elif array_type_ == tools.TypeType.Custom:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(" + array_type + ".protcol_to_" + array_type + "(((MsgPack.MessagePackObject)v_).AsDictionary()));\n"
            elif array_type_ == tools.TypeType.Array:
                raise Exception("not support nested array:%s in struct:%s" % (key, struct_name))
            code += "                    }\n"
        code += "                }\n"
        count = count + 1
    code += "            }\n"
    code += "            return _struct" + _struct_uuid + ";\n"
    code += "        }\n"
    return code

def genstruct(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    struct = pretreatment.struct
    
    code = "/*this struct code is codegen by abelkhan codegen for c#*/\n"
    for struct_name, elems in struct.items():
        code += genmainstruct(struct_name, elems, dependent_struct, dependent_enum, pretreatment.enum)
        code += genstructprotocol(struct_name, elems, dependent_struct, dependent_enum)
        code += genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum)
        code += "    }\n\n"

    return code
