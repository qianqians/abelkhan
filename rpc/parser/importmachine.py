#coding:utf-8
# 2019-12-20
# build by qianqians
# _importmachine

from deletenonespacelstrip import deleteNoneSpacelstrip

class _import(object):
    def __init__(self):
        self.keyworld = ''
        self.name = ''
        self.machine = None

    def push(self, ch):
        if ch in [' ', '    ', '\r', '\n', '\t', '\0', '\r\n']:
            self.keyworld = deleteNoneSpacelstrip(self.keyworld)
            if self.keyworld != '':
                self.name = deleteNoneSpacelstrip(self.keyworld)
                print(self.name)
                return True

        self.keyworld += ch

        return False
