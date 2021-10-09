# 2016-7-1
# build by qianqians
# gencaller

import genenum
import tools

def gen_module_caller(module_name, funcs):
        code = "    public class " + module_name + "\n"
        code += "    {\n"
        code += "        public " + module_name + "()\n"
        code += "        {\n"
        code += "        }\n\n"
        code += "        public " + module_name + "_cliproxy get_client(string uuid)\n"
        code += "        {\n"
        code += "            return new " + module_name + "_cliproxy(uuid);\n"
        code += "        }\n\n"
        code += "        public " + module_name + "_cliproxy_multi get_multicast(ArrayList uuids)\n"
        code += "        {\n"
        code += "            return new " + module_name + "_cliproxy_multi(uuids);\n"
        code += "        }\n\n"
        code += "        public " + module_name + "_broadcast get_broadcast()\n"
        code += "        {\n"
        code += "            return new " + module_name + "_broadcast();\n"
        code += "        }\n\n"
        code += "    }\n\n"

        cp_code = "    public class " + module_name + "_cliproxy\n"
        cp_code += "    {\n"
        cp_code += "        private string uuid;\n\n"
        cp_code += "        public " + module_name + "_cliproxy(string _uuid)\n"
        cp_code += "        {\n"
        cp_code += "            uuid = _uuid;\n"
        cp_code += "        }\n\n"

        cm_code = "    public class " + module_name + "_cliproxy_multi\n"
        cm_code += "    {\n"
        cm_code += "        private ArrayList uuids;\n\n"
        cm_code += "        public " + module_name + "_cliproxy_multi(ArrayList _uuids)\n"
        cm_code += "        {\n"
        cm_code += "            uuids = _uuids;\n"
        cm_code += "        }\n\n"

        cb_code = "    public class " + module_name + "_broadcast\n"
        cb_code += "    {\n"
        cb_code += "        public " + module_name + "_broadcast()\n"
        cb_code += "        {\n"
        cb_code += "        }\n\n"

        for i in funcs:
                func_name = i[0]

                if i[1] != "ntf" and i[1] != "multicast" and i[1] != "broadcast":
                        raise "func:" + func_name + " wrong rpc type:" + i[1] + ", must ntf or broadcast"

                tmp_code = "        public void " + func_name + "("
                count = 0
                for item in i[2]:
                        tmp_code += tools.gentypetocsharp(item) + " argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                tmp_code += ", "
                tmp_code += ")\n"
                tmp_code += "        {\n"

                argvs = ""
                count = 0
                for item in i[2]:
                        argvs += ", argv" + str(count)
                        count = count + 1

                if i[1] == "ntf":
                        cp_code += tmp_code
                        cp_code += "            hub.hub.gates.call_client(uuid, \"" + module_name + "\", \"" + func_name + "\"" + argvs + ");\n"
                        cp_code += "        }\n\n"
                elif i[1] == "multicast":
                        cm_code += tmp_code
                        cm_code += "            hub.hub.gates.call_group_client(uuids, \"" + module_name + "\", \"" + func_name + "\"" + argvs + ");\n"
                        cm_code += "        }\n\n"
                elif i[1] == "broadcast":
                        cb_code += tmp_code
                        cb_code += "            hub.hub.gates.call_global_client(\"" + module_name + "\", \"" + func_name + "\"" + argvs + ");\n"
                        cb_code += "        }\n\n"

        cp_code += "    }\n\n"
        cm_code += "    }\n\n"
        cb_code += "    }\n\n"

        return code + cp_code + cm_code + cb_code

def gencaller(file_name,  modules, enums):
        head_code = "/*this caller file is codegen by abelkhan for c#*/\n"
        head_code += "using System;\n"
        head_code += "using System.Collections;\n"
        head_code += "using System.IO;\n\n"

        head_code += "namespace abelkhan_code_gen\n"
        head_code += "{\n"

        end_code = "}\n"

        module_code = ""
        for module_name, funcs in modules.items():
                module_code += gen_module_caller(module_name, funcs)

        enum_code = ""
        for enum_name, enum_key_values in enums.items():
                enum_code += genenum.genenum(enum_name, enum_key_values)

        return head_code + enum_code + module_code + end_code
