import * as abelkhan from "./abelkhan";
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
/*this cb code is codegen by abelkhan for ts*/
export class gate_call_client_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("gate_call_client_rsp_cb");
    }
}

export let rsp_cb_gate_call_client_handle : gate_call_client_rsp_cb | null = null;
export class gate_call_client_caller extends abelkhan.Icaller {
    private uuid_b84dd831_2e79_3280_a337_a69dd489e75f : number = Math.round(Math.random() * 1000);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("gate_call_client", _ch);
        if (rsp_cb_gate_call_client_handle == null){
            rsp_cb_gate_call_client_handle = new gate_call_client_rsp_cb(modules);
        }
    }

    public ntf_cuuid(cuuid:string){
        let _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1:any[] = [];
        _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.push(cuuid);
        this.call_module_method("gate_call_client_ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
    }

    public call_client(hub_name:string, rpc_argv:Uint8Array){
        let _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab:any[] = [];
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(hub_name);
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(rpc_argv);
        this.call_module_method("gate_call_client_call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
    }

    public migrate_client_start(src_hub:string, target_hub:string){
        let _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee:any[] = [];
        _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee.push(src_hub);
        _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee.push(target_hub);
        this.call_module_method("gate_call_client_migrate_client_start", _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee);
    }

    public migrate_client_done(src_hub:string, target_hub:string){
        let _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c:any[] = [];
        _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c.push(src_hub);
        _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c.push(target_hub);
        this.call_module_method("gate_call_client_migrate_client_done", _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c);
    }

    public hub_loss(hub_name:string){
        let _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099:any[] = [];
        _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099.push(hub_name);
        this.call_module_method("gate_call_client_hub_loss", _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099);
    }

}
/*this module code is codegen by abelkhan codegen for typescript*/
export class gate_call_client_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("gate_call_client");
        this.modules = modules;
        this.modules.reg_method("gate_call_client_ntf_cuuid", [this, this.ntf_cuuid.bind(this)]);
        this.modules.reg_method("gate_call_client_call_client", [this, this.call_client.bind(this)]);
        this.modules.reg_method("gate_call_client_migrate_client_start", [this, this.migrate_client_start.bind(this)]);
        this.modules.reg_method("gate_call_client_migrate_client_done", [this, this.migrate_client_done.bind(this)]);
        this.modules.reg_method("gate_call_client_hub_loss", [this, this.hub_loss.bind(this)]);

        this.cb_ntf_cuuid = null;
        this.cb_call_client = null;
        this.cb_migrate_client_start = null;
        this.cb_migrate_client_done = null;
        this.cb_hub_loss = null;
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

    public cb_migrate_client_start : (src_hub:string, target_hub:string)=>void | null;
    migrate_client_start(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_migrate_client_start){
            this.cb_migrate_client_start.apply(null, _argv_);
        }
    }

    public cb_migrate_client_done : (src_hub:string, target_hub:string)=>void | null;
    migrate_client_done(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_migrate_client_done){
            this.cb_migrate_client_done.apply(null, _argv_);
        }
    }

    public cb_hub_loss : (hub_name:string)=>void | null;
    hub_loss(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_hub_loss){
            this.cb_hub_loss.apply(null, _argv_);
        }
    }

}
