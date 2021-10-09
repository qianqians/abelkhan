# 2016-7-1
# build by qianqians
# genmodule

import genenum_node as genenum

def gen_module_module(module_name, funcs):
        rsp_code = ""

        code = "function " + module_name + "(_hub)\n{\n"
        code += "    var event_cb = require(\"event_cb\");\n"
        code += "    event_cb.event_cb.call(this);\n\n"
        code += "    this.module_name = \"" + module_name + "\";\n"
        code += "    this.hub_handle = _hub;\n"
        code += "    _hub.modules.add_module(\"" + module_name + "\", this);\n\n"

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

                        code += "        this.call_event(\"" + func_name + "\", ["
                        count = 0
                        for item in i[2]:
                                code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[2]):
                                        code += ", "
                        code += "]);\n"
                elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
                        code += "    this." + func_name + " = function(uuid"
                        count = 0
                        for item in i[2]:
                                code += ", argv" + str(count)
                                count = count + 1
                        code += ")\n    {\n"

                        code += "        _hub.modules.rsp = new rsp_" + func_name + "(_hub, uuid, _hub.gates.current_client_uuid);\n"

                        code += "        this.call_event(\"" + func_name + "\", ["
                        count = 0
                        for item in i[2]:
                                code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[2]):
                                        code += ", "
                        code += "]);\n"

                        rsp_code += "function rsp_" + func_name + "(_hub, _uuid, _client_uuid)\n{\n"
                        rsp_code += "    this.hub_handle = _hub;\n"
                        rsp_code += "    this.uuid = _uuid;\n"
                        rsp_code += "    this.client_uuid = _client_uuid;\n"

                        rsp_code += "    this.rsp = function("
                        count = 0
                        for item in i[4]:
                                rsp_code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[4]):
                                        rsp_code += ", "
                        rsp_code += ")\n    {\n"
                        rsp_code += "        _hub.gates.call_client(_client_uuid, \"" + module_name + "\", \"" + func_name + "_rsp\", _uuid"
                        count = 0
                        for item in i[4]:
                                rsp_code += ", argv" + str(count)
                                count = count + 1
                        rsp_code += ");\n"
                        rsp_code += "    }\n"

                        rsp_code += "    this.err = function("
                        count = 0
                        for item in i[6]:
                                rsp_code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[6]):
                                        rsp_code += ", "
                        rsp_code += ")\n    {\n"
                        rsp_code += "        _hub.gates.call_client(_client_uuid, \"" + module_name + "\", \"" + func_name + "_err\", _uuid"
                        count = 0
                        for item in i[6]:
                                rsp_code += ", argv" + str(count)
                                count = count + 1
                        rsp_code += ");\n"
                        rsp_code += "    }\n"
                        rsp_code += "}\n"
                        rsp_code += "module.exports.rsp_" + func_name + " = rsp_" + func_name + ";\n\n"
                else:
                        raise "func:%s wrong rpc type:%s must req or ntf" % (func_name, i[1])

                if i[1] == "ntf":
                        pass
                elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
                        code += "        _hub.modules.rsp = null;\n"
                else:
                        raise "func:%s wrong rpc type:%s must req or ntf" % (func_name, i[1])

                code += "    }\n\n"

        code += "}\n"
        code += "module.exports." + module_name + " = " + module_name + ";\n\n"

        return rsp_code + code

def genmodule(file_name, modules, enums):
        head_code = "/*this rsp file is codegen by abelkhan for js*/\n\n"

        module_code = ""
        for module_name, funcs in modules.items():
                module_code += gen_module_module(module_name, funcs)

        enum_code = ""
        for enum_name, enum_key_values in enums.items():
                enum_code += genenum.genenum(enum_name, enum_key_values)

        return head_code + enum_code + module_code
