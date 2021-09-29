import abelkhan = require("abelkhan");
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
export class gate_call_client_heartbeats_cb{
    private cb_uuid : number;
    private module_rsp_cb : gate_call_client_rsp_cb;

    public event_heartbeats_handle_cb : ()=>void | null;
    public event_heartbeats_handle_err : ()=>void | null;
    public event_heartbeats_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : gate_call_client_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_heartbeats_handle_cb = null;
        this.event_heartbeats_handle_err = null;
        this.event_heartbeats_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_heartbeats_handle_cb = _cb;
        this.event_heartbeats_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.heartbeats_timeout(this.cb_uuid); }, tick);
        this.event_heartbeats_handle_timeout = timeout_cb;
    }

}

/*this cb code is codegen by abelkhan for ts*/
export class gate_call_client_rsp_cb extends abelkhan.Imodule {
    public map_heartbeats:Map<number, gate_call_client_heartbeats_cb>;
    constructor(modules:abelkhan.modulemng){
        super("gate_call_client_rsp_cb");
        modules.reg_module(this);

        this.map_heartbeats = new Map<number, gate_call_client_heartbeats_cb>();
        this.reg_method("heartbeats_rsp", this.heartbeats_rsp.bind(this));
        this.reg_method("heartbeats_err", this.heartbeats_err.bind(this));
    }
    public heartbeats_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [];
        var rsp = this.try_get_and_del_heartbeats_cb(uuid);
        if (rsp && rsp.event_heartbeats_handle_cb) {
            rsp.event_heartbeats_handle_cb.apply(null, _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }
    }

    public heartbeats_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [];
        var rsp = this.try_get_and_del_heartbeats_cb(uuid);
        if (rsp && rsp.event_heartbeats_handle_err) {
            rsp.event_heartbeats_handle_err.apply(null, _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }
    }

    public heartbeats_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_heartbeats_cb(cb_uuid);
        if (rsp){
            if (rsp.event_heartbeats_handle_timeout) {
                rsp.event_heartbeats_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_heartbeats_cb(uuid : number){
        var rsp = this.map_heartbeats.get(uuid);
        this.map_heartbeats.delete(uuid);
        return rsp;
    }

}

export let rsp_cb_gate_call_client_handle : gate_call_client_rsp_cb | null = null;
export class gate_call_client_caller extends abelkhan.Icaller {
    private uuid : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("gate_call_client", _ch);
        if (rsp_cb_gate_call_client_handle == null){
            rsp_cb_gate_call_client_handle = new gate_call_client_rsp_cb(modules);
        }
    }

    public ntf_cuuid(cuuid:string){
        let _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1:any[] = [];
        _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.push(cuuid);
        this.call_module_method("ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
    }

    public heartbeats(timetmp:number){
        let uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = this.uuid++;

        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [uuid_a514ca5f_2c67_5668_aac0_354397bdce36];
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push(timetmp);
        this.call_module_method("heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);

        let cb_heartbeats_obj = new gate_call_client_heartbeats_cb(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, rsp_cb_gate_call_client_handle);
        if (rsp_cb_gate_call_client_handle){
            rsp_cb_gate_call_client_handle.map_heartbeats.set(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj);
        }
        return cb_heartbeats_obj;
    }

    public call_client(rpc_argv:Uint8Array){
        let _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab:any[] = [];
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(rpc_argv);
        this.call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
    }

}
/*this module code is codegen by abelkhan codegen for typescript*/
export class gate_call_client_heartbeats_rsp extends abelkhan.Icaller {
    private uuid : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("gate_call_client_rsp_cb", _ch);
        this.uuid = _uuid;
    }

    public rsp(){
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [this.uuid];
        this.call_module_method("heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
    }

    public err(){
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [this.uuid];
        this.call_module_method("heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
    }

}

export class gate_call_client_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("gate_call_client");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("ntf_cuuid", this.ntf_cuuid.bind(this));
        this.reg_method("heartbeats", this.heartbeats.bind(this));
        this.reg_method("call_client", this.call_client.bind(this));
        this.cb_ntf_cuuid = null;

        this.cb_heartbeats = null;

        this.cb_call_client = null;

    }

    public cb_ntf_cuuid : (cuuid:string)=>void | null;
    ntf_cuuid(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_ntf_cuuid){
            this.cb_ntf_cuuid.apply(null, _argv_);
        }
    }

    public cb_heartbeats : (timetmp:number)=>void | null;
    heartbeats(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        this.rsp = new gate_call_client_heartbeats_rsp(this.current_ch, _cb_uuid);
        if (this.cb_heartbeats){
            this.cb_heartbeats.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_call_client : (rpc_argv:Uint8Array)=>void | null;
    call_client(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_call_client){
            this.cb_call_client.apply(null, _argv_);
        }
    }

}
