# 2016-7-1
# build by qianqians
# gencharp

import sys
sys.path.append("./parser")
sys.path.append("./gen/csharp")

import os
import gencaller
import genmodule
import jparser

def gen(inputdir, outputdir):
        defmodulelist = []

        if not os.path.isdir(outputdir):
                os.mkdir(outputdir)
        if not os.path.isdir(outputdir + '\\caller'):
                os.mkdir(outputdir + '\\caller')
        if not os.path.isdir(outputdir + '\\module'):
                os.mkdir(outputdir + '\\module')
                
        for filename in os.listdir(inputdir):
                fname = os.path.splitext(filename)[0]
                fex = os.path.splitext(filename)[1]
                if fex == '.juggle':
                        file = open(inputdir + '\\' + filename, 'r')
                        genfilestr = file.readlines()

                        keydict = jparser.parser(genfilestr)
                                
                        for module_name, funcs in keydict.items():
                                if module_name in defmodulelist:
                                        raise 'redefined module %s' % module_name

                                defmodulelist.append(module_name)
                                
                                callercode = gencaller.gencaller(module_name, funcs)
                                file = open(outputdir + '\\caller\\' + module_name + 'caller.cs', 'w')
                                file.write(callercode)
                                file.close

                                modulecode = genmodule.genmodule(module_name, funcs)
                                file = open(outputdir + '\\module\\' + module_name + 'module.cs', 'w')
                                file.write(modulecode)
                                file.close

if __name__ == '__main__':
        gen(sys.argv[1], sys.argv[2])
