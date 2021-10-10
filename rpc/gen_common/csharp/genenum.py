#coding:utf-8
# 2019-12-26
# build by qianqians
# genenum

def genenum(pretreatment):
    enum = pretreatment.enum
    
    code = "/*this enum code is codegen by abelkhan codegen for c#*/\n\n"
    for enum_name, enums in enum.items():
        code += "    public enum " + enum_name + "{\n"
        names = []
        count = 0
        for key, value in enums:
            count = count + 1
            if key in names:
                raise Exception("repeat enum elem:%s in enum:%s" % (key, enum_name))
            code += "        " + key + " = " + str(value)
            if count < len(enums):
                code += ",\n"
            names.append(key)
        code += "\n    }\n"

    return code

