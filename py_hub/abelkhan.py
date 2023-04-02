#coding:utf-8
# 2023-3-31
# build by qianqians
# juggle abelkhan
import random
import time
import msgpack
from abc import ABCMeta,abstractmethod
from collections.abc import Callable

class AbelkhanError(RuntimeError):
    def __init__(self, err:str) -> None:
        super(AbelkhanError, self).__init__(err)
        self.err = err   

def RandomUUID():
    random.randint(0, int.max)

def timetmp():
    return int(round(time.time() * 1000))
    
class Ichannel(object):
    __metaclass__ = ABCMeta
    
    @abstractmethod
    def disconnect():
        pass
    
    @abstractmethod
    def send(data:bytes):
        pass
    
class Icaller(object):
    def __init__(self, _ch:Ichannel) -> None:
        self.ch = _ch
    
    def call_module_method(self, methodname:str, argvs:list) -> None:
        _event = [methodname, argvs]
        _data = msgpack.dumps(_event)
        
        _tmp_len = len(_data)
        _len0 = _tmp_len & 0xff
        _len1 = (_tmp_len >> 8) & 0xff
        _len2 = (_tmp_len >> 16) & 0xff
        _len3 = (_tmp_len >> 24) & 0xff
        _send_buf = bytes([_len0, _len1, _len2, _len3]) + _data
        
        self.ch.send(_send_buf)
    
class Response(Icaller):
    def __init__(self, _ch:Ichannel) -> None:
        super(Response, self).__init__(_ch)
        
class Imodule(object):
    def __init__(self) -> None:
        self.current_ch = None
        self.rsp = None

class modulemng(object):
    def __init__(self) -> None:
        self.method_set = {}
        
    def reg_method(self, method_name:str, method:tuple[Imodule, Callable[...]]):
        self.method_set[method_name] = method
    
    def process_event(self, _ch:Ichannel, _event:list):
        try:
            method_name = _event[0]
            _method_tuple = self.method_set.get(method_name)
            if _method_tuple:
                (_module, _method) = _method_tuple
                _module.current_ch = _ch
                _method(_event[1])
                _module.current_ch = None
            else:
                raise AbelkhanError("do not have a method named:" + method_name)
        except:
            raise AbelkhanError("process_event error")