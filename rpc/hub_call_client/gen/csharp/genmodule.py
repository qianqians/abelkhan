# 2016-7-1
# build by qianqians
# genmodule

import genenum
import tools

def gen_module_module(module_name, funcs):
        code = "    public class " + module_name + "_module : common.imodule\n    {\n"
        code += "        public string module_name;\n\n"

        cb_code_Constructor = "        public " + module_name + "_module(client.client _client)\n"
        cb_code_Constructor += "        {\n"
        cb_code_Constructor += "            module_name = \"" + module_name + "\";\n\n"

        for i in funcs:
                func_name = i[0]

                cb_code_Constructor += "            reg_event(\"" + func_name + "\", " + func_name + ");\n"

                if i[1] != "ntf" and i[1] != "multicast" and i[1] != "broadcast":
                        raise "func:" + func_name + " wrong rpc type:" + i[1] + ", must ntf or broadcast"

                code += "        public delegate void " + func_name + "_handle("
                count = 0
                for item in i[2]:
                        code += tools.gentypetocsharp(item) + " argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                code += ", "
                code += ");\n"
                code += "        public event " + func_name + "_handle on" + func_name + ";\n"
                code += "        void " + func_name + "(ArrayList _event)\n"
                code += "        {\n"
                code += "            if (on" + func_name + " == null)\n"
                code += "            {\n"
                code += "                return;\n"
                code += "            }\n\n"
                count = 0
                for item in i[2]:
                        code += "            var argv" + str(count) + " = ((" + tools.gentypetocsharp(item) + ")_event[" + str(count) + "]);\n"
                        count = count + 1
                code += "\n            on" + func_name + "("
                count = 0
                for item in i[2]:
                        code += "argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                code += ", "
                code += ");\n"
                code += "        }\n\n"

        cb_code_Constructor += "\n            _client.modulemanager.add_module(\"" + module_name + "\", this);\n"
        cb_code_Constructor += "        }\n"

        code += cb_code_Constructor
        code += "    }\n"

        return code

def genmodule(module_name, funcs):
        head_code = "/*this imp file is codegen by abelkhan for c#*/\n\n"
        head_code += "using System;\n"
        head_code += "using System.Collections;\n"
        head_code += "using System.Collections.Generic;\n\n"

        head_code += "using common;\n\n"

        head_code += "namespace imp\n"
        head_code += "{\n"

        end_code = "}\n"

        module_code = ""
        for module_name, funcs in modules.items():
                module_code += gen_module_module(module_name, funcs)

        enum_code = ""
        for enum_name, enum_key_values in enums.items():
                enum_code += genenum.genenum(enum_name, enum_key_values)

        return head_code + enum_code + module_code + end_code
