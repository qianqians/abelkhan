import * as abelkhan from "./abelkhan";
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
export class center_reg_server_cb{
    private cb_uuid : number;
    private module_rsp_cb : center_rsp_cb;

    public event_reg_server_handle_cb : ()=>void | null;
    public event_reg_server_handle_err : ()=>void | null;
    public event_reg_server_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : center_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_reg_server_handle_cb = null;
        this.event_reg_server_handle_err = null;
        this.event_reg_server_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_reg_server_handle_cb = _cb;
        this.event_reg_server_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.reg_server_timeout(this.cb_uuid); }, tick);
        this.event_reg_server_handle_timeout = timeout_cb;
    }

}

export class center_reconn_reg_server_cb{
    private cb_uuid : number;
    private module_rsp_cb : center_rsp_cb;

    public event_reconn_reg_server_handle_cb : ()=>void | null;
    public event_reconn_reg_server_handle_err : ()=>void | null;
    public event_reconn_reg_server_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : center_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_reconn_reg_server_handle_cb = null;
        this.event_reconn_reg_server_handle_err = null;
        this.event_reconn_reg_server_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_reconn_reg_server_handle_cb = _cb;
        this.event_reconn_reg_server_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.reconn_reg_server_timeout(this.cb_uuid); }, tick);
        this.event_reconn_reg_server_handle_timeout = timeout_cb;
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
    public map_reg_server:Map<number, center_reg_server_cb>;
    public map_reconn_reg_server:Map<number, center_reconn_reg_server_cb>;
    public map_heartbeat:Map<number, center_heartbeat_cb>;
    constructor(modules:abelkhan.modulemng){
        super("center_rsp_cb");
        this.map_reg_server = new Map<number, center_reg_server_cb>();
        modules.reg_method("reg_server_rsp", [this, this.reg_server_rsp.bind(this)]);
        modules.reg_method("reg_server_err", [this, this.reg_server_err.bind(this)]);
        this.map_reconn_reg_server = new Map<number, center_reconn_reg_server_cb>();
        modules.reg_method("reconn_reg_server_rsp", [this, this.reconn_reg_server_rsp.bind(this)]);
        modules.reg_method("reconn_reg_server_err", [this, this.reconn_reg_server_err.bind(this)]);
        this.map_heartbeat = new Map<number, center_heartbeat_cb>();
        modules.reg_method("heartbeat_rsp", [this, this.heartbeat_rsp.bind(this)]);
        modules.reg_method("heartbeat_err", [this, this.heartbeat_err.bind(this)]);
    }
    public reg_server_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_86ab8166_c1a7_3809_8c9b_df444f746076:any[] = [];
        var rsp = this.try_get_and_del_reg_server_cb(uuid);
        if (rsp && rsp.event_reg_server_handle_cb) {
            rsp.event_reg_server_handle_cb.apply(null, _argv_86ab8166_c1a7_3809_8c9b_df444f746076);
        }
    }

    public reg_server_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_86ab8166_c1a7_3809_8c9b_df444f746076:any[] = [];
        var rsp = this.try_get_and_del_reg_server_cb(uuid);
        if (rsp && rsp.event_reg_server_handle_err) {
            rsp.event_reg_server_handle_err.apply(null, _argv_86ab8166_c1a7_3809_8c9b_df444f746076);
        }
    }

    public reg_server_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_reg_server_cb(cb_uuid);
        if (rsp){
            if (rsp.event_reg_server_handle_timeout) {
                rsp.event_reg_server_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_reg_server_cb(uuid : number){
        var rsp = this.map_reg_server.get(uuid);
        this.map_reg_server.delete(uuid);
        return rsp;
    }

    public reconn_reg_server_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_a181e793_c43f_3b7f_b19e_178395e5927d:any[] = [];
        var rsp = this.try_get_and_del_reconn_reg_server_cb(uuid);
        if (rsp && rsp.event_reconn_reg_server_handle_cb) {
            rsp.event_reconn_reg_server_handle_cb.apply(null, _argv_a181e793_c43f_3b7f_b19e_178395e5927d);
        }
    }

    public reconn_reg_server_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_a181e793_c43f_3b7f_b19e_178395e5927d:any[] = [];
        var rsp = this.try_get_and_del_reconn_reg_server_cb(uuid);
        if (rsp && rsp.event_reconn_reg_server_handle_err) {
            rsp.event_reconn_reg_server_handle_err.apply(null, _argv_a181e793_c43f_3b7f_b19e_178395e5927d);
        }
    }

    public reconn_reg_server_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_reconn_reg_server_cb(cb_uuid);
        if (rsp){
            if (rsp.event_reconn_reg_server_handle_timeout) {
                rsp.event_reconn_reg_server_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_reconn_reg_server_cb(uuid : number){
        var rsp = this.map_reconn_reg_server.get(uuid);
        this.map_reconn_reg_server.delete(uuid);
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

    public reg_server(type:string, svr_name:string, host:string, port:number){
        let uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255 = Math.round(this.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++);

        let _argv_86ab8166_c1a7_3809_8c9b_df444f746076:any[] = [uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255];
        _argv_86ab8166_c1a7_3809_8c9b_df444f746076.push(type);
        _argv_86ab8166_c1a7_3809_8c9b_df444f746076.push(svr_name);
        _argv_86ab8166_c1a7_3809_8c9b_df444f746076.push(host);
        _argv_86ab8166_c1a7_3809_8c9b_df444f746076.push(port);
        this.call_module_method("center_reg_server", _argv_86ab8166_c1a7_3809_8c9b_df444f746076);

        let cb_reg_server_obj = new center_reg_server_cb(uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255, rsp_cb_center_handle);
        if (rsp_cb_center_handle){
            rsp_cb_center_handle.map_reg_server.set(uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255, cb_reg_server_obj);
        }
        return cb_reg_server_obj;
    }

    public reconn_reg_server(type:string, svr_name:string, host:string, port:number){
        let uuid_9564c83b_b4e0_57f7_87dd_02fb4c7a2d0d = Math.round(this.uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++);

        let _argv_a181e793_c43f_3b7f_b19e_178395e5927d:any[] = [uuid_9564c83b_b4e0_57f7_87dd_02fb4c7a2d0d];
        _argv_a181e793_c43f_3b7f_b19e_178395e5927d.push(type);
        _argv_a181e793_c43f_3b7f_b19e_178395e5927d.push(svr_name);
        _argv_a181e793_c43f_3b7f_b19e_178395e5927d.push(host);
        _argv_a181e793_c43f_3b7f_b19e_178395e5927d.push(port);
        this.call_module_method("center_reconn_reg_server", _argv_a181e793_c43f_3b7f_b19e_178395e5927d);

        let cb_reconn_reg_server_obj = new center_reconn_reg_server_cb(uuid_9564c83b_b4e0_57f7_87dd_02fb4c7a2d0d, rsp_cb_center_handle);
        if (rsp_cb_center_handle){
            rsp_cb_center_handle.map_reconn_reg_server.set(uuid_9564c83b_b4e0_57f7_87dd_02fb4c7a2d0d, cb_reconn_reg_server_obj);
        }
        return cb_reconn_reg_server_obj;
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

    public distribute_server_address(svr_type:string, svr_name:string, host:string, port:number){
        let _argv_b71bf35c_d65b_3682_98d1_b934f5276558:any[] = [];
        _argv_b71bf35c_d65b_3682_98d1_b934f5276558.push(svr_type);
        _argv_b71bf35c_d65b_3682_98d1_b934f5276558.push(svr_name);
        _argv_b71bf35c_d65b_3682_98d1_b934f5276558.push(host);
        _argv_b71bf35c_d65b_3682_98d1_b934f5276558.push(port);
        this.call_module_method("center_call_hub_distribute_server_address", _argv_b71bf35c_d65b_3682_98d1_b934f5276558);
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
export class center_reg_server_rsp extends abelkhan.Icaller {
    private uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("center_rsp_cb", _ch);
        this.uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda = _uuid;
    }

    public rsp(){
        let _argv_86ab8166_c1a7_3809_8c9b_df444f746076:any[] = [this.uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda];
        this.call_module_method("center_reg_server_rsp", _argv_86ab8166_c1a7_3809_8c9b_df444f746076);
    }

    public err(){
        let _argv_86ab8166_c1a7_3809_8c9b_df444f746076:any[] = [this.uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda];
        this.call_module_method("center_reg_server_err", _argv_86ab8166_c1a7_3809_8c9b_df444f746076);
    }

}

export class center_reconn_reg_server_rsp extends abelkhan.Icaller {
    private uuid_39461677_ebd9_335f_830b_8d355adba2f0 : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("center_rsp_cb", _ch);
        this.uuid_39461677_ebd9_335f_830b_8d355adba2f0 = _uuid;
    }

    public rsp(){
        let _argv_a181e793_c43f_3b7f_b19e_178395e5927d:any[] = [this.uuid_39461677_ebd9_335f_830b_8d355adba2f0];
        this.call_module_method("center_reconn_reg_server_rsp", _argv_a181e793_c43f_3b7f_b19e_178395e5927d);
    }

    public err(){
        let _argv_a181e793_c43f_3b7f_b19e_178395e5927d:any[] = [this.uuid_39461677_ebd9_335f_830b_8d355adba2f0];
        this.call_module_method("center_reconn_reg_server_err", _argv_a181e793_c43f_3b7f_b19e_178395e5927d);
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
        this.call_module_method("center_heartbeat_rsp", _argv_af04a217_eafb_393c_9e34_0303485bef77);
    }

    public err(){
        let _argv_af04a217_eafb_393c_9e34_0303485bef77:any[] = [this.uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f];
        this.call_module_method("center_heartbeat_err", _argv_af04a217_eafb_393c_9e34_0303485bef77);
    }

}

export class center_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("center");
        this.modules = modules;
        this.modules.reg_method("reg_server", [this, this.reg_server.bind(this)]);
        this.modules.reg_method("reconn_reg_server", [this, this.reconn_reg_server.bind(this)]);
        this.modules.reg_method("heartbeat", [this, this.heartbeat.bind(this)]);
        this.modules.reg_method("closed", [this, this.closed.bind(this)]);

        this.cb_reg_server = null;
        this.cb_reconn_reg_server = null;
        this.cb_heartbeat = null;
        this.cb_closed = null;
    }

    public cb_reg_server : (type:string, svr_name:string, host:string, port:number)=>void | null;
    reg_server(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        _argv_.push(inArray[4]);
        this.rsp = new center_reg_server_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reg_server){
            this.cb_reg_server.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_reconn_reg_server : (type:string, svr_name:string, host:string, port:number)=>void | null;
    reconn_reg_server(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        _argv_.push(inArray[4]);
        this.rsp = new center_reconn_reg_server_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reconn_reg_server){
            this.cb_reconn_reg_server.apply(null, _argv_);
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
        this.modules.reg_method("close_server", [this, this.close_server.bind(this)]);
        this.modules.reg_method("console_close_server", [this, this.console_close_server.bind(this)]);
        this.modules.reg_method("svr_be_closed", [this, this.svr_be_closed.bind(this)]);

        this.cb_close_server = null;
        this.cb_console_close_server = null;
        this.cb_svr_be_closed = null;
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

}
export class center_call_hub_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("center_call_hub");
        this.modules = modules;
        this.modules.reg_method("distribute_server_address", [this, this.distribute_server_address.bind(this)]);
        this.modules.reg_method("reload", [this, this.reload.bind(this)]);

        this.cb_distribute_server_address = null;
        this.cb_reload = null;
    }

    public cb_distribute_server_address : (svr_type:string, svr_name:string, host:string, port:number)=>void | null;
    distribute_server_address(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        if (this.cb_distribute_server_address){
            this.cb_distribute_server_address.apply(null, _argv_);
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
        this.modules.reg_method("confirm_gm", [this, this.confirm_gm.bind(this)]);
        this.modules.reg_method("close_clutter", [this, this.close_clutter.bind(this)]);
        this.modules.reg_method("reload", [this, this.reload.bind(this)]);

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
