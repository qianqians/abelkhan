# 2016-7-1
# build by qianqians
# genmodule

import tools

def genmodule(module_name, funcs):
        code = "/*this module file is codegen by juggle for c#*/\n"
        code += "using System;\n"
        code += "using System.Collections;\n"
        code += "using System.Collections.Generic;\n"
        code += "using MsgPack;\n"
        code += "using MsgPack.Serialization;\n\n"

        code += "namespace module\n{\n"
        code += "    public class " + module_name + " : juggle.Imodule \n    {\n"

        code += "        public " + module_name + "()\n        {\n"
        code += "			module_name = \"" + module_name + "\";\n"
        code += "        }\n\n"

        for i in funcs:
                code += "        public delegate void " + i[1] + "handle("
                count = 0
                for item in i[2]:
                        code += tools.gentypetocsharp(item) + " argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                code += ", "
                code += ");\n"
                code += "        public event " + i[1] + "handle on" + i[1] + ";\n"
                code += "        public void " + i[1] + "(IList<MsgPack.MessagePackObject> _event)\n        {\n"
                code += "            if(on" + i[1] + " != null)\n            {\n"
                count = 0
                for item in i[2]:
                        code += "                var argv" + str(count) + " = ((MsgPack.MessagePackObject)_event[" + str(count) + "])." + tools.gentypetomsgpack(item) + ";\n"
                        count = count + 1
                code += "                on" + i[1] + "("
                count = 0
                for item in i[2]:
                        code += " argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                code += ", "
                code += ");\n"
                code += "            }\n"
                code += "        }\n\n"

        code += "	}\n"
        code += "}\n"

        return code
