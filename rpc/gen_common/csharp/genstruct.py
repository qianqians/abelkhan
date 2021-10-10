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
    code = "        public static Hashtable " + struct_name + "_to_protcol(" + struct_name + " _struct){\n"
    code += "            var _protocol = new Hashtable();\n"
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        if type_ in tools.OriginalTypeList:
            code += "            _protocol.Add(\"" + value + "\", _struct." + value + ");\n"
        elif type_ == tools.TypeType.Custom:
            code += "            _protocol.Add(\"" + value + "\", " + key + "." + key + "_to_protcol(_struct." + value + "));\n"
        elif type_ == tools.TypeType.Array:
            code += "            var _array_" + value + " = new ArrayList();"
            code += "            foreach(var v_ in _struct." + value + "){\n"
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            if array_type_ == tools.TypeType.Original:
                code += "                _array_" + value + ".Add(v_);\n"
            elif array_type_ == tools.TypeType.Custom:
                code += "                _array_" + value + ".Add(" + array_type + "." + array_type + "_to_protcol(v_));\n"
            elif array_type_ == tools.TypeType.Array:
                raise Exception("not support nested array:%s in struct:%s" % (key, struct_name))
            code += "            }\n"
            code += "            _protocol.Add(\"" + value + "\", _array_" + value + ");\n"
    code += "            return _protocol;\n"
    code += "        }\n"
    return code

def genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum):
    code = "        public static " + struct_name + " protcol_to_" + struct_name + "(Hashtable _protocol){\n"
    _struct_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, struct_name)).split('-'))
    code += "            var _struct" + _struct_uuid + " = new " + struct_name + "();\n"
    code += "            foreach(DictionaryEntry i in _protocol){\n"
    count = 0
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        _type_ = tools.convert_type(key, dependent_struct, dependent_enum)
        if count == 0:
            code += "                if ((string)i.Key == \"" + value + "\"){\n"
        else:
            code += "                else if ((string)i.Key == \"" + value + "\"){\n"
        if type_ in tools.OriginalTypeList:
            code += "                    _struct" + _struct_uuid + "." + value + " = (" + _type_ + ")i.Value;\n"
        elif type_ == tools.TypeType.Custom:
            code += "                    _struct" + _struct_uuid + "." + value + " = " + key + ".protcol_to_" + key + "(i.Value);\n"
        elif type_ == tools.TypeType.Array:
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            _array_type = tools.convert_type(array_type, dependent_struct, dependent_enum)
            code += "                    foreach(var v_ in i.Value){\n"
            if array_type_ == tools.TypeType.Original:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add((" + _array_type + ")v_);\n"
            elif array_type_ == tools.TypeType.Custom:
                code += "                        _struct" + _struct_uuid + "." + value + ".Add(" + array_type + ".protcol_to_" + array_type + "(v_));\n"
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
