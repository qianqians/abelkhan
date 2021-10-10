#coding:utf-8
# 2019-12-26
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
        return parameter
    elif typestr == 'double':
        return parameter
    elif typestr == 'bool':
        return parameter
    elif check_in_dependent(typestr, dependent_enum):
        _import = get_import(typestr, dependent_enum)
        enum_elems = enum[typestr]
        for key, value in enum_elems:
            if key == parameter:
                if _import == "":
                    return typestr + '.' + parameter
                else:
                    return _import + '.' + typestr + '.' + parameter
        raise Exception("parameter:%s not %s member" % (parameter, typestr))
    elif typestr == 'bin':
        str_parameter = "Uint8Array.from(%s)"%parameter
        return str_parameter

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
        return 'number'
    elif typestr == 'int16':
        return 'number'
    elif typestr == 'int32':
        return 'number'
    elif typestr == 'int64':
        return 'number'
    elif typestr == 'uint8':
        return 'number'
    elif typestr == 'uint16':
        return 'number'
    elif typestr == 'uint32':
        return 'number'
    elif typestr == 'uint64':
        return 'number'
    elif typestr == 'string':
        return 'string'
    elif typestr == 'float':
        return 'number'
    elif typestr == 'double':
        return 'number'
    elif typestr == 'bool':
        return 'boolean'
    elif typestr == 'bin':
        return 'Uint8Array'
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
        return array_type+'[]'

    raise Exception("non exist type:%s" % typestr)
    
OriginalTypeList = [TypeType.Enum, TypeType.String, TypeType.Int8, TypeType.Int16, TypeType.Int32, TypeType.Int64,
                    TypeType.Uint8, TypeType.Uint16, TypeType.Uint32, TypeType.Uint64, 
                    TypeType.Float, TypeType.Double, TypeType.Bool, TypeType.Bin]