# 2016-7-1
# build by qianqians
# genmodule

import tools

def genmodule(module_name, funcs):
        code = "/*this module file is codegen by juggle for c++*/\n"
        code += "#include <Imodule.h>\n"
        code += "#include <boost/make_shared.hpp>\n"
        code += "#include <boost/signal2.hpp>\n"
        code += "#include <string>\n"
        code += "#include <Msgpack.hpp>\n\n"

        code += "namespace module\n{\n"
        code += "class " + module_name + " : public juggle::Imodule {\n"
        code += "public:\n"
        code += "    " + module_name + "(){\n"
        code += "        module_name = \"" + module_name + "\";\n"
        code += "    }\n\n"

        code += "    ~" + module_name + "(){\n"
        code += "    }\n\n"

        for i in funcs:
                code += "    boost::signals2::signal<void("
                count = 0
                for item in i[2]:
                        code += tools.gentypetocpp(item)
                        count = count + 1
                        if count < len(i[2]):
                                code += ", "
                ") > sig" + i[1] + "handle;"
                code += ");\n"
                code += "    void " + i[1] + "(MsgPack::object _event){\n"
                code += "        sig" + i[1] + "handle(\n"
                for n in range(len(i[2])):
                        code += "            std::get<" + str(n) + ">(_event.as<std::tuple<"
                        count = 0
                        for item in i[2]:
                                code += tools.gentypetocpp(item)
                                count = count + 1
                                if count < len(i[2]):
                                        code += ", "
                        code += "> >())"
                        if n < len(i[2]) - 1:
                                code += ",\n"
                code += ");\n"
                code += "    }\n\n"

        code += "}\n"
        code += "}\n"

        return code
