# 2016-7-1
# build by qianqians
# gencaller

import tools

def gencaller(module_name, funcs):
        code = "/*this caller file is codegen by juggle for c++*/\n"
        code += "#ifndef _" + module_name + "_caller_h\n"
        code += "#define _" + module_name + "_caller_h\n"
        code += "#include <sstream>\n"
        code += "#include <tuple>\n"
        code += "#include <string>\n"
        code += "#include <Icaller.h>\n"
        code += "#include <Ichannel.h>\n"
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
                code += "        auto v = boost::make_shared<std::vector<boost::any> >();\n"
                code += "        v->push_back(\"" + module_name + "\");\n"
                code += "        v->push_back(\"" + i[1] + "\");\n"
                code += "        v->push_back(boost::make_shared<std::vector<boost::any> >());\n"
                for count in range(len(i[2])):
                        code += "        (boost::any_cast<boost::shared_ptr<std::vector<boost::any> > >((*v)[2]))->push_back(argv" + str(count) + ");\n"
                code += "        ch->push(v);\n"
                code += "    }\n\n"

        code += "};\n\n"
        code += "}\n\n"
        code += "#endif\n"

        return code
