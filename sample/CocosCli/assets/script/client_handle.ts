import { encode, decode } from "./@msgpack/msgpack";
import * as abelkhan from "./abelkhan";
import * as gate from "./gate";
import * as hub from "./hub";
import * as _client from "./client";

export class imodule {
    constructor(){
    }

    private methods = new Map<string, any>();
    public reg_cb(method_name:string, method:any) {
        this.methods.set(method_name, method);
    }
    
    public rsp:any = null;
    public invoke(cb_name:string, _event:any[]) {
        this.methods.get(cb_name).call(this, _event);
    }
}

export class modulemng {
    private module_set = new Map<string, imodule>();

    public add_module(module_name:string, _module:imodule) {
        this.module_set.set(module_name, _module);
    }

    public process_event(module_name:string, func_name:string, _event:any[]) {
        this.module_set.get(module_name).invoke(func_name, _event);
    }
}

export class wschannel implements abelkhan.Ichannel {
    private events:any[] = [];

    private offset:number = 0;
    private data:Uint8Array|null = null;

    private ws:WebSocket;

    constructor(_ws:WebSocket, cb:(ch:abelkhan.Ichannel)=>void) {
        this.ws = _ws;
        this.ws.binaryType = "arraybuffer";

        let that = this;
        this.ws.onmessage = (evt) => {
            let u8data = new Uint8Array(evt.data);
            
            var new_data = new Uint8Array(that.offset + u8data.byteLength);
            if (that.data !== null){
                new_data.set(that.data);
            }
            new_data.set(u8data, that.offset);
    
            while(new_data.length > 4) {
                var len = new_data[0] | new_data[1] << 8 | new_data[2] << 16 | new_data[3] << 24;
    
                if ( (len + 4) > new_data.length ){
                    break;
                }
    
                var str_bytes = new_data.subarray(4, (len + 4))
                that.events.push(decode(str_bytes));
                
                if ( new_data.length > (len + 4) ){
                    var _data = new Uint8Array(new_data.length - (len + 4));
                    _data.set(new_data.subarray(len + 4));
                    new_data = _data;
                }
                else{
                    new_data = null;
                    break;
                }
            }
    
            that.data = new_data;
            if (new_data !== null){
                that.offset = new_data.length;
            }else{
                that.offset = 0;
            }
        }
        this.ws.onopen = () => {
            cb(that);
        }
    }
    
    public send(send_data:Uint8Array) {
        this.ws.send(send_data.buffer);
    }

    public disconnect(){
        this.ws.close();
    }

    public pop() {
        if (this.events.length === 0) {
            return null;
        }
        return this.events.shift();
    }
}

export class wsconnectservice {
    public connect(url, cb:(ch:abelkhan.Ichannel)=>void) {
        let ws = new WebSocket(url);
        let ch = new wschannel(ws, cb);
    }
}

class gateproxy {
    private _ch:abelkhan.Ichannel ;
    private _client_call_gate_caller:gate.client_call_gate_caller ;

    constructor(ch:abelkhan.Ichannel, ) {
        this._ch = ch;
        this._client_call_gate_caller = new gate.client_call_gate_caller(ch, abelkhan._modulemng);
    }

    public onGateTime : (tick:number)=>void = null;
    public onGateDisconnect : (ch:abelkhan.Ichannel)=>void = null ;
    public heartbeats() {
        let that = this;
        this._client_call_gate_caller.heartbeats().callBack((_svr_timetmp)=> {
            if (that.onGateTime) {
                that.onGateTime(_svr_timetmp);
            }
        }, ()=> {}).timeout(5 * 1000, ()=> {
            if (that.onGateDisconnect) {
                that.onGateDisconnect(this._ch);
            }
        });
    }

    public get_hub_info(hub_type:string, cb:(info:gate.hub_info[])=>void) {
        let that = this;
        this._client_call_gate_caller.get_hub_info(hub_type).callBack((hub_info) => {
            cb(hub_info);
        }, () => { }).timeout(5 * 1000, ()=> {
            that.onGateDisconnect(this._ch);
        });
    }

    public call_hub(hub:string, module:string, func:string, argv:any[]) {
        let _event = [module, func, argv];
        this._client_call_gate_caller.forward_client_call_hub(hub, encode(_event));
    }
}

class hubproxy {
    public _hub_name:string;
    public _hub_type:string;

    private _ch:abelkhan.Ichannel;
    private _client_call_hub_caller:hub.client_call_hub_caller;

    constructor(hub_name:string, hub_type:string, ch:abelkhan.Ichannel) {
        this._hub_name = hub_name;
        this._hub_type = hub_type;

        this._ch = ch;
        this._client_call_hub_caller = new hub.client_call_hub_caller(ch, abelkhan._modulemng);
    }

    public connect_hub(cuuid:string) {
        this._client_call_hub_caller.connect_hub(cuuid);
    }

    public onHubTime : (hub_name:string, tick:number)=>void = null;
    public onHubDisconnect : (ch:abelkhan.Ichannel)=>void = null;
    public heartbeats() {
        let that = this;
        this._client_call_hub_caller.heartbeats().callBack((_hub_timetmp:number) => {
            if (that.onHubTime) {
                that.onHubTime(that._hub_name, _hub_timetmp);
            }
        }, () => { }).timeout(5 * 1000, () => {
            if (that.onHubDisconnect) {
                that.onHubDisconnect(that._ch);
            }
        });
    }

    public call_hub(module:string, func:string, argv:any[]) {
        let _event = [module, func, argv];
        this._client_call_hub_caller.call_hub(encode(_event));
    }
}

export class client
{
    public onGateDisConnect:()=>void = null;
    public onHubDisConnect:(hub_name:string)=>void = null;

    public onGateTime:(tick:number)=>void = null;
    public onHubTime:(hub_name:string, tick:number)=>void = null;

    public uuid:string;
    public _modulemng:modulemng;

    public current_hub:string;

    private _conn:wsconnectservice;
    private _gateproxy:gateproxy = null;
    private _hubproxy_set:Map<string, hubproxy>;
    private _ch_hubproxy_set:Map<abelkhan.Ichannel, hubproxy>;

    private add_chs:abelkhan.Ichannel[];
    private chs:abelkhan.Ichannel[];
    private remove_chs:abelkhan.Ichannel[];

    private _gate_call_client_module:_client.gate_call_client_module ;
    private _hub_call_client_module:hub.hub_call_client_module ;

    constructor() {
        this._modulemng = new modulemng();

        this._hubproxy_set = new Map<string, hubproxy>();
        this._ch_hubproxy_set = new Map<abelkhan.Ichannel, hubproxy>();

        this._conn = new wsconnectservice();

        this.add_chs = [];
        this.chs = [];
        this.remove_chs = [];

        setInterval(()=>{
            this.heartbeats();
        }, 5 * 1000);

        this._gate_call_client_module = new _client.gate_call_client_module(abelkhan._modulemng);
        this._gate_call_client_module.cb_ntf_cuuid = this.ntf_cuuid.bind(this);
        this._gate_call_client_module.cb_call_client = this.gate_call_client.bind(this);

        this._hub_call_client_module = new hub.hub_call_client_module(abelkhan._modulemng);
        this._hub_call_client_module.cb_call_client = this.hub_call_client.bind(this);
    }

    private ntf_cuuid(_uuid:string)
    {
        this.uuid = _uuid;

        if (this.onGateConnect){
            this.onGateConnect();
        }
    }

    private gate_call_client(hub_name:string, rpc_argv:Uint8Array)
    {
        let _event = decode(rpc_argv) as any[];
        let module = _event[0] as string;
        let func = _event[1] as string;
        let argvs = _event[2] as any[];

        this.current_hub = hub_name;
        this._modulemng.process_event(module, func, argvs);
        this.current_hub = "";
    }

    private hub_call_client(rpc_argv:Uint8Array)
    {
        let _event = decode(rpc_argv) as any[];
        let module = _event[0] as string;
        let func = _event[1] as string;
        let argvs = _event[2] as any[];

        let _hubproxy = this._ch_hubproxy_set.get(this._hub_call_client_module.current_ch);

        this.current_hub = _hubproxy._hub_name;
        this._modulemng.process_event(module, func, argvs);
        this.current_hub = "";
    }

    private heartbeats()
    {
        if (this._gateproxy)
        {
            this._gateproxy.heartbeats();
        }

        this._hubproxy_set.forEach((value, _)=>{
            value.heartbeats();
        });
    }

    public get_hub_info(hub_type:string, cb:(info:gate.hub_info[])=>void)
    {
        this._gateproxy?.get_hub_info(hub_type, cb);
    }

    public call_hub(hub_name:string, module:string, func:string, argv:any[])
    {
        if (this._hubproxy_set.has(hub_name))
        {
            let _hubproxy = this._hubproxy_set.get(hub_name);
            _hubproxy.call_hub(module, func, argv);
            return;
        }

        if (this._gateproxy != null)
        {
            this._gateproxy.call_hub(hub_name, module, func, argv);
        }
    }

    public onGateConnect:()=>void;
    public onGateConnectFaild:()=>void;
    public connect_gate(url:string)
    {
        let that = this;
        this._conn.connect(url, (ch)=>{
            console.log("connect gate url:" + url);

            this.add_chs.push(ch);

            that._gateproxy = new gateproxy(ch);
            that._gateproxy.onGateDisconnect = (ch) =>
            {
                that.remove_chs.push(ch);
                that._gateproxy = null;

                if (that.onGateDisConnect){
                    that.onGateDisConnect();
                }
            };
            that._gateproxy.onGateTime = (tick)=>{
                if (that.onGateTime){
                    that.onGateTime(tick);
                }
            }
        });
    }

    public onHubConnect:(hub_name:string)=>void;
    public onHubConnectFaild:(hub_name:string)=>void;
    public connect_hub(hub_name:string, hub_type:string, url:string)
    {
        let that = this;
        this._conn.connect(url, (ch)=>{
            console.log("connect hub url:" + url);

            this.add_chs.push(ch);

            let _hubproxy = new hubproxy(hub_name, hub_type, ch);
            _hubproxy.onHubDisconnect = (ch) =>
            {
                that.remove_chs.push(ch);

                if (that._ch_hubproxy_set.has(ch)) {
                    let _proxy = that._ch_hubproxy_set.get(ch);
                    
                    that._hubproxy_set.delete(_proxy._hub_name);
                    that._ch_hubproxy_set.delete(ch);
                }

                if (that.onHubDisConnect) {
                    that.onHubDisConnect(hub_name);
                }
            };
            _hubproxy.onHubTime = (hub_name, tick)=>{
                that.onHubTime(hub_name, tick);
            }
            _hubproxy.connect_hub(that.uuid);
            
            that._hubproxy_set.set(hub_name, _hubproxy);
            that._ch_hubproxy_set.set(ch, _hubproxy);

            if (that.onHubConnect){
                that.onHubConnect(hub_name);
            }
        });
    }

    public poll()
    {
        let tick_begin = new Date().getTime();

        for (let ch of this.add_chs)
        {
            this.chs.push(ch);
        }
        this.add_chs = [];

        for (let ch of this.chs)
        {
            while (true)
            {
                let ev:any = ch.pop();
                if (!ev)
                {
                    break;
                }
                abelkhan._modulemng.process_event(ch, ev);
            }
        }

        var _new_event_set = [];
        for(let _ch of this.chs)
        {
            var in_remove_event = false;
            for(let ch of this.remove_chs)
            {
                if (_ch === ch)
                {
                    in_remove_event = true;
                    break;
                }
            }
            if (!in_remove_event)
            {
                _new_event_set.push(_ch);
            }
        }
        this.chs = _new_event_set;
        this.remove_chs = [];

        let tick_end = new Date().getTime();

        return tick_end - tick_begin;
    }

}