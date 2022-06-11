#coding:utf-8
# 2016-6-10
# build by qianqians
# parser
import os
import statemachine
import deletenote
import postprocess

text = """
module name{
    func2 req(kv a) rsp(kv a) err(int i);
    func3 ntf(int i, string str);
    func4 ntf(int i, array a, bool b, float f, string s);
}

struct kv{
    int k1;
}
"""

def parser(_str):
    machine = statemachine.statemachine()
    machine.syntaxanalysis(deletenote.deletenote(_str))
    return machine.getimport(), machine.getmodule(), machine.getenum(), machine.getstruct()

def batch(inputdir, commondir):
    pretreatmentdata = []
    for filename in os.listdir(inputdir):
        fname = os.path.splitext(filename)[0]
        fex = os.path.splitext(filename)[1]
        if fex == '.juggle':
            from sys import version_info
            if version_info.major == 2:
                file = open(inputdir + '//' + filename, 'r')
            elif version_info.major == 3:
                file = open(inputdir + '//' + filename, 'r', encoding='utf-8')
            genfilestr = file.readlines()

            print("fname:" + fname)
            _import, module, enum, struct = parser(genfilestr)
            pretreatmentdata.append(postprocess.pretreatment(fname, _import, module, enum, struct))
            
    if commondir is not None:
        pretreatmentdata_import = []
        for filename in os.listdir(commondir):
            fname = os.path.splitext(filename)[0]
            fex = os.path.splitext(filename)[1]
            if fex == '.juggle':
                from sys import version_info
                if version_info.major == 2:
                    file = open(commondir + '//' + filename, 'r')
                elif version_info.major == 3:
                    file = open(commondir + '//' + filename, 'r', encoding='utf-8')
                genfilestr = file.readlines()

                print("fname:" + fname)
                _import, module, enum, struct = parser(genfilestr)
                if len(module) > 0:
                    raise Exception("fname:" + fname + " is common, could not define module")
                pretreatmentdata_import.append(postprocess.pretreatment(fname, _import, {}, enum, struct))
        pretreatmentdata.extend(pretreatmentdata_import)
        
    postprocess.process(pretreatmentdata)
    return pretreatmentdata
    
#_import, module, enum, struct = parser(text)
#print(_import)
#print(module)
#print(enum)
#print(struct)
