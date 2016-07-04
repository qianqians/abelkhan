# 2016-7-1
# build by qianqians
# gencaller

import tools

def gencaller(module_name, funcs):
        code = "/*this caller file is codegen by juggle for c#*/\n"
        code += "using System;\n"
        code += "using System.Collections;\n"
        code += "using System.IO;\n"
        code += "using MsgPack;\n"
        code += "using MsgPack.Serialization;\n\n"

        code += "namespace caller\n"
        code += "{\n"
        code += "    public class " + module_name + " : juggle.Icaller \n"
        code += "    {\n"
        code += "        public " + module_name + "(juggle.Ichannel _ch) : base(_ch)\n"
        code += "        {\n"
        code += "            module_name = \"" + module_name + "\";\n"
        code += "        }\n\n"

        for i in funcs:
                code += "        public void " + i[1] + "("
                count = 0
                for item in i[2]:
                        code += tools.gentypetocsharp(item) + " argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                code += ","
                code += ")\n"
                code += "        {\n"
                code += "            ArrayList _argv = new ArrayList();\n"
                for n in range(len(i[2])):
                        code += "            _argv.Add(argv" + str(n) + ");\n"
                code += "            call_module_method(\"" + i[1] + "\", _argv);\n"
                code += "        }\n\n"

        code += "    }\n"
        code += "}\n"

        return code
