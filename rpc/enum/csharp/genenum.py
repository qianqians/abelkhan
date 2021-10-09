# 2018-5-5
# build by qianqians
# genenum

def genenum(enum_name, enums):
        code = "/*this enum code is codegen by abelkhan codegen for c#*/\n\n"

        code += "    public enum " + enum_name + "\n    {\n"
        count = 0;
        for key, value in enums:
            code += "    " + key + " = " + str(value)
            count = count + 1
            if count < len(enums):
                code += ",\n"
        code += "\n    }\n"

        return code
