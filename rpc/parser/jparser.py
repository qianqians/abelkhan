# 2016-6-10
# build by qianqians
# parser

text = """
module name(client_call_hub){
   func_test req(int, array , bool) rsp(int) err(int);
}
module test(hub_call_hub){
    test1 ntf(string);
}
"""

import statemachine
import deletenote

def parser(str):
    machine = statemachine.statemachine()

    machine.syntaxanalysis(deletenote.deletenote(str))

    return machine.getmodule(), machine.getenum()

#print(parser(text))
