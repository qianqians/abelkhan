#coding:utf-8
# 2014-12-18
# build by qianqians
# structmachine

from deletenonespacelstrip import deleteNoneSpacelstrip
from parametercheck import parameter_check

class elem(object):
    def __init__(self):
        self.keyworld = ''
        self.step = 'key'
        self.key = ""
        self.value = None
        self.parameter = None
        self.list_parameter = False

    def clear(self):
        self.keyworld = ''
        self.step = 'key'
        self.key = ""
        self.value = None
        self.parameter = None
        self.list_parameter = False

    def push(self, ch):
        if ch == '[' and self.step == 'parameter':
            self.list_parameter = True

        if self.step == 'parameter' and self.list_parameter == True:
            self.keyworld += ch
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if ch == ']':
                self.list_parameter = False
            return False

        if ch in [' ', '    ', '\r', '\n', '\t', '\0'] and self.keyworld != '':
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.step == 'key' and self.keyworld != '':
                self.key = self.keyworld
                self.step = 'value'
            elif self.step == 'value' and self.keyworld != '':
                self.value = self.keyworld
                self.step = 'parameter'
            elif self.step == 'parameter' and self.keyworld != '' and self.keyworld != '=':
                self.parameter = self.keyworld
                self.step = 'done'
            self.keyworld = ''
            return False

        if ch == ';':
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.step == 'value' and self.keyworld != '':
                self.value = self.keyworld
            elif self.step == 'parameter' and self.keyworld != '' and self.keyworld != '=':
                self.parameter = self.keyworld
            self.keyworld = ''
            return True

        self.keyworld += ch
        self.keyworld = deleteNoneSpacelstrip(self.keyworld)

        return False

class struct(object):
    def __init__(self):
        self.keyworld = ''
        self.name = ''
        self.elem = []
        self.machine = None

    def push(self, ch):
        if ch == '}':
            return True

        if self.machine is not None:
            if self.machine.push(ch):
                self.elem.append((self.machine.key, self.machine.value, self.machine.parameter))
                if self.machine.parameter != None:
                    if parameter_check(self.machine.key, self.machine.parameter) == False:
                        raise Exception("wrong type default parameter:%s,%s,%s in struct:%s" % (self.key, self.value, self.parameter, self.name))
                self.machine.clear()
        else:
            if ch == '{':
                self.keyworld = deleteNoneSpacelstrip(self.keyworld)
                if self.keyworld != '':
                    self.name = deleteNoneSpacelstrip(self.keyworld)
                self.keyworld = ''
                self.machine = elem()
                return False

            self.keyworld += ch

        return False