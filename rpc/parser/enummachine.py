#coding:utf-8
# 2014-12-18
# build by qianqians
# enummachine

from deletenonespacelstrip import deleteNoneSpacelstrip

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
                self.value = deleteNoneSpacelstrip(self.keyworld)
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
