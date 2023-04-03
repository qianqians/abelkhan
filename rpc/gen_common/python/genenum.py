#coding:utf-8
# 2023-3-31
# build by qianqians
# genenum

def genenum(pretreatment):
    enum = pretreatment.enum
    
    code = "# this enum code is codegen by abelkhan codegen for python\n\n"
    for enum_name, enums in enum.items():
        code += "class " + enum_name + "(Enum):\n"
        names = []
        count = 0
        for key, value in enums:
            count = count + 1
            if key in names:
                raise Exception("repeat enum elem:%s in enum:%s" % (key, enum_name))
            code += "    " + key + " = " + str(value) + "\n"
            names.append(key)
        code += "\n\n"

    return code

