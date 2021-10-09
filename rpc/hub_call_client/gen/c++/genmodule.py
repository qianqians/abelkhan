# 2016-7-1
# build by qianqians
# genmodule

import genenum
import tools

def gen_module_module(module_name, funcs):
        code = "    class " + module_name + "_module : public common::imodule, public std::enable_shared_from_this<" + module_name + "_module> \n    {\n"
        code += "    public:\n        std::string module_name;\n"
        code += "    public:\n        " + module_name + "_module()\n        {\n        }\n\n"
        code += "        void Init(std::shared_ptr<client::client> _client)\n        {\n"
        code += "            module_name = \"" + module_name + "\";\n"
        code += "            _client->modules.add_module(\"" + module_name + "\", shared_from_this());\n\n"
        for i in funcs:
                func_name = i[0]

                if i[1] != "ntf" and i[1] != "broadcast":
                        raise "func:" + func_name + " wrong rpc type:" + i[1] + ", must ntf or broadcast"

                code += "            reg_cb(\"" + func_name + "\", std::bind(&" + module_name + "_module::" + func_name + ", this, std::placeholders::_1));\n"
        code += "        }\n\n    public:\n"

        for i in funcs:
                func_name = i[0]

                if i[1] != "ntf" and i[1] != "broadcast":
                        raise "func:" + func_name + " wrong rpc type:" + i[1] + ", must ntf or broadcast"

                code += "        boost::signals2::signal<void("
                count = 0
                for item in i[2]:
                        code += tools.gentypetocpp(item) + " argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                code += ", "
                code += ")> sig" + func_name + ";\n"
                code += "        void " + func_name + "(std::shared_ptr<std::vector<boost::any> > argvs)\n        {\n"
                count = 0
                for item in i[2]:
                        code += "            auto argv" + str(count) + " = boost::any_cast<" + tools.gentypetocpp(item) + " >((*argvs)[" + str(count) + "]);\n"
                        count = count + 1
                code += "            sig" + func_name + "("
                count = 0
                for item in i[2]:
                        code += "argv" + str(count)
                        count = count + 1
                        if count < len(i[2]):
                                code += ", "
                code += ");\n"
                code += "        }\n\n"

        code += "    };\n"

        return code

def genmodule(file_name,modules, enums):
        head_code = "/*this rsp file is codegen by abelkhan for c++*/\n\n"
        head_code += "#include <string>\n"
        head_code += "#include <functional>\n"
        head_code += "#include <memory>\n\n"

        head_code += "#include <boost/any.hpp>\n"
        head_code += "#include <boost/signals2.hpp>\n\n"

        head_code += "#include <module.h>\n\n"

        head_code += "#include <client.h>\n\n"

        head_code += "namespace abelkhan_gen_code\n"
        head_code += "{\n"

        end_code = "}\n"

        module_code = ""
        for module_name, funcs in modules.items():
                module_code += gen_module_module(module_name, funcs)

        enum_code = ""
        for enum_name, enum_key_value in enums.items():
                enum_code += genenum.genenum(enum_name, enum_key_value)


        return head_code + enum_code + module_code + end_code
