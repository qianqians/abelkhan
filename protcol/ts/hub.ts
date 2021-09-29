import abelkhan = require("abelkhan");
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
export class hub_call_hub_reg_hub_cb{
    private cb_uuid : number;
    private module_rsp_cb : hub_call_hub_rsp_cb;

    public event_reg_hub_handle_cb : ()=>void | null;
    public event_reg_hub_handle_err : ()=>void | null;
    public event_reg_hub_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : hub_call_hub_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_reg_hub_handle_cb = null;
        this.event_reg_hub_handle_err = null;
        this.event_reg_hub_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_reg_hub_handle_cb = _cb;
        this.event_reg_hub_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.reg_hub_timeout(this.cb_uuid); }, tick);
        this.event_reg_hub_handle_timeout = timeout_cb;
    }

}

/*this cb code is codegen by abelkhan for ts*/
export class hub_call_hub_rsp_cb extends abelkhan.Imodule {
    public map_reg_hub:Map<number, hub_call_hub_reg_hub_cb>;
    constructor(modules:abelkhan.modulemng){
        super("hub_call_hub_rsp_cb");
        modules.reg_module(this);

        this.map_reg_hub = new Map<number, hub_call_hub_reg_hub_cb>();
        this.reg_method("reg_hub_rsp", this.reg_hub_rsp.bind(this));
        this.reg_method("reg_hub_err", this.reg_hub_err.bind(this));
    }
    public reg_hub_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [];
        var rsp = this.try_get_and_del_reg_hub_cb(uuid);
        if (rsp && rsp.event_reg_hub_handle_cb) {
            rsp.event_reg_hub_handle_cb.apply(null, _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }
    }

    public reg_hub_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [];
        var rsp = this.try_get_and_del_reg_hub_cb(uuid);
        if (rsp && rsp.event_reg_hub_handle_err) {
            rsp.event_reg_hub_handle_err.apply(null, _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }
    }

    public reg_hub_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_reg_hub_cb(cb_uuid);
        if (rsp){
            if (rsp.event_reg_hub_handle_timeout) {
                rsp.event_reg_hub_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_reg_hub_cb(uuid : number){
        var rsp = this.map_reg_hub.get(uuid);
        this.map_reg_hub.delete(uuid);
        return rsp;
    }

}

export let rsp_cb_hub_call_hub_handle : hub_call_hub_rsp_cb | null = null;
export class hub_call_hub_caller extends abelkhan.Icaller {
    private uuid : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("hub_call_hub", _ch);
        if (rsp_cb_hub_call_hub_handle == null){
            rsp_cb_hub_call_hub_handle = new hub_call_hub_rsp_cb(modules);
        }
    }

    public reg_hub(hub_name:string, hub_type:string){
        let uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = this.uuid++;

        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106];
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_name);
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_type);
        this.call_module_method("reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

        let cb_reg_hub_obj = new hub_call_hub_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_hub_handle);
        if (rsp_cb_hub_call_hub_handle){
            rsp_cb_hub_call_hub_handle.map_reg_hub.set(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
        }
        return cb_reg_hub_obj;
    }

    public hub_call_hub_mothed(module:string, func:string, argv:Uint8Array){
        let _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e:any[] = [];
        _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.push(module);
        _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.push(func);
        _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.push(argv);
        this.call_module_method("hub_call_hub_mothed", _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e);
    }

}
/*this cb code is codegen by abelkhan for ts*/
export class gate_call_hub_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("gate_call_hub_rsp_cb");
        modules.reg_module(this);

    }
}

export let rsp_cb_gate_call_hub_handle : gate_call_hub_rsp_cb | null = null;
export class gate_call_hub_caller extends abelkhan.Icaller {
    private uuid : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("gate_call_hub", _ch);
        if (rsp_cb_gate_call_hub_handle == null){
            rsp_cb_gate_call_hub_handle = new gate_call_hub_rsp_cb(modules);
        }
    }

    public client_call_hub(client_uuid:string, rpc_argv:Uint8Array){
        let _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263:any[] = [];
        _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.push(client_uuid);
        _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.push(rpc_argv);
        this.call_module_method("client_call_hub", _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263);
    }

}
/*this cb code is codegen by abelkhan for ts*/
export class client_call_hub_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("client_call_hub_rsp_cb");
        modules.reg_module(this);

    }
}

export let rsp_cb_client_call_hub_handle : client_call_hub_rsp_cb | null = null;
export class client_call_hub_caller extends abelkhan.Icaller {
    private uuid : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("client_call_hub", _ch);
        if (rsp_cb_client_call_hub_handle == null){
            rsp_cb_client_call_hub_handle = new client_call_hub_rsp_cb(modules);
        }
    }

    public connect_hub(client_uuid:string){
        let _argv_dc2ee339_bef5_3af9_a492_592ba4f08559:any[] = [];
        _argv_dc2ee339_bef5_3af9_a492_592ba4f08559.push(client_uuid);
        this.call_module_method("connect_hub", _argv_dc2ee339_bef5_3af9_a492_592ba4f08559);
    }

    public call_hub(module:string, func:string, argv:Uint8Array){
        let _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3:any[] = [];
        _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.push(module);
        _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.push(func);
        _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.push(argv);
        this.call_module_method("call_hub", _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3);
    }

}
/*this cb code is codegen by abelkhan for ts*/
export class hub_call_client_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("hub_call_client_rsp_cb");
        modules.reg_module(this);

    }
}

export let rsp_cb_hub_call_client_handle : hub_call_client_rsp_cb | null = null;
export class hub_call_client_caller extends abelkhan.Icaller {
    private uuid : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("hub_call_client", _ch);
        if (rsp_cb_hub_call_client_handle == null){
            rsp_cb_hub_call_client_handle = new hub_call_client_rsp_cb(modules);
        }
    }

    public call_client(module:string, func:string, argv:Uint8Array){
        let _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab:any[] = [];
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(module);
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(func);
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(argv);
        this.call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
    }

}
/*this module code is codegen by abelkhan codegen for typescript*/
export class hub_call_hub_reg_hub_rsp extends abelkhan.Icaller {
    private uuid : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("hub_call_hub_rsp_cb", _ch);
        this.uuid = _uuid;
    }

    public rsp(){
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [this.uuid];
        this.call_module_method("reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
    }

    public err(){
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [this.uuid];
        this.call_module_method("reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
    }

}

export class hub_call_hub_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("hub_call_hub");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("reg_hub", this.reg_hub.bind(this));
        this.reg_method("hub_call_hub_mothed", this.hub_call_hub_mothed.bind(this));
        this.cb_reg_hub = null;

        this.cb_hub_call_hub_mothed = null;

    }

    public cb_reg_hub : (hub_name:string, hub_type:string)=>void | null;
    reg_hub(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        this.rsp = new hub_call_hub_reg_hub_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reg_hub){
            this.cb_reg_hub.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_hub_call_hub_mothed : (module:string, func:string, argv:Uint8Array)=>void | null;
    hub_call_hub_mothed(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        if (this.cb_hub_call_hub_mothed){
            this.cb_hub_call_hub_mothed.apply(null, _argv_);
        }
    }

}
export class gate_call_hub_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("gate_call_hub");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("client_call_hub", this.client_call_hub.bind(this));
        this.cb_client_call_hub = null;

    }

    public cb_client_call_hub : (client_uuid:string, rpc_argv:Uint8Array)=>void | null;
    client_call_hub(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_client_call_hub){
            this.cb_client_call_hub.apply(null, _argv_);
        }
    }

}
export class client_call_hub_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("client_call_hub");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("connect_hub", this.connect_hub.bind(this));
        this.reg_method("call_hub", this.call_hub.bind(this));
        this.cb_connect_hub = null;

        this.cb_call_hub = null;

    }

    public cb_connect_hub : (client_uuid:string)=>void | null;
    connect_hub(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_connect_hub){
            this.cb_connect_hub.apply(null, _argv_);
        }
    }

    public cb_call_hub : (module:string, func:string, argv:Uint8Array)=>void | null;
    call_hub(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        if (this.cb_call_hub){
            this.cb_call_hub.apply(null, _argv_);
        }
    }

}
export class hub_call_client_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("hub_call_client");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("call_client", this.call_client.bind(this));
        this.cb_call_client = null;

    }

    public cb_call_client : (module:string, func:string, argv:Uint8Array)=>void | null;
    call_client(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        if (this.cb_call_client){
            this.cb_call_client.apply(null, _argv_);
        }
    }

}
