# 2016-7-1
# build by qianqians
# gencaller

import tools

def gencaller(module_name, funcs):
        code = "/*this caller file is codegen by juggle for c++*/\n"
        code += "#include <string>\n"
        code += "#include <Icaller.h>\n"
        code += "#include <Ichannel.h>\n"
        code += "#include <MsgPack.hpp>\n"
        code += "#include <boost/any.hpp>\n"
        code += "#include <boost/make_shared.hpp>\n\n"

        code += "namespace caller\n"
        code += "{\n"
        code += "class " + module_name + " : public juggle::Icaller {\n"
        code += "public:\n"
        code += "    " + module_name + "(boost::shared_ptr<juggle::Ichannel> _ch) : Icaller(_ch) {\n"
        code += "        module_name = \"" + module_name + "\";\n"
        code += "    }\n\n"

        code += "    ~" + module_name + "(){\n"
        code += "    }\n\n"

        for i in funcs:
                code += "    void " + i[1] + "("
                count = 0
                for item in i[2]:
                        code += tools.gentypetocpp(item) + " argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                code += ","
                code += "){\n"
                code += "        std::tuple<std::string, std::string, std::tuple<"
                for item in i[2]:
                        code += tools.gentypetocpp(item)
                        count = count + 1
                        if count < len(i[2]):
                                code += ","
                code += "> _argv(module_name, \"" + i[1] + "\", std::make_tuple("
                for n in range(len(i[2])):
                        code += "argv" + str(n)
                        if count < len(i[2]):
                                code += ","
                code += "));\n"
                code += "        std::stringstream ss;\n"
                code += "        msgpack::pack(ss, v);\n"
                code += "        ch->senddata(ss.str().data(), ss.str().size());\n"
                code += "    }\n\n"

        code += "};\n\n"
        code += "}\n"

        return code
