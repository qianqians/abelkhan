# 2014-12-18
# build by qianqians
# statemachine

#module name{
#   void func();
#   int func(int);
#   array func(int, string)
#   table func(int, array, bool, float, string)
#}

from deletenonespacelstrip import deleteNoneSpacelstrip

class func(object):
    def __init__(self):
        self.keyworld = ''
        self.func = []
        self.argvtuple = None

    def clear(self):
        self.keyworld = ''
        self.func = []
        self.argvtuple = None

    def push(self, ch):
        if ch == ' ' or ch == '\0':
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.keyworld != '':
                if self.argvtuple is None:
                    self.func.append(deleteNoneSpacelstrip(self.keyworld))
                else:
                    if self.keyworld in ['table', 'array', 'int', 'string', 'float', 'bool']:
                        self.argvtuple.append(deleteNoneSpacelstrip(self.keyworld))
                self.keyworld = ''
            return False

        if ch == ',':
            if self.keyworld != '' and self.keyworld in ['table', 'array', 'int', 'string', 'float', 'bool']:
                self.argvtuple.append(deleteNoneSpacelstrip(self.keyworld))
                self.keyworld = ''

            return False

        if ch == '(':
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.keyworld != '':
                self.func.append(deleteNoneSpacelstrip(self.keyworld))
            self.argvtuple = []
            self.keyworld = ''
            return False

        if ch == ')':
            if self.keyworld != '' and self.keyworld in ['table', 'array', 'int', 'string', 'float', 'bool']:
                self.argvtuple.append(deleteNoneSpacelstrip(self.keyworld))

            if self.argvtuple is None:
                self.func.append([])
            else:
                self.func.append(self.argvtuple)

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
        self.module = {}
        self.machine = None

    def push(self, ch):
        if ch == '}':
            self.machine = None
            return True

        if self.machine is not None:
            if self.machine.push(ch):
                self.module["method"].append(self.machine.func)
                self.machine.clear()
        else:
            if ch == '{':
                self.keyworld = ''
                self.module["method"] = []
                self.machine = func()
                return False

            if ch == '(':
                self.keyworld = deleteNoneSpacelstrip(self.keyworld)
                if self.keyworld != '':
                    self.name = deleteNoneSpacelstrip(self.keyworld)
                self.keyworld = ''
                return False

            if ch == ')':
                self.keyworld = deleteNoneSpacelstrip(self.keyworld)
                if self.keyworld != '' and self.keyworld in ['client_call_hub', 'hub_call_client', 'hub_call_hub']:
                    self.module["module_type"] = self.keyworld
                self.keyworld = ''
                return False

            self.keyworld += ch

        return False

class keyvalue(object):
    def __init__(self):
        self.keyworld = ''
        self.key = ""
        self.value = None

    def clear(self):
        self.keyworld = ''
        self.key = ""
        self.value = None

    def push(self, ch):
        if ch == '=':
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.keyworld != '':
                self.key = deleteNoneSpacelstrip(self.keyworld)
            self.keyworld = ''
            return False

        if ch == ';':
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.keyworld != '':
                self.value = int(deleteNoneSpacelstrip(self.keyworld))
            self.keyworld = ''
            return True

        self.keyworld += ch

        return False

class enum(object):
    def __init__(self):
        self.keyworld = ''
        self.name = ''
        self.enum = []
        self.machine = None

    def push(self, ch):
        if ch == '}':
            return True

        if self.machine is not None:
            if self.machine.push(ch):
                self.enum.append((self.machine.key, self.machine.value))
                self.machine.clear()
        else:
            if ch == '{':
                self.keyworld = deleteNoneSpacelstrip(self.keyworld)
                if self.keyworld != '':
                    self.name = deleteNoneSpacelstrip(self.keyworld)
                self.keyworld = ''
                self.machine = keyvalue()
                return False

            self.keyworld += ch

        return False

class statemachine(object):
    def __init__(self):
        self.keyworld = ''
        self.module = {}
        self.enum = {}
        self.machine = None

    def push(self, ch):
        if self.machine is not None:
            if self.machine.push(ch):
                if isinstance(self.machine, module):
                    self.module[self.machine.name] = self.machine.module
                    self.machine = None
                if isinstance(self.machine, enum):
                    self.enum[self.machine.name] = self.machine.enum
                    self.machine = None
        else:
            if ch == ' ' or ch == '\0':
                if deleteNoneSpacelstrip(self.keyworld) == 'module':
                    self.machine = module()
                    self.keyworld = ''
                elif deleteNoneSpacelstrip(self.keyworld) == 'enum':
                    self.machine = enum()
                    self.keyworld = ''
            else:
                self.keyworld += ch

    def getmodule(self):
        return self.module

    def getenum(self):
        return self.enum

    def syntaxanalysis(self, genfilestr):
        for str in genfilestr:
            for ch in str:
                self.push(ch)
