import { encode } from "./msgpack";

export interface Ichannel{
    disconnect : () => void;
    send : (_event:Uint8Array) => void;
    recv : (evt:ArrayBuffer) => void;
    pop : () => any[];
}

export class Icaller{
    private module_name : String;
    private ch : Ichannel;
    constructor(_module_name:String, _ch:Ichannel){
        this.module_name = _module_name;
        this.ch = _ch;
    }

    public call_module_method(method_name:String, argvs:any){
        var _event = encode([this.module_name, method_name, argvs]);
        
        var send_data = new Uint8Array(4 + _event.length);
        send_data[0] = _event.length & 0xff;
        send_data[1] = (_event.length >> 8) & 0xff;
        send_data[2] = (_event.length >> 16) & 0xff;
        send_data[3] = (_event.length >> 24) & 0xff;
        send_data.set(_event, 4);

        this.ch.send(send_data); 
    }
}

export class Imodule{
    public module_name : String;
    constructor(_module_name:String){
        this.module_name = _module_name;
    }

    private methods = new Map<String, any>();
    public reg_method(method_name:String, method:any){
        this.methods.set(method_name, method);
    }
    
    public current_ch:Ichannel = null;
    public rsp:any = null;
    public process_event(_ch:Ichannel, _event:any){
        this.current_ch = _ch;
        this.methods.get(_event[1]).call(this, _event[2]);
        this.current_ch = null;
    }

}

export class modulemng{
    private module_set = new Map<String, Imodule>();

    public reg_module(_module:Imodule){
        this.module_set.set(_module.module_name, _module);
    }

    public process_event(_ch:Ichannel, _event:any){
        this.module_set.get(_event[0]).process_event(_ch, _event);
    }
}
export let _modulemng = new modulemng();