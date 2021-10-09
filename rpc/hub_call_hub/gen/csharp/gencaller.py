# 2016-7-1
# build by qianqians
# gencaller

import genenum
import tools

def gen_module_caller(module_name, funcs):
        cb_func = ""

        cb_code = "    /*this cb code is codegen by abelkhan for c#*/\n"
        cb_code += "    public class cb_" + module_name + " : common.imodule\n    {\n"

        code = "    public class " + module_name + "\n"
        code += "    {\n"
        code += "        private cb_" + module_name + " cb_" + module_name + "_handle;\n\n"
        code += "        public " + module_name + "()\n"
        code += "        {\n"
        code += "            cb_" + module_name + "_handle = new cb_" + module_name + "();\n"
        code += "            hub.hub.modules.add_module(\"" + module_name + "\", cb_" + module_name + "_handle);\n"
        code += "        }\n\n"
        code += "        public " + module_name + "_hubproxy get_hub(string hub_name)\n"
        code += "        {\n"
        code += "            return new " + module_name + "_hubproxy(hub_name, cb_" + module_name + "_handle);\n"
        code += "        }\n\n"
        code += "    }\n\n"

        code += "    public class " + module_name + "_hubproxy\n"
        code += "    {\n"
        code += "        public cb_" + module_name + " cb_" + module_name + "_handle;\n"
        code += "        public string hub_name;\n\n"
        code += "        public " + module_name + "_hubproxy(string _hub_name, cb_" + module_name + " _cb_" + module_name + "_handle)\n"
        code += "        {\n"
        code += "            cb_" + module_name + "_handle = _cb_" + module_name + "_handle;\n"
        code += "            hub_name = _hub_name;\n"
        code += "        }\n\n"

        for i in funcs:
                func_name = i[0]
                if i[1] == "ntf":
                        code += "        public void " + func_name + "("
                        count = 0
                        for item in i[2]:
                                code += tools.gentypetocsharp(item) + " argv" + str(count)
                                count = count + 1
                                if count < len(i[2]):
                                        code += ", "
                        code += ")\n        {\n"
                        code += "            hub.hub.hubs.call_hub(hub_name, \"" + module_name + "\", \"" + func_name + "\","
                        count = 0
                        for item in i[2]:
                                code += " argv" + str(count)
                                count = count + 1
                                if count < len(i[2]):
                                        code += ", "
                        code += ");\n        }\n\n"
                elif i[1] == "req" and i[3] == "rsp" and i[5] == "err":
                        code += "        public cb_" + func_name + "_func " + func_name + "("
                        count = 0
                        for item in i[2]:
                                code += tools.gentypetocsharp(item) + " argv" + str(count)
                                count = count + 1
                                if count < len(i[2]):
                                        code += ", "
                        code += ")\n        {\n"
                        code += "            var uuid = System.Guid.NewGuid().ToString();\n"
                        code += "            hub.hub.hubs.call_hub(hub_name, \"" + module_name + "\", \"" + func_name + "\", hub.hub.name, uuid"
                        count = 0
                        for item in i[2]:
                                code += ", argv" + str(count)
                                count = count + 1
                        code += ");\n\n"
                        code += "            var cb_" + func_name + "_obj = new cb_" + func_name + "_func();\n"
                        code += "            cb_" + module_name + "_handle.map_" + func_name + ".Add(uuid, cb_" + func_name + "_obj);\n\n"
                        code += "            return cb_" + func_name + "_obj;\n        }\n\n"
                        cb_code += "        public Hashtable map_" + func_name + " = new Hashtable();\n"
                        cb_code += "        public void " + func_name + "_rsp("
                        cb_code += "string uuid"
                        count = 0
                        for item in i[4]:
                                cb_code += ", " + tools.gentypetocsharp(item) + " argv" + str(count)
                                count = count + 1
                        cb_code += ")\n        {\n"
                        cb_code += "            var rsp = (cb_" + func_name + "_func)map_" + func_name + "[uuid];\n"
                        cb_code += "            rsp.cb("
                        count = 0
                        for item in i[4]:
                                cb_code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[4]):
                                        cb_code += ", "
                        cb_code += ");\n"
                        cb_code += "        }\n\n"
                        cb_code += "        public void " + func_name + "_err("
                        cb_code += "string uuid"
                        count = 0
                        for item in i[6]:
                                cb_code += ", " + tools.gentypetocsharp(item) + " argv" + str(count)
                                count = count + 1
                        cb_code += ")\n        {\n"
                        cb_code += "            var rsp = (cb_" + func_name + "_func)map_" + func_name + "[uuid];\n"
                        cb_code += "            rsp.err("
                        count = 0
                        for item in i[6]:
                                cb_code += "argv" + str(count)
                                count = count + 1
                                if count < len(i[6]):
                                        cb_code += ", "
                        cb_code += ");\n"
                        cb_code += "        }\n\n"
                        cb_func += "    public class cb_" + func_name + "_func\n    {\n"
                        cb_func += "        public delegate void " + func_name + "_handle_cb();\n"
                        cb_func += "        public event " + func_name + "_handle_cb on" + func_name + "_cb;\n"
                        cb_func += "        public void cb("
                        count = 0
                        for item in i[4]:
                                cb_func += tools.gentypetocsharp(item) + " argv" + str(count)
                                count = count + 1
                                if count < len(i[4]):
                                        cb_func += ", "
                        cb_func += ")\n        {\n"
                        cb_func += "            if (on" + func_name + "_cb != null)\n            {\n"
                        cb_func += "                on" + func_name + "_cb("
                        count = 0
                        for item in i[4]:
                                cb_func += "argv" + str(count)
                                count = count + 1
                                if count < len(i[4]):
                                        cb_func += ", "
                        cb_func += ");\n            }\n"
                        cb_func += "        }\n\n"
                        cb_func += "        public delegate void " + func_name + "_handle_err();\n"
                        cb_func += "        public event " + func_name + "_handle_err on" + func_name + "_err;\n"
                        cb_func += "        public void err("
                        count = 0
                        for item in i[6]:
                                cb_func += tools.gentypetocsharp(item) + " argv" + str(count)
                                count = count + 1
                                if count < len(i[6]):
                                        cb_func += ", "
                        cb_func += ")\n        {\n"
                        cb_func += "            if (on" + func_name + "_err != null)\n            {\n"
                        cb_func += "                on" + func_name + "_err("
                        count = 0
                        for item in i[6]:
                                cb_func += "argv" + str(count)
                                count = count + 1
                                if count < len(i[6]):
                                        cb_func += ", "
                        cb_func += ");\n            }\n"
                        cb_func += "        }\n\n"
                        cb_func += "        public void callBack(" + func_name + "_handle_cb cb, " + func_name + "_handle_err err)\n        {\n"
                        cb_func += "            on" + func_name + "_cb += cb;\n"
                        cb_func += "            on" + func_name + "_err += err;\n"
                        cb_func += "        }\n\n"
                        cb_func += "    }\n\n"
                else:
                        raise "func:" + func_name + " wrong rpc type:" + i[1] + ", must req or ntf"

        cb_code += "    }\n\n"
        code += "    }\n"

        return cb_func + cb_code + code

def gencaller(file_name, modules, enums):
        head_code = "/*this req file is codegen by abelkhan for c#*/\n"
        head_code += "using System;\n"
        head_code += "using System.Collections;\n"
        head_code += "using System.IO;\n\n"

        head_code += "namespace req\n"
        head_code += "{\n"

        end_code = "}\n"

        module_code = ""
        for module_name, funcs in modules.items():
                module_code += gen_module_caller(module_name, funcs)

        enum_code = ""
        for enum_name, enum_key_values in enums.items():
                enum_code += genenum.genenum(enum_name, enum_key_values)

        return head_code + enum_code + module_code + end_code
