# 2018-3-16
# build by qianqians
# genjs

import sys
sys.path.append("./parser")

import os
import jparser

def gen(inputdir, lang, outputdir):
        defmodulelist = []

        syspath = "./client_call_hub/gen/"
        c_suffix = ""
        if lang == 'c++':
                sys.path.append("./enum/c++")
                syspath += "c++/"
                c_suffix = "hpp"
        if lang == 'csharp':
                sys.path.append("./enum/csharp")
                syspath += "csharp/"
                c_suffix = "cs"
        if lang == 'js':
                sys.path.append("./enum/js")
                syspath += "js/"
                c_suffix = "js"
        sys.path.append(syspath)
        import gencaller
        sys.path.remove(syspath)

        if not os.path.isdir(outputdir):
                os.mkdir(outputdir)

        for filename in os.listdir(inputdir):
                fname = os.path.splitext(filename)[0]
                fex = os.path.splitext(filename)[1]
                if fex != '.juggle':
                        continue

                file = open(inputdir + '//' + filename, 'r')
                genfilestr = file.readlines()

                module, enum = jparser.parser(genfilestr)
                print module
                print enum
                modules = {}
                for module_name, module_info in module.items():
                        if module_name in defmodulelist:
                                raise 'redefined module %s' % module_name

                        if module_info["module_type"] != "client_call_hub":
                                raise ('%s has wrong module type %s' % (module_name, module_info["module_type"]))
                        defmodulelist.append(module_name)

                        modules[module_name] = module_info["method"]
                for enum_name, enums in enum.items():
                        if enum_name in defmodulelist:
                                raise 'redefined enum %s' % enum_name
                        defmodulelist.append(enum_name)

                callercode = gencaller.gencaller(fname, modules, enum)
                file = open(outputdir + '//' + fname + '_caller.' + c_suffix, 'w')
                file.write(callercode)
                file.close()

if __name__ == '__main__':
        gen(sys.argv[1], sys.argv[2], sys.argv[3])
