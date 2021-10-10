#coding:utf-8
# 2014-12-18
# build by qianqians
# statemachine

from deletenonespacelstrip import deleteNoneSpacelstrip
from modulemachine import module
from enummachine import enum
from structmachine import struct
from importmachine import _import

class statemachine(object):
    def __init__(self):
        self.keyworld = ''
        self.module = {}
        self.enum = {}
        self.struct = {}
        self._import = []
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
                if isinstance(self.machine, struct):
                    self.struct[self.machine.name] = self.machine.elem
                    self.machine = None
                if isinstance(self.machine, _import):
                    self._import.append(self.machine.name)
        else:
            if ch in [' ', '    ', '\r', '\n', '\t', '\0']:
                if deleteNoneSpacelstrip(self.keyworld) == 'module':
                    self.machine = module()
                    self.keyworld = ''
                elif deleteNoneSpacelstrip(self.keyworld) == 'enum':
                    self.machine = enum()
                    self.keyworld = ''
                elif deleteNoneSpacelstrip(self.keyworld) == 'struct':
                    self.machine = struct()
                    self.keyworld = ''
                elif deleteNoneSpacelstrip(self.keyworld) == 'import':
                    self.machine = _import()
                    self.keyworld = ''
            else:
                self.keyworld += ch

    def getmodule(self):
        print(self.module)
        return self.module

    def getenum(self):
        return self.enum

    def getstruct(self):
        return self.struct

    def getimport(self):
        return self._import

    def syntaxanalysis(self, genfilestr):
        for str in genfilestr:
            for ch in str:
                self.push(ch)
