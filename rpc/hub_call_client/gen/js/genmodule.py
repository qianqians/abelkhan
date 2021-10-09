# 2018-3-16
# build by qianqians
# genmodule

import genenum_browser as genenum

def gen_module_module(module_name, funcs):
        code = "function " + module_name + "(_client){\n"
        code += "    event_cb.call(this);\n\n"
        code += "    this.client_handle = _client;\n"
        code += "    _client.modules.add_module(\"" + module_name + "\", this);\n\n"

        for i in funcs:
                func_name = i[0]

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

                code += "    }\n\n"

        code += "}\n"

        return code

def genmodule(file_name, modules, enums):
        code = "/*this imp file is codegen by abelkhan for js*/\n"

        module_code = ""
        for module_name, funcs in modules.items():
                module_code += gen_module_module(module_name, funcs)

        enum_code = ""
        for enum_name, enum_key_values in enums.items():
                enum_code += genenum.genenum(enum_name, enum_key_values)

        return code + enum_code + module_code
