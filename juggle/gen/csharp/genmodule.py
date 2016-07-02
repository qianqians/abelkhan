# 2016-7-1
# build by qianqians
# genmodule

import tools

def genmodule(module_name, funcs):
	code = "/*this module file is codegen by juggle*/\n"
	code += "using System;\n"
	code += "using System.Collections;\n\n"

	code += "namespace module\n{\n"
	code += "    public class " + module_name + " : Imodule \n    {\n"

	code += "        public " + module_name + "()\n        {\n"
	code += "			module_name = \"" + module_name + "\";\n"
	code += "        }\n\n"

	for i in funcs:
		code += "		public abstract void " + i[1] + "("
		count = 0
		for item in i[2]:
			code += tools.gentypetocsharp(item) + " argv" + str(count)
			count = count + 1
			if count < len(i[2]):
                                code += ", "
		code += ");\n\n"

	code += "	}\n"
	code += "}\n"

	return code
