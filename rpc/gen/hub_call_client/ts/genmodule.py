#coding:utf-8
# 2020-1-21
# build by qianqians
# genmodule

import uuid
import tools

def gen_module_module(module_name, funcs, dependent_struct, dependent_enum, enum):
    code_constructor = "export class " + module_name + "_module extends client_handle.imodule {\n"
    code_constructor += "    public _client_handle:client_handle.client;\n"
    code_constructor += "    constructor(_client_handle_:client_handle.client){\n"
    code_constructor += "        super();\n"
    code_constructor += "        this._client_handle = _client_handle_;\n"
    code_constructor += "        this._client_handle._modulemng.add_module(\"" + module_name + "\", this);\n\n"
        
    code_constructor_cb = ""
    rsp_code = ""
    code_func = ""
    for i in funcs:
        func_name = i[0]

        if i[1] == "ntf":
            code_constructor += "        this.reg_cb(\"" + func_name + "\", this." + func_name + ".bind(this));\n"
            code_constructor_cb += "        this.cb_" + func_name + " = null;\n"
                
            code_func += "    public cb_" + func_name + " : ("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                count += 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ")=>void | null;\n"

            code_func += "    " + func_name + "(inArray:any[]){\n"
            code_func += "        let _argv_:any[] = [];\n"
            count = 0 
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code_func += "        _argv_.push(inArray[" + str(count) + "]);\n"
                elif type_ == tools.TypeType.Custom:
                    _import = tools.get_import(_type, dependent_struct)
                    if _import == "":
                        code_func += "        _argv_.push(protcol_to_" + _type + "(inArray[" + str(count) + "]));\n"
                    else:
                        code_func += "        _argv_.push(" + _import + ".protcol_to_" + _type + "(inArray[" + str(count) + "]));\n"
                elif type_ == tools.TypeType.Array:
                    code_func += "        let _array_:any[] = [];\n"
                    code_func += "        for(let v_ of inArray[" + str(count) + "]){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        code_func += "            _array_.push(v_);\n"
                    elif array_type_ == tools.TypeType.Custom:
                        _import = tools.get_import(array_type, dependent_struct)
                        if _import == "":
                            code_func += "            _array_.push(protcol_to_" + array_type + "(v_));\n"
                        else:
                            code_func += "            _array_.push(" + _import + ".protcol_to_" + array_type + "(v_));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code_func += "        }\n"                                                     
                    code_func += "        _argv_.push(_array_);\n"
                count += 1
            code_func += "        if (this.cb_" + func_name + "){\n"
            code_func += "            this.cb_" + func_name + ".apply(null, _argv_);\n"
            code_func += "        }\n"
            code_func += "    }\n\n"
        elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
            code_constructor += "        this.reg_cb(\"" + func_name + "\", this." + func_name + ".bind(this));\n"
            code_constructor_cb += "        this.cb_" + func_name + " = null;\n\n"

            code_func += "    public cb_" + func_name + " : ("
            count = 0
            for _type, _name, _parameter in i[2]:
                code_func += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                count += 1
                if count < len(i[2]):
                    code_func += ", "
            code_func += ")=>void | null;\n"

            code_func += "    " + func_name + "(inArray:any[]){\n"
            code_func += "        let _cb_uuid = inArray[0];\n"
            code_func += "        let _argv_:any[] = [];\n"
            count = 1 
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code_func += "        _argv_.push(inArray[" + str(count) + "]);\n"
                elif type_ == tools.TypeType.Custom:
                    _import = tools.get_import(_type, dependent_struct)
                    if _import == "":
                        code_func += "        _argv_.push(protcol_to_" + _type + "(inArray[" + str(count) + "]));\n"
                    else:
                        code_func += "        _argv_.push(" + _import + ".protcol_to_" + _type + "(inArray[" + str(count) + "]));\n"
                elif type_ == tools.TypeType.Array:
                    code_func += "        let _array_:any[] = [];"
                    code_func += "        for(let v_ of inArray[" + str(count) + "]){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        code_func += "            _array_.push(v_);\n"
                    elif array_type_ == tools.TypeType.Custom:
                        _import = tools.get_import(array_type, dependent_struct)
                        if _import == "":
                            code_func += "            _array_.push(protcol_to_" + array_type + "(v_));\n"
                        else:
                            code_func += "            _array_.push(" + _import + ".protcol_to_" + array_type + "(v_));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code_func += "        }\n"                                                     
                    code_func += "        _argv_.push(_array_);\n"
                count += 1
            code_func += "        this.rsp = new " + module_name + "_" + func_name + "_rsp(this._client_handle, this._client_handle.current_hub, _cb_uuid);\n"
            code_func += "        if (this.cb_" + func_name + "){\n"
            code_func += "            this.cb_" + func_name + ".apply(null, _argv_);\n"
            code_func += "        }\n"
            code_func += "        this.rsp = null;\n"
            code_func += "    }\n\n"

            rsp_code += "export class " + module_name + "_" + func_name + "_rsp {\n"
            _rsp_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, func_name)).split('-'))
            rsp_code += "    private uuid_" + _rsp_uuid + " : number;\n"
            _hub_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "    private hub_name_" + _hub_uuid + ":string;\n"
            rsp_code += "    private _client_handle:client_handle.client ;\n\n"
            rsp_code += "    constructor(_client_handle_:client_handle.client, current_hub:string, _uuid:number){\n"
            rsp_code += "        this._client_handle = client_handle_;\n"
            rsp_code += "        this.hub_name_" + _hub_uuid + " = current_hub;\n"
            rsp_code += "        this.uuid_" + _rsp_uuid + " = _uuid;\n"
            rsp_code += "    }\n\n"

            rsp_code += "    public rsp("
            count = 0
            for _type, _name, _parameter in i[4]:
                if _parameter == None:
                    rsp_code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                else:
                    rsp_code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum) + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[4]):
                    rsp_code += ", "
            rsp_code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "        let _argv_" + _argv_uuid + ":any[] = [this.uuid_" + _rsp_uuid + "];\n"
            for _type, _name, _parameter in i[4]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    rsp_code += "        _argv_" + _argv_uuid + ".push(" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    _import = tools.get_import(_type, dependent_struct)
                    if _import == "":
                        rsp_code += "        _argv_" + _argv_uuid + ".push(" + _type + "_to_protcol(" + _name + "));\n"
                    else:
                        rsp_code += "        _argv_" + _argv_uuid + ".push(" + _import + "." + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "        let _array_" + _array_uuid + ":any[] = [];"
                    _v_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, _name)).split('-'))
                    rsp_code += "        for(let v_" + _v_uuid + " of " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        rsp_code += "            _array_" + _array_uuid + ".push(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        _import = tools.get_import(array_type, dependent_struct)
                        if _import == "":
                            rsp_code += "            _array_" + _array_uuid + ".push(" + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                        else:
                            rsp_code += "            _array_" + _array_uuid + ".push(" + _import + "." + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    rsp_code += "        }\n"                                                     
                    rsp_code += "        _argv_" + _argv_uuid + ".push(_array_" + _array_uuid + ");\n"
            rsp_code += "        this._client_handle.call_hub(hub_name_" + _hub_uuid + ", \"" + module_name + "_rsp_cb\", \"" + func_name + "_rsp\", _argv_" + _argv_uuid + ");\n"
            rsp_code += "    }\n\n"

            rsp_code += "    public err("
            count = 0
            for _type, _name, _parameter in i[6]:
                if _parameter == None:
                    rsp_code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                else:
                    rsp_code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum) + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[6]):
                    rsp_code += ", "
            rsp_code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            rsp_code += "        let _argv_" + _argv_uuid + ":any[] = [this.uuid_" + _rsp_uuid + "];\n"
            for _type, _name, _parameter in i[6]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    rsp_code += "        _argv_" + _argv_uuid + ".push(" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    _import = tools.get_import(_type, dependent_struct)
                    if _import == "":
                        rsp_code += "        _argv_" + _argv_uuid + ".push(" + _type + "_to_protcol(" + _name + "));\n"
                    else:
                        rsp_code += "        _argv_" + _argv_uuid + ".push(" + _import + "." + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    rsp_code += "        let _array_" + _array_uuid + ":any[] = [];"
                    _v_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_X500, _name)).split('-'))
                    rsp_code += "        for(let v_" + _v_uuid + " of " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        rsp_code += "            _array_" + _array_uuid + ".push(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        _import = tools.get_import(array_type, dependent_struct)
                        if _import == "":
                            rsp_code += "            _array_" + _array_uuid + ".push(" + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                        else:
                            rsp_code += "            _array_" + _array_uuid + ".push(" + _import + "." + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    rsp_code += "        }\n"                                                     
                    rsp_code += "        _argv_" + _argv_uuid + ".push(_array_" + _array_uuid + ");\n"
            rsp_code += "        this._client_handle.call_hub(hub_name_" + _hub_uuid + ", \"" + module_name + "_rsp_cb\", \"" + func_name + "_err\", _argv_" + _argv_uuid + ");\n"
            rsp_code += "    }\n\n"
            rsp_code += "}\n\n"

        else:
            raise Exception("func:%s wrong rpc type:%s must req or ntf" % (func_name, str(i[1])))

    code_constructor += "\n"
    code_constructor_end = "    }\n\n"
    code = "}\n"
        
    return rsp_code + code_constructor + code_constructor_cb + code_constructor_end + code_func + code
        

def genmodule(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    modules = pretreatment.module
        
    code = "/*this module code is codegen by abelkhan codegen for typescript*/\n"
    for module_name, funcs in modules.items():
        code += gen_module_module(module_name, funcs, dependent_struct, dependent_enum, pretreatment.enum)
                
    return code