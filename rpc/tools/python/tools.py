#coding:utf-8
# 2023-3-31
# build by qianqians
# tools

class TypeType():
    Enum = 0
    Custom = 1
    Array = 2
    String = 3
    Int8 = 4
    Int16 = 5
    Int32 = 6
    Int64 = 7
    Uint8 = 8
    Uint16 = 9
    Uint32 = 10
    Uint64 = 11
    Float = 12
    Double = 13
    Bool = 14
    Bin = 15

def convert_parameter(typestr, parameter, dependent_enum, enum):
    if typestr == 'int8':
        return parameter
    elif typestr == 'int16':
        return parameter
    elif typestr == 'int32':
        return parameter
    elif typestr == 'int64':
        return parameter
    elif typestr == 'uint8':
        return parameter
    elif typestr == 'uint16':
        return parameter
    elif typestr == 'uint32':
        return parameter
    elif typestr == 'uint64':
        return parameter
    elif typestr == 'string':
        return parameter
    elif typestr == 'float':
        return "float(" + parameter + ")"
    elif typestr == 'double':
        return "float(" + parameter + ")"
    elif typestr == 'bool':
        if parameter == "false":
            return "False"
        elif parameter == "true":
            return "True"
        raise Exception("wrong parameter:%s" % parameter)
    elif check_in_dependent(typestr, dependent_enum):
        enum_elems = enum[typestr]
        for key, value in enum_elems:
            if key == parameter:
                return typestr + '.' + parameter
        raise Exception("parameter:%s not %s member" % (parameter, typestr))
    elif typestr == 'bin':
        value = parameter[1:-1]
        str_parameter = "bytes([%s])"%value
        return str_parameter

def get_type_default(typestr, dependent_enum):
    if typestr == 'int8':
        return "0"
    elif typestr == 'int16':
        return "0"
    elif typestr == 'int32':
        return "0"
    elif typestr == 'int64':
        return "0"
    elif typestr == 'uint8':
        return "0"
    elif typestr == 'uint16':
        return "0"
    elif typestr == 'uint32':
        return "0"
    elif typestr == 'uint64':
        return "0"
    elif typestr == 'string':
        return ""
    elif typestr == 'float':
        return "0.0"
    elif typestr == 'double':
        return "0.0"
    elif typestr == 'bool':
        return "False"
    elif check_in_dependent(typestr, dependent_enum):
        return "0"
    elif typestr == 'bin':
        return "None"
    else:
        return "None"

def check_in_dependent(typestr, dependent):
    for _type, _import in dependent:
        if _type == typestr:
            return True
    return False

def get_import(typestr, dependent):
    for _type, _import in dependent:
        if _type == typestr:
            return _import
    return ""

def check_type(typestr, dependent_struct, dependent_enum):
    if typestr == 'int8':
        return TypeType.Int8
    elif typestr == 'int16':
        return TypeType.Int16
    elif typestr == 'int32':
        return TypeType.Int32
    elif typestr == 'int64':
        return TypeType.Int64
    elif typestr == 'uint8':
        return TypeType.Uint8
    elif typestr == 'uint16':
        return TypeType.Uint16
    elif typestr == 'uint32':
        return TypeType.Uint32
    elif typestr == 'uint64':
        return TypeType.Uint64
    elif typestr == 'string':
        return TypeType.String
    elif typestr == 'float':
        return TypeType.Float
    elif typestr == 'double':
        return TypeType.Double
    elif typestr == 'bool':
        return TypeType.Bool
    elif typestr == 'bin':
        return TypeType.Bin
    elif check_in_dependent(typestr, dependent_struct):
        return TypeType.Custom
    elif check_in_dependent(typestr, dependent_enum):
        return TypeType.Enum
    elif typestr[len(typestr)-2] == '[' and typestr[len(typestr)-1] == ']':
        return TypeType.Array

    raise Exception("non exist type:%s" % typestr)

def convert_type(typestr, dependent_struct, dependent_enum):
    if typestr == 'int8':
        return 'int'
    elif typestr == 'int16':
        return 'int'
    elif typestr == 'int32':
        return 'int'
    elif typestr == 'int64':
        return 'int'
    elif typestr == 'uint8':
        return 'int'
    elif typestr == 'uint16':
        return 'int'
    elif typestr == 'uint32':
        return 'int'
    elif typestr == 'uint64':
        return 'int'
    elif typestr == 'string':
        return 'str'
    elif typestr == 'float':
        return 'float'
    elif typestr == 'double':
        return 'float'
    elif typestr == 'bool':
        return 'bool'
    elif typestr == 'bin':
        return 'bytes'
    elif check_in_dependent(typestr, dependent_struct):
        _import = get_import(typestr, dependent_struct)
        if _import == "":
            return typestr
        else:
            return _import + "." + typestr
    elif check_in_dependent(typestr, dependent_enum):
        _import = get_import(typestr, dependent_enum)
        if _import == "":
            return typestr
        else:
            return _import + "." + typestr
    elif typestr[len(typestr)-2] == '[' and typestr[len(typestr)-1] == ']':
        array_type = typestr[:-2]
        array_type = convert_type(array_type, dependent_struct, dependent_enum)
        return 'list[' + array_type+']'

    raise Exception("non exist type:%s" % typestr)
    

OriginalTypeList = [TypeType.Enum, TypeType.String, TypeType.Int8, TypeType.Int16, TypeType.Int32, TypeType.Int64,
                    TypeType.Uint8, TypeType.Uint16, TypeType.Uint32, TypeType.Uint64, 
                    TypeType.Float, TypeType.Double, TypeType.Bool, TypeType.Bin]