# 2018-3-16
# build by qianqians
# genmodule

import genenum_node as genenum

def gen_module_caller(module_name, funcs):
        code = "function " + module_name + "(hub_ptr){\n"
        code += "    this.get_client = function(uuid){\n"
        code += "        return new " + module_name + "_cliproxy(uuid, hub_ptr);\n"
        code += "    }\n"
        code += "    this.get_multicast = function(uuids){\n"
        code += "        return new " + module_name + "_cliproxy_multi(uuids, hub_ptr);\n"
        code += "    }\n"
        code += "    this.get_broadcast = function(){\n"
        code += "        return new " + module_name + "_broadcast(hub_ptr);\n"
        code += "    }\n"
        code += "}\n"
        code += "module.exports." + module_name + " = " + module_name + ";\n"

        cp_code = "function " + module_name + "_cliproxy(uuid, hub_ptr){\n"
        cp_code += "    this.uuid = uuid;\n\n"

        cm_code = "function " + module_name + "_cliproxy_multi(uuids, hub_ptr){\n"
        cm_code += "    this.uuids = uuids;\n"

        cb_code = "function " + module_name + "_broadcast(hub_ptr){\n"

        for i in funcs:
                func_name = i[0]

                if i[1] != "ntf" and i[1] != "multicast" and i[1] != "broadcast":
                        raise "func:" + func_name + " wrong rpc type:" + i[1] + ", must ntf or broadcast"

                argvs1 = ""
                count = 0
                for item in i[2]:
                        argvs1 += "argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                            argvs1 += ", "

                argvs = ""
                count = 0
                for item in i[2]:
                        argvs += ", argv" + str(count)
                        count = count + 1

                tmp_code = "    this." + func_name + " = function("
                tmp_code += argvs1
                tmp_code += "){\n"

                if i[1] == "ntf":
                        cp_code += tmp_code
                        cp_code += "        hub_ptr.gates.call_client(uuid, \"" + module_name + "\", \"" + func_name + "\"" + argvs + ");\n"
                        cp_code += "    }\n"
                elif i[1] == "multicast":
                        cm_code += tmp_code
                        cm_code += "        hub_ptr.gates.call_group_client(uuids, \"" + module_name + "\", \"" + func_name + "\"" + argvs + ");\n"
                        cm_code += "    }\n"
                elif i[1] == "broadcast":
                        cb_code += tmp_code
                        cb_code += "        hub_ptr.gates.call_global_client(\"" + module_name + "\", \"" + func_name + "\"" + argvs + ");\n"
                        cb_code += "    }\n"

        cp_code += "}\n"
        code += "module.exports." + module_name + "_cliproxy = " + module_name + "_cliproxy;\n"
        cm_code += "}\n"
        code += "module.exports." + module_name + "_cliproxy_multi = " + module_name + "_cliproxy_multi;\n"
        cb_code += "}\n"
        code += "module.exports." + module_name + "_broadcast = " + module_name + "_broadcast;\n\n"

        return cp_code + cm_code + cb_code + code

def gencaller(file_name, modules, enums):
        code = "/*this ntf file is codegen by ablekhan for js*/\n\n"

        module_code = ""
        for module_name, funcs in modules.items():
                module_code += gen_module_caller(module_name, funcs)

        enum_code = ""
        for enum_name, enum_key_values in enums.items():
                enum_code += genenum.genenum(enum_name, enum_key_values)

        return code + enum_code + module_code
