# 2018-3-16
# build by qianqians
# genjs

import sys
sys.path.append("./parser")

import uuid
import os
import jparser

def gen_cpp_import(_import):
    code = "#include <hub_service.h>\n"
    code += "#include <signals.h>\n\n"
    for _i in _import:
        code += "#include \"" + _i + ".h\"\n"
    code += "\nnamespace abelkhan\n{\n"
    return code

def gen_csharp_import(_import):
    code = "using System;\n"
    code += "using System.Collections;\n"
    code += "using System.Collections.Generic;\n"
    code += "using System.Threading;\n"
    code += "using MsgPack.Serialization;\n\n"
    code += "namespace abelkhan\n{\n"
    return code

def gen(inputdir, commondir, lang, outputdir):
    syspath = "./gen/hub_call_hub/"
    if lang == 'cpp':
        sys.path.append("./gen_common/cpp")
        sys.path.append("./tools/cpp")
        syspath += "cpp/"
        sys.path.append(syspath)
        import gencaller
        import genmodule
        sys.path.remove(syspath)
    elif lang == 'csharp':
        sys.path.append("./gen_common/csharp")
        sys.path.append("./tools/csharp")
        syspath += "csharp/"
        sys.path.append(syspath)
        import gencaller
        import genmodule
        sys.path.remove(syspath)
    import genenum
    import genstruct

    if not os.path.isdir(outputdir):
        os.mkdir(outputdir)

    pretreatmentdata = jparser.batch(inputdir, commondir)
    for pretreatment in pretreatmentdata:
        if lang == "cpp":
            _uuid = '_'.join(str(uuid.uuid3(uuid.NAMESPACE_DNS, pretreatment.name)).split('-'))
            code = "#ifndef _h_" + pretreatment.name + "_" + _uuid + "_\n"
            code += "#define _h_" + pretreatment.name + "_" + _uuid + "_\n\n"
            code += gen_cpp_import(pretreatment._import)
            code += genenum.genenum(pretreatment)
            code += genstruct.genstruct(pretreatment)
            h_code_tmp, cpp_code_tmp = gencaller.gencaller(pretreatment)
            code += h_code_tmp
            code += genmodule.genmodule(pretreatment)
            code += "\n}\n\n"
            code += "#endif //_h_" + pretreatment.name + "_" + _uuid + "_\n"

            cpp_code = "#include \"" + pretreatment.name + ".h\"\n\n"
            cpp_code += "namespace abelkhan\n{\n\n"
            cpp_code += cpp_code_tmp
            cpp_code += "\n}\n"

            file = open(outputdir + '//' + pretreatment.name + ".h", 'w')
            file.write(code)
            file.close()

            file = open(outputdir + '//' + pretreatment.name + ".cpp", 'w')
            file.write(cpp_code)
            file.close()
        elif lang == 'csharp':
            code = gen_csharp_import(pretreatment._import)
            code += genenum.genenum(pretreatment)
            code += genstruct.genstruct(pretreatment)
            code += gencaller.gencaller(pretreatment)
            code += genmodule.genmodule(pretreatment)
            code += "\n}\n"

            file = open(outputdir + '//' + pretreatment.name + "_svr.cs", 'w')
            file.write(code)
            file.close()

if __name__ == '__main__':
    if len(sys.argv) == 5:
        gen(sys.argv[1], sys.argv[4], sys.argv[2], sys.argv[3])
    else:
        gen(sys.argv[1], None, sys.argv[2], sys.argv[3])