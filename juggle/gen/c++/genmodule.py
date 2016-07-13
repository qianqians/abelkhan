# 2016-7-1
# build by qianqians
# genmodule

import tools

def genmodule(module_name, funcs):
        code = "/*this module file is codegen by juggle for c++*/\n"
        code += "#ifndef _" + module_name + "_module_h\n"
        code += "#define _" + module_name + "_module_h\n"
        code += "#include <Imodule.h>\n"
        code += "#include <boost/shared_ptr.hpp>\n"
        code += "#include <boost/signals2.hpp>\n"
        code += "#include <string>\n"

        code += "namespace module\n{\n"
        code += "class " + module_name + " : public juggle::Imodule {\n"
        code += "public:\n"
        code += "    " + module_name + "(){\n"
        code += "        module_name = \"" + module_name + "\";\n"
        for i in funcs:
                code += "        protcolcall_set.insert(std::make_pair(\"" + i[1] + "\", boost::bind(&" + module_name + "::" + i[1] + ", this, _1)));\n"

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
                code += ") > sig" + i[1] + "handle;\n"
                code += "    void " + i[1] + "(boost::shared_ptr<std::vector<boost::any> > _event){\n"
                code += "        sig" + i[1] + "handle(\n"
                count = 0
                for item in i[2]:
                        code += "            boost::any_cast<" + tools.gentypetocpp(item) + ">((*_event)[" + str(count) + "])"
                        count += 1
                        if count < len(i[2]):
                                code += ", \n"
                code += ");\n"
                code += "    }\n\n"

        code += "};\n\n"
        code += "}\n\n"
        code += "#endif\n"

        return code
