import * as abelkhan from "./abelkhan";
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
export class center_reg_server_mq_cb{
    private cb_uuid : number;
    private module_rsp_cb : center_rsp_cb;

    public event_reg_server_mq_handle_cb : ()=>void | null;
    public event_reg_server_mq_handle_err : ()=>void | null;
    public event_reg_server_mq_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : center_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_reg_server_mq_handle_cb = null;
        this.event_reg_server_mq_handle_err = null;
        this.event_reg_server_mq_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_reg_server_mq_handle_cb = _cb;
        this.event_reg_server_mq_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.reg_server_mq_timeout(this.cb_uuid); }, tick);
        this.event_reg_server_mq_handle_timeout = timeout_cb;
    }

}

export class center_reconn_reg_server_mq_cb{
    private cb_uuid : number;
    private module_rsp_cb : center_rsp_cb;

    public event_reconn_reg_server_mq_handle_cb : ()=>void | null;
    public event_reconn_reg_server_mq_handle_err : ()=>void | null;
    public event_reconn_reg_server_mq_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : center_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_reconn_reg_server_mq_handle_cb = null;
        this.event_reconn_reg_server_mq_handle_err = null;
        this.event_reconn_reg_server_mq_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_reconn_reg_server_mq_handle_cb = _cb;
        this.event_reconn_reg_server_mq_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.reconn_reg_server_mq_timeout(this.cb_uuid); }, tick);
        this.event_reconn_reg_server_mq_handle_timeout = timeout_cb;
    }

}

export class center_heartbeat_cb{
    private cb_uuid : number;
    private module_rsp_cb : center_rsp_cb;

    public event_heartbeat_handle_cb : ()=>void | null;
    public event_heartbeat_handle_err : ()=>void | null;
    public event_heartbeat_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : center_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_heartbeat_handle_cb = null;
        this.event_heartbeat_handle_err = null;
        this.event_heartbeat_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_heartbeat_handle_cb = _cb;
        this.event_heartbeat_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.heartbeat_timeout(this.cb_uuid); }, tick);
        this.event_heartbeat_handle_timeout = timeout_cb;
    }

}

/*this cb code is codegen by abelkhan for ts*/
export class center_rsp_cb extends abelkhan.Imodule {
    public map_reg_server_mq:Map<number, center_reg_server_mq_cb>;
    public map_reconn_reg_server_mq:Map<number, center_reconn_reg_server_mq_cb>;
    public map_heartbeat:Map<number, center_heartbeat_cb>;
    constructor(modules:abelkhan.modulemng){
        super("center_rsp_cb");
        this.map_reg_server_mq = new Map<number, center_reg_server_mq_cb>();
        modules.reg_method("center_rsp_cb_reg_server_mq_rsp", [this, this.reg_server_mq_rsp.bind(this)]);
        modules.reg_method("center_rsp_cb_reg_server_mq_err", [this, this.reg_server_mq_err.bind(this)]);
        this.map_reconn_reg_server_mq = new Map<number, center_reconn_reg_server_mq_cb>();
        modules.reg_method("center_rsp_cb_reconn_reg_server_mq_rsp", [this, this.reconn_reg_server_mq_rsp.bind(this)]);
        modules.reg_method("center_rsp_cb_reconn_reg_server_mq_err", [this, this.reconn_reg_server_mq_err.bind(this)]);
        this.map_heartbeat = new Map<number, center_heartbeat_cb>();
        modules.reg_method("center_rsp_cb_heartbeat_rsp", [this, this.heartbeat_rsp.bind(this)]);
        modules.reg_method("center_rsp_cb_heartbeat_err", [this, this.heartbeat_err.bind(this)]);
    }
    public reg_server_mq_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2:any[] = [];
        var rsp = this.try_get_and_del_reg_server_mq_cb(uuid);
        if (rsp && rsp.event_reg_server_mq_handle_cb) {
            rsp.event_reg_server_mq_handle_cb.apply(null, _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);
        }
    }

    public reg_server_mq_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2:any[] = [];
        var rsp = this.try_get_and_del_reg_server_mq_cb(uuid);
        if (rsp && rsp.event_reg_server_mq_handle_err) {
            rsp.event_reg_server_mq_handle_err.apply(null, _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);
        }
    }

    public reg_server_mq_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_reg_server_mq_cb(cb_uuid);
        if (rsp){
            if (rsp.event_reg_server_mq_handle_timeout) {
                rsp.event_reg_server_mq_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_reg_server_mq_cb(uuid : number){
        var rsp = this.map_reg_server_mq.get(uuid);
        this.map_reg_server_mq.delete(uuid);
        return rsp;
    }

    public reconn_reg_server_mq_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_a018be20_2048_315d_9832_8120b194980f:any[] = [];
        var rsp = this.try_get_and_del_reconn_reg_server_mq_cb(uuid);
        if (rsp && rsp.event_reconn_reg_server_mq_handle_cb) {
            rsp.event_reconn_reg_server_mq_handle_cb.apply(null, _argv_a018be20_2048_315d_9832_8120b194980f);
        }
    }

    public reconn_reg_server_mq_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_a018be20_2048_315d_9832_8120b194980f:any[] = [];
        var rsp = this.try_get_and_del_reconn_reg_server_mq_cb(uuid);
        if (rsp && rsp.event_reconn_reg_server_mq_handle_err) {
            rsp.event_reconn_reg_server_mq_handle_err.apply(null, _argv_a018be20_2048_315d_9832_8120b194980f);
        }
    }

    public reconn_reg_server_mq_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_reconn_reg_server_mq_cb(cb_uuid);
        if (rsp){
            if (rsp.event_reconn_reg_server_mq_handle_timeout) {
                rsp.event_reconn_reg_server_mq_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_reconn_reg_server_mq_cb(uuid : number){
        var rsp = this.map_reconn_reg_server_mq.get(uuid);
        this.map_reconn_reg_server_mq.delete(uuid);
        return rsp;
    }

    public heartbeat_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_af04a217_eafb_393c_9e34_0303485bef77:any[] = [];
        var rsp = this.try_get_and_del_heartbeat_cb(uuid);
        if (rsp && rsp.event_heartbeat_handle_cb) {
            rsp.event_heartbeat_handle_cb.apply(null, _argv_af04a217_eafb_393c_9e34_0303485bef77);
        }
    }

    public heartbeat_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_af04a217_eafb_393c_9e34_0303485bef77:any[] = [];
        var rsp = this.try_get_and_del_heartbeat_cb(uuid);
        if (rsp && rsp.event_heartbeat_handle_err) {
            rsp.event_heartbeat_handle_err.apply(null, _argv_af04a217_eafb_393c_9e34_0303485bef77);
        }
    }

    public heartbeat_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_heartbeat_cb(cb_uuid);
        if (rsp){
            if (rsp.event_heartbeat_handle_timeout) {
                rsp.event_heartbeat_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_heartbeat_cb(uuid : number){
        var rsp = this.map_heartbeat.get(uuid);
        this.map_heartbeat.delete(uuid);
        return rsp;
    }

}

export let rsp_cb_center_handle : center_rsp_cb | null = null;
export class center_caller extends abelkhan.Icaller {
    private uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066 : number = Math.round(Math.random() * 1000);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("center", _ch);
        if (rsp_cb_center_handle == null){
            rsp_cb_center_handle = new center_rsp_cb(modules);
        }
    }

    public reg_server_mq(type:string, hub_type:string, svr_name:string){
        let uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf = Math.round(this.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++);

        let _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2:any[] = [uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf];
        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.push(type);
        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.push(hub_type);
        _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.push(svr_name);
        this.call_module_method("center_reg_server_mq", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);

        let cb_reg_server_mq_obj = new center_reg_server_mq_cb(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf, rsp_cb_center_handle);
        if (rsp_cb_center_handle){
            rsp_cb_center_handle.map_reg_server_mq.set(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf, cb_reg_server_mq_obj);
        }
        return cb_reg_server_mq_obj;
    }

    public reconn_reg_server_mq(type:string, hub_type:string, svr_name:string){
        let uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21 = Math.round(this.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++);

        let _argv_a018be20_2048_315d_9832_8120b194980f:any[] = [uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21];
        _argv_a018be20_2048_315d_9832_8120b194980f.push(type);
        _argv_a018be20_2048_315d_9832_8120b194980f.push(hub_type);
        _argv_a018be20_2048_315d_9832_8120b194980f.push(svr_name);
        this.call_module_method("center_reconn_reg_server_mq", _argv_a018be20_2048_315d_9832_8120b194980f);

        let cb_reconn_reg_server_mq_obj = new center_reconn_reg_server_mq_cb(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21, rsp_cb_center_handle);
        if (rsp_cb_center_handle){
            rsp_cb_center_handle.map_reconn_reg_server_mq.set(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21, cb_reconn_reg_server_mq_obj);
        }
        return cb_reconn_reg_server_mq_obj;
    }

    public heartbeat(tick:number){
        let uuid_9654538a_9916_57dc_8ea5_806086d7a378 = Math.round(this.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++);

        let _argv_af04a217_eafb_393c_9e34_0303485bef77:any[] = [uuid_9654538a_9916_57dc_8ea5_806086d7a378];
        _argv_af04a217_eafb_393c_9e34_0303485bef77.push(tick);
        this.call_module_method("center_heartbeat", _argv_af04a217_eafb_393c_9e34_0303485bef77);

        let cb_heartbeat_obj = new center_heartbeat_cb(uuid_9654538a_9916_57dc_8ea5_806086d7a378, rsp_cb_center_handle);
        if (rsp_cb_center_handle){
            rsp_cb_center_handle.map_heartbeat.set(uuid_9654538a_9916_57dc_8ea5_806086d7a378, cb_heartbeat_obj);
        }
        return cb_heartbeat_obj;
    }

    public closed(){
        let _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee:any[] = [];
        this.call_module_method("center_closed", _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee);
    }

}
/*this cb code is codegen by abelkhan for ts*/
export class center_call_server_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("center_call_server_rsp_cb");
    }
}

export let rsp_cb_center_call_server_handle : center_call_server_rsp_cb | null = null;
export class center_call_server_caller extends abelkhan.Icaller {
    private uuid_8c11e5bb_e9ff_3a0b_a436_65a9922a8da5 : number = Math.round(Math.random() * 1000);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("center_call_server", _ch);
        if (rsp_cb_center_call_server_handle == null){
            rsp_cb_center_call_server_handle = new center_call_server_rsp_cb(modules);
        }
    }

    public close_server(){
        let _argv_8394af17_8a06_3068_977d_477a1276f56e:any[] = [];
        this.call_module_method("center_call_server_close_server", _argv_8394af17_8a06_3068_977d_477a1276f56e);
    }

    public console_close_server(svr_type:string, svr_name:string){
        let _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc:any[] = [];
        _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc.push(svr_type);
        _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc.push(svr_name);
        this.call_module_method("center_call_server_console_close_server", _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc);
    }

    public svr_be_closed(svr_type:string, svr_name:string){
        let _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac:any[] = [];
        _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.push(svr_type);
        _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.push(svr_name);
        this.call_module_method("center_call_server_svr_be_closed", _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac);
    }

    public take_over_svr(svr_name:string){
        let _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4:any[] = [];
        _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4.push(svr_name);
        this.call_module_method("center_call_server_take_over_svr", _argv_8ea1cba0_190b_3582_a2d3_7349a0a04cf4);
    }

}
/*this cb code is codegen by abelkhan for ts*/
export class center_call_hub_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("center_call_hub_rsp_cb");
    }
}

export let rsp_cb_center_call_hub_handle : center_call_hub_rsp_cb | null = null;
export class center_call_hub_caller extends abelkhan.Icaller {
    private uuid_adbd1e34_0c90_3426_aefa_4d734c07a706 : number = Math.round(Math.random() * 1000);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("center_call_hub", _ch);
        if (rsp_cb_center_call_hub_handle == null){
            rsp_cb_center_call_hub_handle = new center_call_hub_rsp_cb(modules);
        }
    }

    public distribute_server_mq(svr_type:string, svr_name:string){
        let _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393:any[] = [];
        _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393.push(svr_type);
        _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393.push(svr_name);
        this.call_module_method("center_call_hub_distribute_server_mq", _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393);
    }

    public reload(argv:string){
        let _argv_ba37af53_beea_3d61_82e1_8d15e335971d:any[] = [];
        _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push(argv);
        this.call_module_method("center_call_hub_reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
    }

}
/*this cb code is codegen by abelkhan for ts*/
export class gm_center_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("gm_center_rsp_cb");
    }
}

export let rsp_cb_gm_center_handle : gm_center_rsp_cb | null = null;
export class gm_center_caller extends abelkhan.Icaller {
    private uuid_130fb971_5ae0_3446_b480_f9ee83dbeb28 : number = Math.round(Math.random() * 1000);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("gm_center", _ch);
        if (rsp_cb_gm_center_handle == null){
            rsp_cb_gm_center_handle = new gm_center_rsp_cb(modules);
        }
    }

    public confirm_gm(gm_name:string){
        let _argv_d097689c_b711_3837_8783_64916ae34cea:any[] = [];
        _argv_d097689c_b711_3837_8783_64916ae34cea.push(gm_name);
        this.call_module_method("gm_center_confirm_gm", _argv_d097689c_b711_3837_8783_64916ae34cea);
    }

    public close_clutter(gmname:string){
        let _argv_576c03c3_06da_36a2_b868_752da601cb54:any[] = [];
        _argv_576c03c3_06da_36a2_b868_752da601cb54.push(gmname);
        this.call_module_method("gm_center_close_clutter", _argv_576c03c3_06da_36a2_b868_752da601cb54);
    }

    public reload(gmname:string, argv:string){
        let _argv_ba37af53_beea_3d61_82e1_8d15e335971d:any[] = [];
        _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push(gmname);
        _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push(argv);
        this.call_module_method("gm_center_reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
    }

}
/*this module code is codegen by abelkhan codegen for typescript*/
export class center_reg_server_mq_rsp extends abelkhan.Icaller {
    private uuid_7254d987_ac9c_3d73_831c_f43efb3268a9 : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("center_rsp_cb", _ch);
        this.uuid_7254d987_ac9c_3d73_831c_f43efb3268a9 = _uuid;
    }

    public rsp(){
        let _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2:any[] = [this.uuid_7254d987_ac9c_3d73_831c_f43efb3268a9];
        this.call_module_method("center_rsp_cb_reg_server_mq_rsp", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);
    }

    public err(){
        let _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2:any[] = [this.uuid_7254d987_ac9c_3d73_831c_f43efb3268a9];
        this.call_module_method("center_rsp_cb_reg_server_mq_err", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);
    }

}

export class center_reconn_reg_server_mq_rsp extends abelkhan.Icaller {
    private uuid_4d058274_a122_382e_8084_b9067ed713c5 : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("center_rsp_cb", _ch);
        this.uuid_4d058274_a122_382e_8084_b9067ed713c5 = _uuid;
    }

    public rsp(){
        let _argv_a018be20_2048_315d_9832_8120b194980f:any[] = [this.uuid_4d058274_a122_382e_8084_b9067ed713c5];
        this.call_module_method("center_rsp_cb_reconn_reg_server_mq_rsp", _argv_a018be20_2048_315d_9832_8120b194980f);
    }

    public err(){
        let _argv_a018be20_2048_315d_9832_8120b194980f:any[] = [this.uuid_4d058274_a122_382e_8084_b9067ed713c5];
        this.call_module_method("center_rsp_cb_reconn_reg_server_mq_err", _argv_a018be20_2048_315d_9832_8120b194980f);
    }

}

export class center_heartbeat_rsp extends abelkhan.Icaller {
    private uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("center_rsp_cb", _ch);
        this.uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f = _uuid;
    }

    public rsp(){
        let _argv_af04a217_eafb_393c_9e34_0303485bef77:any[] = [this.uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f];
        this.call_module_method("center_rsp_cb_heartbeat_rsp", _argv_af04a217_eafb_393c_9e34_0303485bef77);
    }

    public err(){
        let _argv_af04a217_eafb_393c_9e34_0303485bef77:any[] = [this.uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f];
        this.call_module_method("center_rsp_cb_heartbeat_err", _argv_af04a217_eafb_393c_9e34_0303485bef77);
    }

}

export class center_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("center");
        this.modules = modules;
        this.modules.reg_method("center_reg_server_mq", [this, this.reg_server_mq.bind(this)]);
        this.modules.reg_method("center_reconn_reg_server_mq", [this, this.reconn_reg_server_mq.bind(this)]);
        this.modules.reg_method("center_heartbeat", [this, this.heartbeat.bind(this)]);
        this.modules.reg_method("center_closed", [this, this.closed.bind(this)]);

        this.cb_reg_server_mq = null;
        this.cb_reconn_reg_server_mq = null;
        this.cb_heartbeat = null;
        this.cb_closed = null;
    }

    public cb_reg_server_mq : (type:string, hub_type:string, svr_name:string)=>void | null;
    reg_server_mq(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        this.rsp = new center_reg_server_mq_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reg_server_mq){
            this.cb_reg_server_mq.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_reconn_reg_server_mq : (type:string, hub_type:string, svr_name:string)=>void | null;
    reconn_reg_server_mq(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        this.rsp = new center_reconn_reg_server_mq_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reconn_reg_server_mq){
            this.cb_reconn_reg_server_mq.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_heartbeat : (tick:number)=>void | null;
    heartbeat(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        this.rsp = new center_heartbeat_rsp(this.current_ch, _cb_uuid);
        if (this.cb_heartbeat){
            this.cb_heartbeat.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_closed : ()=>void | null;
    closed(inArray:any[]){
        let _argv_:any[] = [];
        if (this.cb_closed){
            this.cb_closed.apply(null, _argv_);
        }
    }

}
export class center_call_server_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("center_call_server");
        this.modules = modules;
        this.modules.reg_method("center_call_server_close_server", [this, this.close_server.bind(this)]);
        this.modules.reg_method("center_call_server_console_close_server", [this, this.console_close_server.bind(this)]);
        this.modules.reg_method("center_call_server_svr_be_closed", [this, this.svr_be_closed.bind(this)]);
        this.modules.reg_method("center_call_server_take_over_svr", [this, this.take_over_svr.bind(this)]);

        this.cb_close_server = null;
        this.cb_console_close_server = null;
        this.cb_svr_be_closed = null;
        this.cb_take_over_svr = null;
    }

    public cb_close_server : ()=>void | null;
    close_server(inArray:any[]){
        let _argv_:any[] = [];
        if (this.cb_close_server){
            this.cb_close_server.apply(null, _argv_);
        }
    }

    public cb_console_close_server : (svr_type:string, svr_name:string)=>void | null;
    console_close_server(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_console_close_server){
            this.cb_console_close_server.apply(null, _argv_);
        }
    }

    public cb_svr_be_closed : (svr_type:string, svr_name:string)=>void | null;
    svr_be_closed(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_svr_be_closed){
            this.cb_svr_be_closed.apply(null, _argv_);
        }
    }

    public cb_take_over_svr : (svr_name:string)=>void | null;
    take_over_svr(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_take_over_svr){
            this.cb_take_over_svr.apply(null, _argv_);
        }
    }

}
export class center_call_hub_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("center_call_hub");
        this.modules = modules;
        this.modules.reg_method("center_call_hub_distribute_server_mq", [this, this.distribute_server_mq.bind(this)]);
        this.modules.reg_method("center_call_hub_reload", [this, this.reload.bind(this)]);

        this.cb_distribute_server_mq = null;
        this.cb_reload = null;
    }

    public cb_distribute_server_mq : (svr_type:string, svr_name:string)=>void | null;
    distribute_server_mq(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_distribute_server_mq){
            this.cb_distribute_server_mq.apply(null, _argv_);
        }
    }

    public cb_reload : (argv:string)=>void | null;
    reload(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_reload){
            this.cb_reload.apply(null, _argv_);
        }
    }

}
export class gm_center_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("gm_center");
        this.modules = modules;
        this.modules.reg_method("gm_center_confirm_gm", [this, this.confirm_gm.bind(this)]);
        this.modules.reg_method("gm_center_close_clutter", [this, this.close_clutter.bind(this)]);
        this.modules.reg_method("gm_center_reload", [this, this.reload.bind(this)]);

        this.cb_confirm_gm = null;
        this.cb_close_clutter = null;
        this.cb_reload = null;
    }

    public cb_confirm_gm : (gm_name:string)=>void | null;
    confirm_gm(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_confirm_gm){
            this.cb_confirm_gm.apply(null, _argv_);
        }
    }

    public cb_close_clutter : (gmname:string)=>void | null;
    close_clutter(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_close_clutter){
            this.cb_close_clutter.apply(null, _argv_);
        }
    }

    public cb_reload : (gmname:string, argv:string)=>void | null;
    reload(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_reload){
            this.cb_reload.apply(null, _argv_);
        }
    }

}
