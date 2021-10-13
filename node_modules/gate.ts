import abelkhan = require("abelkhan");
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
export class hub_info
{
    public hub_name : string;
    public hub_type : string;

    constructor(){
    }
}

export function hub_info_to_protcol(_struct:hub_info){
    let _protocol:any[] = [];
    _protocol.push(_struct.hub_name);
    _protocol.push(_struct.hub_type);
    return _protocol;
}

export function protcol_to_hub_info(_protocol:any[]){
    let _struct = new hub_info();
    _struct.hub_name = _protocol[0] as string;
    _struct.hub_type = _protocol[1] as string;
    return _struct;
}

/*this caller code is codegen by abelkhan codegen for typescript*/
export class hub_call_gate_reg_hub_cb{
    private cb_uuid : number;
    private module_rsp_cb : hub_call_gate_rsp_cb;

    public event_reg_hub_handle_cb : ()=>void | null;
    public event_reg_hub_handle_err : ()=>void | null;
    public event_reg_hub_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : hub_call_gate_rsp_cb){
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
export class hub_call_gate_rsp_cb extends abelkhan.Imodule {
    public map_reg_hub:Map<number, hub_call_gate_reg_hub_cb>;
    constructor(modules:abelkhan.modulemng){
        super("hub_call_gate_rsp_cb");
        modules.reg_module(this);

        this.map_reg_hub = new Map<number, hub_call_gate_reg_hub_cb>();
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

export let rsp_cb_hub_call_gate_handle : hub_call_gate_rsp_cb | null = null;
export class hub_call_gate_caller extends abelkhan.Icaller {
    private uuid_9796175c_1119_3833_bf31_5ee139b40edc : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("hub_call_gate", _ch);
        if (rsp_cb_hub_call_gate_handle == null){
            rsp_cb_hub_call_gate_handle = new hub_call_gate_rsp_cb(modules);
        }
    }

    public reg_hub(hub_name:string, hub_type:string){
        let uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = this.uuid_9796175c_1119_3833_bf31_5ee139b40edc++;

        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106];
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_name);
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_type);
        this.call_module_method("reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

        let cb_reg_hub_obj = new hub_call_gate_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_gate_handle);
        if (rsp_cb_hub_call_gate_handle){
            rsp_cb_hub_call_gate_handle.map_reg_hub.set(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
        }
        return cb_reg_hub_obj;
    }

    public disconnect_client(client_uuid:string){
        let _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85:any[] = [];
        _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85.push(client_uuid);
        this.call_module_method("disconnect_client", _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85);
    }

    public forward_hub_call_client(client_uuid:string, rpc_argv:Uint8Array){
        let _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1:any[] = [];
        _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.push(client_uuid);
        _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.push(rpc_argv);
        this.call_module_method("forward_hub_call_client", _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1);
    }

    public forward_hub_call_group_client(client_uuids:string[], rpc_argv:Uint8Array){
        let _argv_374b012d_0718_3f84_91f0_784b12f8189c:any[] = [];
        let _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1:any[] = [];
        for(let v_dfd11414_89c9_5adb_8977_69b93b30195b of client_uuids){
            _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1.push(v_dfd11414_89c9_5adb_8977_69b93b30195b);
        }
        _argv_374b012d_0718_3f84_91f0_784b12f8189c.push(_array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1);
        _argv_374b012d_0718_3f84_91f0_784b12f8189c.push(rpc_argv);
        this.call_module_method("forward_hub_call_group_client", _argv_374b012d_0718_3f84_91f0_784b12f8189c);
    }

    public forward_hub_call_global_client(rpc_argv:Uint8Array){
        let _argv_f69241c3_642a_3b51_bb37_cf638176493a:any[] = [];
        _argv_f69241c3_642a_3b51_bb37_cf638176493a.push(rpc_argv);
        this.call_module_method("forward_hub_call_global_client", _argv_f69241c3_642a_3b51_bb37_cf638176493a);
    }

}
export class client_call_gate_heartbeats_cb{
    private cb_uuid : number;
    private module_rsp_cb : client_call_gate_rsp_cb;

    public event_heartbeats_handle_cb : (timetmp:number)=>void | null;
    public event_heartbeats_handle_err : ()=>void | null;
    public event_heartbeats_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : client_call_gate_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_heartbeats_handle_cb = null;
        this.event_heartbeats_handle_err = null;
        this.event_heartbeats_handle_timeout = null;
    }

    callBack(_cb:(timetmp:number)=>void, _err:()=>void)
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

export class client_call_gate_get_hub_info_cb{
    private cb_uuid : number;
    private module_rsp_cb : client_call_gate_rsp_cb;

    public event_get_hub_info_handle_cb : (hub_info:hub_info[])=>void | null;
    public event_get_hub_info_handle_err : ()=>void | null;
    public event_get_hub_info_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : client_call_gate_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_hub_info_handle_cb = null;
        this.event_get_hub_info_handle_err = null;
        this.event_get_hub_info_handle_timeout = null;
    }

    callBack(_cb:(hub_info:hub_info[])=>void, _err:()=>void)
    {
        this.event_get_hub_info_handle_cb = _cb;
        this.event_get_hub_info_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.get_hub_info_timeout(this.cb_uuid); }, tick);
        this.event_get_hub_info_handle_timeout = timeout_cb;
    }

}

/*this cb code is codegen by abelkhan for ts*/
export class client_call_gate_rsp_cb extends abelkhan.Imodule {
    public map_heartbeats:Map<number, client_call_gate_heartbeats_cb>;
    public map_get_hub_info:Map<number, client_call_gate_get_hub_info_cb>;
    constructor(modules:abelkhan.modulemng){
        super("client_call_gate_rsp_cb");
        modules.reg_module(this);

        this.map_heartbeats = new Map<number, client_call_gate_heartbeats_cb>();
        this.reg_method("heartbeats_rsp", this.heartbeats_rsp.bind(this));
        this.reg_method("heartbeats_err", this.heartbeats_err.bind(this));
        this.map_get_hub_info = new Map<number, client_call_gate_get_hub_info_cb>();
        this.reg_method("get_hub_info_rsp", this.get_hub_info_rsp.bind(this));
        this.reg_method("get_hub_info_err", this.get_hub_info_err.bind(this));
    }
    public heartbeats_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [];
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push(inArray[1]);
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

    public get_hub_info_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24:any[] = [];
        let _array_53b78086_1765_5879_87b4_63333838766a:any[] = [];        for(let v_447f0ba1_16b5_5b5f_b754_ee583fc6f6a8 of inArray[1]){
            _array_53b78086_1765_5879_87b4_63333838766a.push(protcol_to_hub_info(v_447f0ba1_16b5_5b5f_b754_ee583fc6f6a8));
        }
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push(_array_53b78086_1765_5879_87b4_63333838766a);
        var rsp = this.try_get_and_del_get_hub_info_cb(uuid);
        if (rsp && rsp.event_get_hub_info_handle_cb) {
            rsp.event_get_hub_info_handle_cb.apply(null, _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }
    }

    public get_hub_info_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24:any[] = [];
        var rsp = this.try_get_and_del_get_hub_info_cb(uuid);
        if (rsp && rsp.event_get_hub_info_handle_err) {
            rsp.event_get_hub_info_handle_err.apply(null, _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }
    }

    public get_hub_info_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_get_hub_info_cb(cb_uuid);
        if (rsp){
            if (rsp.event_get_hub_info_handle_timeout) {
                rsp.event_get_hub_info_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_get_hub_info_cb(uuid : number){
        var rsp = this.map_get_hub_info.get(uuid);
        this.map_get_hub_info.delete(uuid);
        return rsp;
    }

}

export let rsp_cb_client_call_gate_handle : client_call_gate_rsp_cb | null = null;
export class client_call_gate_caller extends abelkhan.Icaller {
    private uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2 : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("client_call_gate", _ch);
        if (rsp_cb_client_call_gate_handle == null){
            rsp_cb_client_call_gate_handle = new client_call_gate_rsp_cb(modules);
        }
    }

    public heartbeats(){
        let uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = this.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2++;

        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [uuid_a514ca5f_2c67_5668_aac0_354397bdce36];
        this.call_module_method("heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);

        let cb_heartbeats_obj = new client_call_gate_heartbeats_cb(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, rsp_cb_client_call_gate_handle);
        if (rsp_cb_client_call_gate_handle){
            rsp_cb_client_call_gate_handle.map_heartbeats.set(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj);
        }
        return cb_heartbeats_obj;
    }

    public get_hub_info(hub_type:string){
        let uuid_e9d2753f_7d38_512d_80ff_7aae13508048 = this.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2++;

        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24:any[] = [uuid_e9d2753f_7d38_512d_80ff_7aae13508048];
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push(hub_type);
        this.call_module_method("get_hub_info", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);

        let cb_get_hub_info_obj = new client_call_gate_get_hub_info_cb(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, rsp_cb_client_call_gate_handle);
        if (rsp_cb_client_call_gate_handle){
            rsp_cb_client_call_gate_handle.map_get_hub_info.set(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, cb_get_hub_info_obj);
        }
        return cb_get_hub_info_obj;
    }

    public forward_client_call_hub(hub_name:string, rpc_argv:Uint8Array){
        let _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5:any[] = [];
        _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.push(hub_name);
        _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.push(rpc_argv);
        this.call_module_method("forward_client_call_hub", _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5);
    }

}
/*this module code is codegen by abelkhan codegen for typescript*/
export class hub_call_gate_reg_hub_rsp extends abelkhan.Icaller {
    private uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("hub_call_gate_rsp_cb", _ch);
        this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid;
    }

    public rsp(){
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67];
        this.call_module_method("reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
    }

    public err(){
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67];
        this.call_module_method("reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
    }

}

export class hub_call_gate_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("hub_call_gate");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("reg_hub", this.reg_hub.bind(this));
        this.reg_method("disconnect_client", this.disconnect_client.bind(this));
        this.reg_method("forward_hub_call_client", this.forward_hub_call_client.bind(this));
        this.reg_method("forward_hub_call_group_client", this.forward_hub_call_group_client.bind(this));
        this.reg_method("forward_hub_call_global_client", this.forward_hub_call_global_client.bind(this));

        this.cb_reg_hub = null;
        this.cb_disconnect_client = null;
        this.cb_forward_hub_call_client = null;
        this.cb_forward_hub_call_group_client = null;
        this.cb_forward_hub_call_global_client = null;
    }

    public cb_reg_hub : (hub_name:string, hub_type:string)=>void | null;
    reg_hub(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        this.rsp = new hub_call_gate_reg_hub_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reg_hub){
            this.cb_reg_hub.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_disconnect_client : (client_uuid:string)=>void | null;
    disconnect_client(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_disconnect_client){
            this.cb_disconnect_client.apply(null, _argv_);
        }
    }

    public cb_forward_hub_call_client : (client_uuid:string, rpc_argv:Uint8Array)=>void | null;
    forward_hub_call_client(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_forward_hub_call_client){
            this.cb_forward_hub_call_client.apply(null, _argv_);
        }
    }

    public cb_forward_hub_call_group_client : (client_uuids:string[], rpc_argv:Uint8Array)=>void | null;
    forward_hub_call_group_client(inArray:any[]){
        let _argv_:any[] = [];
        let _array_:any[] = [];
        for(let v_ of inArray[0]){
            _array_.push(v_);
        }
        _argv_.push(_array_);
        _argv_.push(inArray[1]);
        if (this.cb_forward_hub_call_group_client){
            this.cb_forward_hub_call_group_client.apply(null, _argv_);
        }
    }

    public cb_forward_hub_call_global_client : (rpc_argv:Uint8Array)=>void | null;
    forward_hub_call_global_client(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_forward_hub_call_global_client){
            this.cb_forward_hub_call_global_client.apply(null, _argv_);
        }
    }

}
export class client_call_gate_heartbeats_rsp extends abelkhan.Icaller {
    private uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("client_call_gate_rsp_cb", _ch);
        this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 = _uuid;
    }

    public rsp(timetmp:number){
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56];
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push(timetmp);
        this.call_module_method("heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
    }

    public err(){
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4:any[] = [this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56];
        this.call_module_method("heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
    }

}

export class client_call_gate_get_hub_info_rsp extends abelkhan.Icaller {
    private uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("client_call_gate_rsp_cb", _ch);
        this.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b = _uuid;
    }

    public rsp(hub_info:hub_info[]){
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24:any[] = [this.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b];
        let _array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2:any[] = [];        for(let v_72192cc7_d049_3653_a25b_4eaf8d18d7e2 of hub_info){
            _array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.push(hub_info_to_protcol(v_72192cc7_d049_3653_a25b_4eaf8d18d7e2));
        }
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push(_array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2);
        this.call_module_method("get_hub_info_rsp", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
    }

    public err(){
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24:any[] = [this.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b];
        this.call_module_method("get_hub_info_err", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
    }

}

export class client_call_gate_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("client_call_gate");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("heartbeats", this.heartbeats.bind(this));
        this.reg_method("get_hub_info", this.get_hub_info.bind(this));
        this.reg_method("forward_client_call_hub", this.forward_client_call_hub.bind(this));

        this.cb_heartbeats = null;
        this.cb_get_hub_info = null;
        this.cb_forward_client_call_hub = null;
    }

    public cb_heartbeats : ()=>void | null;
    heartbeats(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        this.rsp = new client_call_gate_heartbeats_rsp(this.current_ch, _cb_uuid);
        if (this.cb_heartbeats){
            this.cb_heartbeats.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_get_hub_info : (hub_type:string)=>void | null;
    get_hub_info(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        this.rsp = new client_call_gate_get_hub_info_rsp(this.current_ch, _cb_uuid);
        if (this.cb_get_hub_info){
            this.cb_get_hub_info.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_forward_client_call_hub : (hub_name:string, rpc_argv:Uint8Array)=>void | null;
    forward_client_call_hub(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_forward_client_call_hub){
            this.cb_forward_client_call_hub.apply(null, _argv_);
        }
    }

}
