# 2016-7-1
# build by qianqians
# gencaller

import genenum_node as genenum

def gen_module_caller(module_name, funcs):
        cb_func = ""

        cb_code = "/*this cb code is codegen by abelkhan for js*/\n"
        cb_code += "function cb_" + module_name + "_handle()\n{\n"

        code = "function " + module_name + "(_hub_handle)\n{\n"
        code += "    this.hub_handle = _hub_handle;\n"
        code += "    this.cb_" + module_name + "_handle = new cb_" + module_name + "_handle();\n"
        code += "    _hub_handle.modules.add_module(\"" + module_name + "\", this.cb_" + module_name + "_handle);\n\n"

        code += "    this.get_hub = function(hub_name){\n"
        code += "        return new " + module_name + "_hubproxy(hub_name, _hub_handle, this.cb_" + module_name + "_handle);\n"
        code += "    }\n"
        code += "}\n"
        code += "module.exports." + module_name + " = " + module_name + ";\n\n"

        code += "function " + module_name + "_hubproxy (hub_name, _hub_handle, _" + module_name + "_handle)\n{\n"
        code += "    this.hub_name = hub_name;\n"
        code += "    this.hub_handle = _hub_handle;\n\n"

        for i in funcs:
                func_name = i[0]
                if i[1] == "ntf":
                        code += "    this." + func_name + " = function("
                        count = 0
                        for item in i[2]:
                                code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[2]):
                                        code += ", "
                        code += ")\n    {\n"
                        code += "        _hub_handle.hubs.call_hub(hub_name, \"" + module_name + "\", \"" + func_name + "\""
                        count = 0
                        for item in i[2]:
                                code += ", argv" + str(count)
                                count = count + 1
                        code += ");\n    }\n\n"
                elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
                        code += "    this." + func_name + " = function("
                        count = 0
                        for item in i[2]:
                                code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[2]):
                                        code += ", "
                        code += ")\n    {\n"
                        code += "        const uuidv1 = require('uuid/v1');\n"
                        code += "        var uuid = uuidv1();\n\n"
                        code += "        _hub_handle.hubs.call_hub(hub_name, \"" + module_name + "\", \"" + func_name + "\", _hub_handle.name, uuid"
                        count = 0
                        for item in i[2]:
                                code += ", argv" + str(count)
                                count = count + 1
                        code += ");\n\n"
                        code += "        var cb_" + func_name + "_obj = new cb_" + func_name + "();\n"
                        code += "        _" + module_name + "_handle.map_" + func_name + "[uuid] = cb_" + func_name + "_obj;\n\n"
                        code += "        return cb_" + func_name + "_obj;\n    }\n\n"

                        cb_code += "    this.map_" + func_name + " = {};\n"
                        cb_code += "    this." + func_name + "_rsp = function("
                        cb_code += "uuid"
                        count = 0
                        for item in i[4]:
                                cb_code += ", argv" + str(count)
                                count = count + 1
                                #if count < len(i[4]):
                                #        cb_code += ", "
                        cb_code += ")\n    {\n"
                        cb_code += "        var rsp = this.map_" + func_name + "[uuid];\n"
                        cb_code += "        rsp.cb("
                        count = 0
                        for item in i[4]:
                                cb_code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[4]):
                                        cb_code += ", "
                        cb_code += ");\n"
                        cb_code += "    }\n\n"

                        cb_code += "    this." + func_name + "_err = function("
                        cb_code += "uuid"
                        count = 0
                        for item in i[6]:
                                cb_code += ", argv" + str(count)
                                count = count + 1
                                #if count < len(i[6]):
                                #        cb_code += ", "
                        cb_code += ")\n    {\n"
                        cb_code += "        var rsp = this.map_" + func_name + "[uuid];\n"
                        cb_code += "        rsp.err("
                        count = 0
                        for item in i[6]:
                                cb_code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[6]):
                                        cb_code += ", "
                        cb_code += ");\n"
                        cb_code += "    }\n\n"

                        cb_func += "function cb_" + func_name + "()\n{\n"
                        cb_func += "    this.event_" + func_name + "_handle_cb = null;\n"
                        cb_func += "    this.cb = function("
                        count = 0
                        for item in i[4]:
                                cb_func += "argv" + str(count)
                                count = count + 1
                                if count < len(i[4]):
                                        cb_func += ", "
                        cb_func += ")\n    {\n"
                        cb_func += "        if (this.event_" + func_name + "_handle_cb !== null)\n        {\n"
                        cb_func += "            this.event_" + func_name + "_handle_cb("
                        count = 0
                        for item in i[4]:
                                cb_func += "argv" + str(count)
                                count = count + 1
                                if count < len(i[4]):
                                        cb_func += ", "
                        cb_func += ");\n        }\n"
                        cb_func += "    }\n\n"

                        cb_func += "    this.event_" + func_name + "_handle_err = null;\n"
                        cb_func += "    this.err = function("
                        count = 0
                        for item in i[6]:
                                cb_func += "argv" + str(count)
                                count = count + 1
                                if count < len(i[6]):
                                        cb_func += ", "
                        cb_func += ")\n    {\n"
                        cb_func += "        if (this.event_" + func_name + "_handle_err != null)\n        {\n"
                        cb_func += "            this.event_" + func_name + "_handle_err("
                        count = 0
                        for item in i[6]:
                                cb_func += "argv" + str(count)
                                count = count + 1
                                if count < len(i[6]):
                                        cb_func += ", "
                        cb_func += ");\n        }\n"
                        cb_func += "    }\n\n"

                        cb_func += "    this.callBack = function(cb, err)\n    {\n"
                        cb_func += "        this.event_" + func_name + "_handle_cb = cb;\n"
                        cb_func += "        this.event_" + func_name + "_handle_err = err;\n"
                        cb_func += "    }\n"
                        cb_func += "}\n\n"
                else:
                        raise "func:" + func_name + " wrong rpc type:" + i[1] + ", must req or ntf"

        cb_code += "}\n\n"
        code += "}\n"

        return cb_func + cb_code + code

def gencaller(file_name, modules, enums):
        head_code = "/*this req file is codegen by abelkhan for js*/\n\n"

        module_code = ""
        for module_name, funcs in modules.items():
                module_code += gen_module_caller(module_name, funcs)

        enum_code = ""
        for enum_name, enum_key_values in enums.items():
                enum_code += genenum.genenum(enum_name, enum_key_values)

        return head_code + enum_code + module_code
