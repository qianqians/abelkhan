# 2016-6-10
# build by qianqians
# parser

text = """
module name{
   void func1();
   int func2(int);
   array func3(int , string);
   table func4(int, array , bool, float, string);
}
"""

import statemachine
import deletenote

def parser(str):
    machine = statemachine.statemachine()

    machine.syntaxanalysis(deletenote.deletenote(str))

    return machine.getmodule()

#print(parser(text))

