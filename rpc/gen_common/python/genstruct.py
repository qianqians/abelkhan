#coding:utf-8
# 2023-3-31
# build by qianqians
# genstruct

import tools
import uuid

def genmainstruct(struct_name, elems, dependent_struct, dependent_enum, enum):
    code = "class " + struct_name + "(object):\n"
    code += "    def __init__(self):\n"
    names = []
    for key, value, parameter in elems:
        if value in names:
            raise Exception("repeat struct elem:%s in struct:%s" % (key, struct_name))
        names.append(value)
        if parameter == None:
            code += "        self." + value + " = " + tools.get_type_default(key, dependent_enum) + "\n"
        else:
            code += "        self." + value + " = " + tools.convert_parameter(key, parameter, dependent_enum, enum) + "\n"
    code += "\n\n"
    return code

def genstructprotocol(struct_name, elems, dependent_struct, dependent_enum):
    code = "def " + struct_name + "_to_protcol(_struct:" + struct_name + "):\n"
    code += "    _protocol = {}\n"
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        if type_ in tools.OriginalTypeList:
            code += "    _protocol[\"" + value + "\"] = _struct." + value + "\n"
        elif type_ == tools.TypeType.Custom:
            code += "    _protocol[\"" + value + "\"] = " + key + "_to_protcol(_struct." + value + ")\n"
        elif type_ == tools.TypeType.Array:
            code += "    if _struct." + value + ":\n"
            code += "        _array_" + value + " = []\n"
            code += "        for v_ in _struct." + value + ":\n"
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            if array_type_ in tools.OriginalTypeList:
                code += "            _array_" + value + ".append(v_)\n"
            elif array_type_ == tools.TypeType.Enum:
                code += "            _array_" + value + ".append(int(v_))\n"
            elif array_type_ == tools.TypeType.Custom:
                code += "            _array_" + value + ".append(" + array_type + "_to_protcol(v_))\n"
            elif array_type_ == tools.TypeType.Array:
                raise Exception("not support nested array:%s in struct:%s" % (key, struct_name))
            code += "        _protocol[\"" + value + "\"] = _array_" + value + "\n"
    code += "    return _protocol\n\n"
    return code

def genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum):
    code = "def protcol_to_" + struct_name + "(_protocol:any):\n"
    code += "    _struct = " + struct_name + "()\n"
    code += "    for key, val in _protocol:\n"
    count = 0
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        _type = tools.convert_type(key, dependent_struct, dependent_enum)
        if count == 0:
            code += "        if key == \"" + value + "\":\n"
        else:
            code += "        elif key == \"" + value + "\":\n"
        if type_ in tools.OriginalTypeList:
            code += "            _struct." + value + " = val\n"
        elif type_ == tools.TypeType.Custom:
            code += "            _struct." + value + " = protcol_to_" + key + "(val)\n"
        elif type_ == tools.TypeType.Array:
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            code += "            _struct." + value + " = []\n"
            code += "            for v_ in val:\n"
            if array_type_ in tools.OriginalTypeList:
                code += "                _struct." + value + ".append(v_)\n"
            elif array_type_ == tools.TypeType.Custom:
                code += "                _struct." + value + ".append(protcol_to_" + array_type + "(v_))\n"
            elif array_type_ == tools.TypeType.Array:
                raise Exception("not support nested array:%s in struct:%s" % (key, struct_name))
        count = count + 1
    code += "    return _struct\n\n"
    return code

def genstruct(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    struct = pretreatment.struct
    
    code = "#this struct code is codegen by abelkhan codegen for python\n"
    for struct_name, elems in struct.items():
        code += genmainstruct(struct_name, elems, dependent_struct, dependent_enum, pretreatment.all_enum)
        code += genstructprotocol(struct_name, elems, dependent_struct, dependent_enum)
        code += genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum)

    return code
