#coding:utf-8
# 2014-12-18
# build by qianqians
# structmachine

def parameter_check(type_str, parameter_str):
    try:
        if type_str == "int8":
            return parameter_int8_check(parameter_str)
        elif type_str == "int16":
            return parameter_int16_check(parameter_str)
        elif type_str == "int32":
            return parameter_int32_check(parameter_str)
        elif type_str == "int64":
            return parameter_int64_check(parameter_str)
        elif type_str == "uint8":
            return parameter_uint8_check(parameter_str)
        elif type_str == "uint16":
            return parameter_uint16_check(parameter_str)
        elif type_str == "uint32":
            return parameter_uint32_check(parameter_str)
        elif type_str == "uint64":
            return parameter_uint64_check(parameter_str)
        elif type_str == "float":
            return parameter_float_check(parameter_str)
        elif type_str == "double":
            return parameter_double_check(parameter_str)
        elif type_str == "bool":
            return parameter_bool_check(parameter_str)
        elif type_str == "string":
            return parameter_string_check(parameter_str)
        elif type_str == "bin":
            return parameter_bin_check(parameter_str)
    except:
        return False
    
    return True

def parameter_int8_check(parameter_str):
    p = int(parameter_str)
    return p >= -128 and p <= 127

def parameter_int16_check(parameter_str):
    p = int(parameter_str)
    return p >= -32768 and p <= 32767

def parameter_int32_check(parameter_str):
    p = int(parameter_str)
    return p >= -2147483648 and p <= 2147483647

def parameter_int64_check(parameter_str):
    return True

def parameter_uint8_check(parameter_str):
    p = int(parameter_str)
    return p >= 0 and p <= 255

def parameter_uint16_check(parameter_str):
    p = int(parameter_str)
    return p >= 0 and p <= 65535

def parameter_uint32_check(parameter_str):
    p = int(parameter_str)
    return p >= 0 and p <= 4294967295

def parameter_uint64_check(parameter_str):
    return True

def parameter_float_check(parameter_str):
    p = float(parameter_str)
    return True

def parameter_double_check(parameter_str):
    p = float(parameter_str)
    return True

def parameter_bool_check(parameter_str):
    if parameter_str == "true" or parameter_str == "false":
        return True
    return False

def parameter_string_check(parameter_str):
    if parameter_str[0] == "\"" and parameter_str[-1] == "\"":
        return True
    return False

def parameter_bin_check(parameter_str):
    if parameter_str[0] == "[" and parameter_str[-1] == "]":
        v_list = eval(parameter_str)
        for v in v_list:
            if v < 0 or v > 255:
                return False
        return True
    return False
