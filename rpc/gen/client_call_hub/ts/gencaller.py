#coding:utf-8
# 2020-1-21
# build by qianqians
# gencaller

import uuid
import tools

def gen_module_caller(module_name, funcs, dependent_struct, dependent_enum, enum):
    cb_func = ""

    cb_code = "/*this cb code is codegen by abelkhan for ts*/\n"
    cb_code += "export class " + module_name + "_rsp_cb extends client_handle.imodule {\n"
    cb_code_constructor = "    constructor(modules:client_handle.modulemng){\n"
    cb_code_constructor += "        super();\n"
    cb_code_constructor += "        modules.add_module(\"" + module_name + "_rsp_cb\", this);\n\n"
    cb_code_section = ""

    code = "let rsp_cb_" + module_name + "_handle : " + module_name + "_rsp_cb | null = null;\n"
    code += "export class " + module_name + "_caller {\n"
    code += "    private _hubproxy:" + module_name + "_hubproxy;\n"
    code += "    constructor(_client:client_handle.client){\n"
    code += "        if (rsp_cb_" + module_name + "_handle == null){\n"
    code += "            rsp_cb_" + module_name + "_handle = new " + module_name + "_rsp_cb(_client._modulemng);\n"
    code += "        }\n"
    code += "        this._hubproxy = new " + module_name + "_hubproxy(_client);\n"
    code += "    }\n\n"
    code += "    public get_hub(hub_name:string)\n"
    code += "    {\n"
    _hub_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, module_name)).split('-'))
    code += "        this._hubproxy.hub_name_" + _hub_uuid + " = hub_name;\n"
    code += "        return this._hubproxy;\n"
    code += "    }\n\n"
    code += "}\n\n"

    code += "export class " + module_name + "_hubproxy\n{\n"
    _uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, module_name)).split('-'))
    code += "    private uuid_" + _uuid + " : number = Math.round(Math.random() * 1000);\n\n"
    code += "    public hub_name_" + _hub_uuid + ":string;\n"
    code += "    private _client_handle:client_handle.client;\n\n"
    code += "    constructor(client_handle_:client_handle.client)\n"
    code += "    {\n"
    code += "        this._client_handle = client_handle_;\n"
    code += "    }\n\n"

    for i in funcs:
        func_name = i[0]

        if i[1] == "ntf":
            code += "    public " + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                if _parameter == None:
                    code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                else:
                    code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum) + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[2]):
                    code += ", "
            code += "){\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            code += "        let _argv_" + _argv_uuid + ":any[] = [];\n"
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code += "        _argv_" + _argv_uuid + ".push(" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    _import = tools.get_import(_type, dependent_struct)
                    if _import == "":
                        code += "        _argv_" + _argv_uuid + ".push(" + _type + "_to_protcol(" + _name + "));\n"
                    else:
                        code += "        _argv_" + _argv_uuid + ".push(" + _import + "." + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code += "        let _array_" + _array_uuid + ":any[] = [];\n"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code += "        for(let v_" + _v_uuid + " of " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        code += "            _array_" + _array_uuid + ".push(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        _import = tools.get_import(array_type, dependent_struct)
                        if _import == "":
                            code += "            _array_" + _array_uuid + ".push(" + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                        else:
                            code += "            _array_" + _array_uuid + ".push(" + _import + "." + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code += "        }\n"                                                     
                    code += "        _argv_" + _argv_uuid + ".push(_array_" + _array_uuid + ");\n"
            code += "        this._client_handle.call_hub(this.hub_name_" + _hub_uuid + ", \"" + module_name + "\", \"" + func_name + "\", _argv_" + _argv_uuid + ");\n"
            code += "    }\n\n"
        elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
            rsp_fn = "("
            count = 0
            for _type, _name, _parameter in i[4]:
                rsp_fn += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                count += 1
                if count < len(i[4]):
                    rsp_fn += ", "
            rsp_fn += ")=>void"
            err_fn = "("
            count = 0
            for _type, _name, _parameter in i[6]:
                err_fn += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                count += 1
                if count < len(i[6]):
                    err_fn += ", "
            err_fn += ")=>void"

            cb_func += "export class " + module_name + "_" + func_name + "_cb{\n"
            cb_func += "    private cb_uuid : number;\n"
            cb_func += "    private module_rsp_cb : " + module_name + "_rsp_cb;\n\n"
            cb_func += "    public event_" + func_name + "_handle_cb : " + rsp_fn + " | null;\n"
            cb_func += "    public event_" + func_name + "_handle_err : " + err_fn + " | null;\n"
            cb_func += "    public event_" + func_name + "_handle_timeout : ()=>void | null;\n"
            
            cb_func += "    constructor(_cb_uuid : number, _module_rsp_cb : " + module_name + "_rsp_cb){\n"
            cb_func += "        this.cb_uuid = _cb_uuid;\n"
            cb_func += "        this.module_rsp_cb = _module_rsp_cb;\n"
            cb_func += "        this.event_" + func_name + "_handle_cb = null;\n"
            cb_func += "        this.event_" + func_name + "_handle_err = null;\n"
            cb_func += "        this.event_" + func_name + "_handle_timeout = null;\n"
            cb_func += "    }\n\n"

            cb_func += "    callBack(_cb:" + rsp_fn + ", _err:" + err_fn + ")\n    {\n"
            cb_func += "        this.event_" + func_name + "_handle_cb = _cb;\n"
            cb_func += "        this.event_" + func_name + "_handle_err = _err;\n"
            cb_func += "        return this;\n"
            cb_func += "    }\n\n"

            cb_func += "    timeout(tick:number, timeout_cb:()=>void)\n    {\n"
            cb_func += "        setTimeout(()=>{ this.module_rsp_cb." + func_name + "_timeout(this.cb_uuid); }, tick);\n"
            cb_func += "        this.event_" + func_name + "_handle_timeout = timeout_cb;\n"
            cb_func += "    }\n\n"
            
            cb_func += "}\n\n"

            cb_code += "    public map_" + func_name + ":Map<number, " + module_name + "_" + func_name + "_cb>;\n"
            cb_code_constructor += "        this.map_" + func_name + " = new Map<number, " + module_name + "_" + func_name + "_cb>();\n"
            cb_code_constructor += "        this.reg_cb(\"" + func_name + "_rsp\", this." + func_name + "_rsp.bind(this));\n"
            cb_code_constructor += "        this.reg_cb(\"" + func_name + "_err\", this." + func_name + "_err.bind(this));\n"

            cb_code_section += "    public " + func_name + "_rsp(inArray:any[]){\n"
            cb_code_section += "        let uuid = inArray[0];\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            cb_code_section += "        let _argv_" + _argv_uuid + ":any[] = [];\n"
            count = 1 
            for _type, _name, _parameter in i[4]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    cb_code_section += "        _argv_" + _argv_uuid + ".push(inArray[" + str(count) + "]);\n"
                elif type_ == tools.TypeType.Custom:
                    _import = tools.get_import(_type, dependent_struct)
                    if _import == "":
                        cb_code_section += "        _argv_" + _argv_uuid + ".push(protcol_to_" + _type + "(inArray[" + str(count) + "]));\n"
                    else:
                        cb_code_section += "        _argv_" + _argv_uuid + ".push(" + _import + ".protcol_to_" + _type + "(inArray[" + str(count) + "]));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    cb_code_section += "        let _array_" + _array_uuid + ":any[] = [];"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_X500, _name)).split('-'))
                    cb_code_section += "        for(let v_" + _v_uuid + " of inArray[" + str(count) + "]){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        cb_code_section += "            _array_" + _array_uuid + ".push(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        _import = tools.get_import(array_type, dependent_struct)
                        if _import == "":
                            cb_code_section += "            _array_" + _array_uuid + ".push(protcol_to_" + array_type + "(v_" + _v_uuid + "));\n"
                        else:
                            cb_code_section += "            _array_" + _array_uuid + ".push(" + _import + ".protcol_to_" + array_type + "(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    cb_code_section += "        }\n"                                                     
                    cb_code_section += "        _argv_" + _argv_uuid + ".push(_array_" + _array_uuid + ");\n"
                count += 1
            cb_code_section += "        var rsp = this.try_get_and_del_" + func_name + "_cb(uuid);\n"
            cb_code_section += "        if (rsp && rsp.event_" + func_name + "_handle_cb) {\n"
            cb_code_section += "            rsp.event_" + func_name + "_handle_cb.apply(null, _argv_" + _argv_uuid + ");\n"
            cb_code_section += "        }\n"
            cb_code_section += "    }\n\n"

            cb_code_section += "    public " + func_name + "_err(inArray:any[]){\n"
            cb_code_section += "        let uuid = inArray[0];\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            cb_code_section += "        let _argv_" + _argv_uuid + ":any[] = [];\n"
            count = 1 
            for _type, _name, _parameter in i[6]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    cb_code_section += "        _argv_" + _argv_uuid + ".push(inArray[" + str(count) + "]);\n"
                elif type_ == tools.TypeType.Custom:
                    _import = tools.get_import(_type, dependent_struct)
                    if _import == "":
                        cb_code_section += "        _argv_" + _argv_uuid + ".push(protcol_to_" + _type + "(inArray[" + str(count) + "]));\n"
                    else:
                        cb_code_section += "        _argv_" + _argv_uuid + ".push(" + _import + ".protcol_to_" + _type + "(inArray[" + str(count) + "]));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, _name)).split('-'))
                    cb_code_section += "        let _array_" + _array_uuid + ":any[] = [];"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_X500, _name)).split('-'))
                    cb_code_section += "        for(let v_" + _v_uuid + " of inArray[" + str(count) + "]){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        cb_code_section += "            _array_" + _array_uuid + ".push(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        _import = tools.get_import(array_type, dependent_struct)
                        if _import == "":
                            cb_code_section += "            _array_" + _array_uuid + ".push(protcol_to_" + array_type + "(v_" + _v_uuid + "));\n"
                        else:
                            cb_code_section += "            _array_" + _array_uuid + ".push(" + _import + ".protcol_to_" + array_type + "(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    cb_code_section += "        }\n"                                                     
                    cb_code_section += "        _argv_" + _argv_uuid + ".push(_array_" + _array_uuid + ");\n"
                count += 1
            cb_code_section += "        var rsp = this.try_get_and_del_" + func_name + "_cb(uuid);\n"
            cb_code_section += "        if (rsp && rsp.event_" + func_name + "_handle_err) {\n"
            cb_code_section += "            rsp.event_" + func_name + "_handle_err.apply(null, _argv_" + _argv_uuid + ");\n"
            cb_code_section += "        }\n"
            cb_code_section += "    }\n\n"

            cb_code_section += "    public " + func_name + "_timeout(cb_uuid : number){\n"
            cb_code_section += "        let rsp = this.try_get_and_del_" + func_name + "_cb(cb_uuid);\n"
            cb_code_section += "        if (rsp){\n"
            cb_code_section += "            if (rsp.event_" + func_name + "_handle_timeout) {\n"
            cb_code_section += "                rsp.event_" + func_name + "_handle_timeout.apply(null);\n"
            cb_code_section += "            }\n"
            cb_code_section += "        }\n"
            cb_code_section += "    }\n\n"

            cb_code_section += "    private try_get_and_del_" + func_name + "_cb(uuid : number){\n"
            cb_code_section += "        var rsp = this.map_" + func_name + ".get(uuid);\n"
            cb_code_section += "        this.map_" + func_name + ".delete(uuid);\n"
            cb_code_section += "        return rsp;\n"
            cb_code_section += "    }\n\n"

            code += "    public " + func_name + "("
            count = 0
            for _type, _name, _parameter in i[2]:
                if _parameter == None:
                    code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum)
                else:
                    code += _name + ":" + tools.convert_type(_type, dependent_struct, dependent_enum) + " = " + tools.convert_parameter(_type, _parameter, dependent_enum, enum)
                count = count + 1
                if count < len(i[2]):
                    code += ", "
            code += "){\n"
            _cb_uuid_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_DNS, func_name)).split('-'))
            code += "        let uuid_" + _cb_uuid_uuid + " = Math.round(this.uuid_" + _uuid + "++);\n\n"
            _argv_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, func_name)).split('-'))
            code += "        let _argv_" + _argv_uuid + ":any[] = [uuid_" + _cb_uuid_uuid + "];\n"
            for _type, _name, _parameter in i[2]:
                type_ = tools.check_type(_type, dependent_struct, dependent_enum)
                if type_ in tools.OriginalTypeList:
                    code += "        _argv_" + _argv_uuid + ".push(" + _name + ");\n"
                elif type_ == tools.TypeType.Custom:
                    _import = tools.get_import(_type, dependent_struct)
                    if _import == "":
                        code += "        _argv_" + _argv_uuid + ".push(" + _type + "_to_protcol(" + _name + "));\n"
                    else:
                        code += "        _argv_" + _argv_uuid + ".push(" + _import + "." + _type + "_to_protcol(" + _name + "));\n"
                elif type_ == tools.TypeType.Array:
                    _array_uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, _name)).split('-'))
                    code += "        let _array_" + _array_uuid + ":any[] = [];"
                    _v_uuid = '_'.join(str(uuid.uuid5(uuid.NAMESPACE_X500, _name)).split('-'))
                    code += "        for(let v_" + _v_uuid + " of " + _name + "){\n"
                    array_type = _type[:-2]
                    array_type_ = tools.check_type(array_type, dependent_struct, dependent_enum)
                    if array_type_ in tools.OriginalTypeList:
                        code += "            _array_" + _array_uuid + ".push(v_" + _v_uuid + ");\n"
                    elif array_type_ == tools.TypeType.Custom:
                        _import = tools.get_import(array_type, dependent_struct)
                        if _import == "":
                            code += "            _array_" + _array_uuid + ".push(" + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                        else:
                            code += "            _array_" + _array_uuid + ".push(" + _import + "." + array_type + "_to_protcol(v_" + _v_uuid + "));\n"
                    elif array_type_ == tools.TypeType.Array:
                        raise Exception("not support nested array:%s in func:%s" % (_type, func_name))
                    code += "        }\n"                                                     
                    code += "        _argv_" + _argv_uuid + ".push(_array_" + _array_uuid + ");\n"
            code += "        this._client_handle.call_hub(this.hub_name_" + _hub_uuid + ", \"" + module_name + "\", \"" + func_name + "\", _argv_" + _argv_uuid + ");\n"
            code += "        let cb_" + func_name + "_obj = new " + module_name + "_" + func_name + "_cb(uuid_" + _cb_uuid_uuid + ", rsp_cb_" + module_name + "_handle);\n"
            code += "        if (rsp_cb_" + module_name + "_handle){\n"
            code += "            rsp_cb_" + module_name + "_handle.map_" + func_name + ".set(uuid_" + _cb_uuid_uuid + ", cb_" + func_name + "_obj);\n"
            code += "        }\n"
            code += "        return cb_" + func_name + "_obj;\n"
            code += "    }\n\n"

        else:
            raise Exception("func:" + func_name + " wrong rpc type:" + str(i[1]) + ", must req or ntf")

    cb_code_constructor += "    }\n"
    cb_code_section += "}\n\n"
    code += "}\n"

    return cb_func + cb_code + cb_code_constructor + cb_code_section + code

def gencaller(pretreatment):
    dependent_struct = pretreatment.dependent_struct
    dependent_enum = pretreatment.dependent_enum
    
    modules = pretreatment.module
    
    code = "/*this caller code is codegen by abelkhan codegen for typescript*/\n"
    for module_name, funcs in modules.items():
        code += gen_module_caller(module_name, funcs, dependent_struct, dependent_enum, pretreatment.enum)
        
    return code