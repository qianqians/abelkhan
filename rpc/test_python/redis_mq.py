import abelkhan
import redis
import msgpack

class redis_mq:
     pass

class redischannel(abelkhan.Ichannel):
    def __init__(self, channelName, mq_handle:redis_mq) -> None:
        super(redischannel, self).__init__()
         
        self.channelName = channelName
        self.redis_mq_handle = mq_handle

    def disconnect(self):
        pass

    def send(self, data:bytes):
        self.redis_mq_handle.sendmsg(self.channelName, data)

class redis_mq(object):
    def __init__(self, connUrl:str, _listen_channel_name:str, _modulemng:abelkhan.modulemng) -> None:
        self.connUrl = connUrl
        self.r = redis.Redis.from_url(connUrl)
        
        self.main_channel_name = _listen_channel_name
        self.listen_channel_names = [_listen_channel_name]
        self.wait_listen_channel_names : list[str] = []

        self.channels : dict[str, abelkhan.Ichannel] = {}

        self.modulemng = _modulemng

    def take_over_svr(self, svr_name:str):
        self.wait_listen_channel_names.append(svr_name)

    def recover(self):
        self.r = redis.Redis.from_url(self.connUrl)

    def connect(self, ch_name:str) -> redischannel:
        _ch = self.channels.get(ch_name)
        if _ch:
            return _ch
            
        _ch = redischannel(ch_name, self)
        self.channels[ch_name] = _ch

        return _ch
    
    def sendmsg(self, ch_name:str, data:bytes):
        b_listen_ch_name = bytes(self.main_channel_name)
        _listen_ch_name_size = len(b_listen_ch_name)
        b_buf = bytes([(_listen_ch_name_size & 0xff), (_listen_ch_name_size >> 8 & 0xff), (_listen_ch_name_size >> 16 & 0xff), (_listen_ch_name_size >> 24 & 0xff)])
        b_buf = b_buf + b_listen_ch_name + data
        
        while True:
            try:
                self.r.lpush(ch_name, b_buf)
                break
            except Exception as ex:
                print(ex)
                self.recover()

    def list_channel_name(self):
        for _name in self.wait_listen_channel_names:
            self.listen_channel_names.append(_name)
        self.wait_listen_channel_names.clear()

    def recvmsg_mq(self):
        self.list_channel_name()

        for listen_channel_name in self.listen_channel_names:
            try:
                pop_data = self.r.rpop(listen_channel_name)
                while pop_data:
                    _ch_name_size = pop_data[0] | pop_data[1] << 8 | pop_data[2] << 16 | pop_data[3] << 24
                    _header_len = 4 + _ch_name_size
                    _ch_name = str(pop_data[4:_header_len])

                    _ch = self.channels.get(_ch_name)
                    if not _ch:
                        _ch = redischannel(_ch_name, self)
                        self.channels[_ch_name] = _ch
                    
                    self.modulemng.process_event(_ch, msgpack.loads(pop_data[_header_len:]))

                    pop_data = self.r.rpop(listen_channel_name)

            except Exception as ex:
                print(ex)
                self.recover()