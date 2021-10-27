#coding:utf-8
# 2019-12-27
# build by qianqians
# genstruct

import tools
import uuid

def genmainstruct(struct_name, elems, dependent_struct, dependent_enum, enum):
    code = "export class " + struct_name + "\n{\n"
    names = []
    for key, value, parameter in elems:
        if value in names:
            raise Exception("repeat struct elem:%s in struct:%s" % (key, struct_name))
        names.append(value)
        if parameter == None:
            code += "    public " + value + " : " + tools.convert_type(key, dependent_struct, dependent_enum) + ";\n"
        else:
            code += "    public " + value + " : " + tools.convert_type(key, dependent_struct, dependent_enum) + " = " + tools.convert_parameter(key, parameter, dependent_enum, enum) + ";\n"
    code += "\n    constructor(){\n    }\n" 
    code += "}\n\n"
    return code

def genstructprotocol(struct_name, elems, dependent_struct, dependent_enum):
    code = "export function " + struct_name + "_to_protcol(_struct:" + struct_name + "){\n"
    code += "    return _struct;\n"
    code += "}\n\n"
    return code

def genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum):
    code = "export function protcol_to_" + struct_name + "(_protocol:any){\n"
    code += "    let _struct = new " + struct_name + "();\n"
    code += "    for (const [key, val] of Object.entries(_protocol))"
    count = 0
    for key, value, parameter in elems:
        type_ = tools.check_type(key, dependent_struct, dependent_enum)
        _type = tools.convert_type(key, dependent_struct, dependent_enum)
        if count == 0:
            code += "        if (key === \"" + value + "\"){\n"
        else:
            code += "        else if (key === \"" + value + "\"){\n"
        if type_ in tools.OriginalTypeList:
            code += "            _struct." + value + " = val as " + _type + ";\n"
        elif type_ == tools.TypeType.Custom:
            _import = tools.get_import(key, dependent_struct)
            if _import == "":
                code += "            _struct." + value + " = protcol_to_" + key + "(val);\n"
            else:
                code += "            _struct." + value + " = " + _import + ".protcol_to_" + key + "(val);\n"
        elif type_ == tools.TypeType.Array:
            array_type = key[:-2]
            array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
            code += "            _struct." + value + " = [];\n"
            code += "            for(let v_ of val){\n"
            if array_type_ in tools.OriginalTypeList:
                code += "                _struct." + value + ".push(v_);\n"
            elif array_type_ == tools.TypeType.Custom:
                _import = tools.get_import(array_type, dependent_struct)
                if _import == "":
                    code += "                _struct." + value + ".push(protcol_to_" + array_type + "(v_));\n"
                else:
                    code += "                _struct." + value + ".push(" + _import + ".protcol_to_" + array_type + "(v_));\n"
            elif array_type_ == tools.TypeType.Array:
                raise Exception("not support nested array:%s in struct:%s" % (key, struct_name))
            code += "    }\n"
        code += "        }\n"
        count = count + 1
    code += "    return _struct;\n"
    code += "}\n\n"
    return code

def genstruct(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    struct = pretreatment.struct
    
    code = "/*this struct code is codegen by abelkhan codegen for typescript*/\n"
    for struct_name, elems in struct.items():
        code += genmainstruct(struct_name, elems, dependent_struct, dependent_enum, pretreatment.enum)
        code += genstructprotocol(struct_name, elems, dependent_struct, dependent_enum)
        code += genprotocolstruct(struct_name, elems, dependent_struct, dependent_enum)

    return code
