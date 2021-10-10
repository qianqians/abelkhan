#coding:utf-8
# 2015-5-15
# build by qianqians
# modulemachine

from deletenonespacelstrip import deleteNoneSpacelstrip
from parametercheck import parameter_check

class func(object):
    def __init__(self):
        self.keyworld = ''
        self.func = []
        self.argvtuple = None
        self.argvPair = None
        self.parameter = False
        self.list_parameter = False
        self.isdefaultparameter = False

    def clear(self):
        self.keyworld = ''
        self.func = []
        self.argvtuple = None
        self.argvPair = None
        self.parameter = False
        self.list_parameter = False
        self.isdefaultparameter = False

    def push(self, ch):
        if ch == '[' and self.parameter:
            self.list_parameter = True

        if self.parameter and self.list_parameter == True:
            self.keyworld += ch
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if ch == ']':
                self.list_parameter = False
            return False

        if ch in [' ', '    ', '\r', '\n', '\t', '\0']:
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.keyworld == '=':
                self.parameter = True
                self.keyworld = ''
            if self.keyworld != '':
                if self.argvtuple is None:
                    self.func.append(deleteNoneSpacelstrip(self.keyworld))
                    self.keyworld = ''
                else:
                    self.argvPair.append(deleteNoneSpacelstrip(self.keyworld))
                    self.keyworld = ''
            return False

        if ch == ',':
            if self.keyworld != '':
                self.argvPair.append(deleteNoneSpacelstrip(self.keyworld))
                n = len(self.argvPair)
                if n == 2:
                    if self.isdefaultparameter:
                        raise Exception("after default parameter mast have default parameter:%s,%s in func:%s" % (self.argvPair[0], self.argvPair[1], self.func[0]))
                    else:
                        self.argvPair.append(None)
                elif n == 3:
                    self.isdefaultparameter = True
                    (key, value, parameter) = self.argvPair
                    if parameter_check(key, parameter)  == False:
                        raise Exception("wrong type default parameter:%s,%s,%s in func:%s" % (key, value, parameter, self.func[0]))
                self.argvtuple.append(self.argvPair)
                self.argvPair = []
                self.keyworld = ''
                self.list_parameter = False
                self.isdefaultparameter = False
            return False

        if ch == '(':
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.keyworld != '':
                self.func.append(deleteNoneSpacelstrip(self.keyworld))
            self.argvtuple = []
            self.argvPair = []
            self.keyworld = ''
            return False

        if ch == ')':
            if self.keyworld != '':
                self.argvPair.append(deleteNoneSpacelstrip(self.keyworld))
                n = len(self.argvPair)
                if n == 2:
                    if self.isdefaultparameter:
                        raise Exception("after default parameter mast have default parameter:%s,%s in func:%s" % (self.argvPair[0], self.argvPair[1], self.func[0]))
                    else:
                        self.argvPair.append(None)
                elif n == 3:
                    (key, value, parameter) = self.argvPair
                    if parameter_check(key, parameter)  == False:
                        raise Exception("wrong type default parameter:%s,%s,%s in func:%s" % (key, value, parameter, self.func[0]))
                self.argvtuple.append(self.argvPair)

            if self.argvtuple is None:
                self.func.append([])
            else:
                self.func.append(self.argvtuple)

            self.argvtuple = None
            self.argvPair = None
            self.keyworld = ''

            return False

        if ch == ';':
            return True

        self.keyworld += ch

        return False

class module(object):
    def __init__(self):
        self.keyworld = ''
        self.name = ''
        self.module = []
        self.machine = None

    def check_repeat_func(self):
        func_list = []
        for func_info in self.module:
            name = func_info[0]
            if name in func_list:
                raise Exception("repeat func:" + name + " in module:" + self.name)
            func_list.append(name)

    def push(self, ch):
        if ch == '}':
            self.machine = None
            self.check_repeat_func()
            return True

        if self.machine is not None:
            if self.machine.push(ch):
                self.module.append(self.machine.func)
                self.machine.clear()
        else:
            if ch == '{':
                self.name = deleteNoneSpacelstrip(self.keyworld)
                self.keyworld = ''
                self.machine = func()
                return False

            self.keyworld += ch

        return False
