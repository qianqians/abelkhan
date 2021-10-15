import abelkhan = require("abelkhan");
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
/*this cb code is codegen by abelkhan for ts*/
export class gate_call_client_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("gate_call_client_rsp_cb");
        modules.reg_module(this);

    }
}

export let rsp_cb_gate_call_client_handle : gate_call_client_rsp_cb | null = null;
export class gate_call_client_caller extends abelkhan.Icaller {
    private uuid_b84dd831_2e79_3280_a337_a69dd489e75f : number = Math.round(Math.random() * Number.MAX_VALUE);

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

    public call_client(hub_name:string, rpc_argv:Uint8Array){
        let _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab:any[] = [];
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(hub_name);
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(rpc_argv);
        this.call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
    }

}
/*this module code is codegen by abelkhan codegen for typescript*/
export class gate_call_client_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("gate_call_client");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("ntf_cuuid", this.ntf_cuuid.bind(this));
        this.reg_method("call_client", this.call_client.bind(this));

        this.cb_ntf_cuuid = null;
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

    public cb_call_client : (hub_name:string, rpc_argv:Uint8Array)=>void | null;
    call_client(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_call_client){
            this.cb_call_client.apply(null, _argv_);
        }
    }

}
