import abelkhan = require("abelkhan");
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
/*this cb code is codegen by abelkhan for ts*/
export class dbproxy_call_hub_rsp_cb extends abelkhan.Imodule {
    constructor(modules:abelkhan.modulemng){
        super("dbproxy_call_hub_rsp_cb");
        modules.reg_module(this);

    }
}

export let rsp_cb_dbproxy_call_hub_handle : dbproxy_call_hub_rsp_cb | null = null;
export class dbproxy_call_hub_caller extends abelkhan.Icaller {
    private uuid : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("dbproxy_call_hub", _ch);
        if (rsp_cb_dbproxy_call_hub_handle == null){
            rsp_cb_dbproxy_call_hub_handle = new dbproxy_call_hub_rsp_cb(modules);
        }
    }

    public ack_get_object_info(callbackid:string, object_info:Uint8Array){
        let _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7:any[] = [];
        _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.push(callbackid);
        _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.push(object_info);
        this.call_module_method("ack_get_object_info", _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7);
    }

    public ack_get_object_info_end(callbackid:string){
        let _argv_e4756ccf_94e2_3b4f_958a_701f7076e607:any[] = [];
        _argv_e4756ccf_94e2_3b4f_958a_701f7076e607.push(callbackid);
        this.call_module_method("ack_get_object_info_end", _argv_e4756ccf_94e2_3b4f_958a_701f7076e607);
    }

}
export class hub_call_dbproxy_reg_hub_cb{
    private cb_uuid : number;
    private module_rsp_cb : hub_call_dbproxy_rsp_cb;

    public event_reg_hub_handle_cb : ()=>void | null;
    public event_reg_hub_handle_err : ()=>void | null;
    public event_reg_hub_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : hub_call_dbproxy_rsp_cb){
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

export class hub_call_dbproxy_create_persisted_object_cb{
    private cb_uuid : number;
    private module_rsp_cb : hub_call_dbproxy_rsp_cb;

    public event_create_persisted_object_handle_cb : ()=>void | null;
    public event_create_persisted_object_handle_err : ()=>void | null;
    public event_create_persisted_object_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : hub_call_dbproxy_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_create_persisted_object_handle_cb = null;
        this.event_create_persisted_object_handle_err = null;
        this.event_create_persisted_object_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_create_persisted_object_handle_cb = _cb;
        this.event_create_persisted_object_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.create_persisted_object_timeout(this.cb_uuid); }, tick);
        this.event_create_persisted_object_handle_timeout = timeout_cb;
    }

}

export class hub_call_dbproxy_updata_persisted_object_cb{
    private cb_uuid : number;
    private module_rsp_cb : hub_call_dbproxy_rsp_cb;

    public event_updata_persisted_object_handle_cb : ()=>void | null;
    public event_updata_persisted_object_handle_err : ()=>void | null;
    public event_updata_persisted_object_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : hub_call_dbproxy_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_updata_persisted_object_handle_cb = null;
        this.event_updata_persisted_object_handle_err = null;
        this.event_updata_persisted_object_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_updata_persisted_object_handle_cb = _cb;
        this.event_updata_persisted_object_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.updata_persisted_object_timeout(this.cb_uuid); }, tick);
        this.event_updata_persisted_object_handle_timeout = timeout_cb;
    }

}

export class hub_call_dbproxy_remove_object_cb{
    private cb_uuid : number;
    private module_rsp_cb : hub_call_dbproxy_rsp_cb;

    public event_remove_object_handle_cb : ()=>void | null;
    public event_remove_object_handle_err : ()=>void | null;
    public event_remove_object_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : hub_call_dbproxy_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_remove_object_handle_cb = null;
        this.event_remove_object_handle_err = null;
        this.event_remove_object_handle_timeout = null;
    }

    callBack(_cb:()=>void, _err:()=>void)
    {
        this.event_remove_object_handle_cb = _cb;
        this.event_remove_object_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.remove_object_timeout(this.cb_uuid); }, tick);
        this.event_remove_object_handle_timeout = timeout_cb;
    }

}

export class hub_call_dbproxy_get_object_count_cb{
    private cb_uuid : number;
    private module_rsp_cb : hub_call_dbproxy_rsp_cb;

    public event_get_object_count_handle_cb : (count:number)=>void | null;
    public event_get_object_count_handle_err : ()=>void | null;
    public event_get_object_count_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : hub_call_dbproxy_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_object_count_handle_cb = null;
        this.event_get_object_count_handle_err = null;
        this.event_get_object_count_handle_timeout = null;
    }

    callBack(_cb:(count:number)=>void, _err:()=>void)
    {
        this.event_get_object_count_handle_cb = _cb;
        this.event_get_object_count_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.get_object_count_timeout(this.cb_uuid); }, tick);
        this.event_get_object_count_handle_timeout = timeout_cb;
    }

}

/*this cb code is codegen by abelkhan for ts*/
export class hub_call_dbproxy_rsp_cb extends abelkhan.Imodule {
    public map_reg_hub:Map<number, hub_call_dbproxy_reg_hub_cb>;
    public map_create_persisted_object:Map<number, hub_call_dbproxy_create_persisted_object_cb>;
    public map_updata_persisted_object:Map<number, hub_call_dbproxy_updata_persisted_object_cb>;
    public map_remove_object:Map<number, hub_call_dbproxy_remove_object_cb>;
    public map_get_object_count:Map<number, hub_call_dbproxy_get_object_count_cb>;
    constructor(modules:abelkhan.modulemng){
        super("hub_call_dbproxy_rsp_cb");
        modules.reg_module(this);

        this.map_reg_hub = new Map<number, hub_call_dbproxy_reg_hub_cb>();
        this.reg_method("reg_hub_rsp", this.reg_hub_rsp.bind(this));
        this.reg_method("reg_hub_err", this.reg_hub_err.bind(this));
        this.map_create_persisted_object = new Map<number, hub_call_dbproxy_create_persisted_object_cb>();
        this.reg_method("create_persisted_object_rsp", this.create_persisted_object_rsp.bind(this));
        this.reg_method("create_persisted_object_err", this.create_persisted_object_err.bind(this));
        this.map_updata_persisted_object = new Map<number, hub_call_dbproxy_updata_persisted_object_cb>();
        this.reg_method("updata_persisted_object_rsp", this.updata_persisted_object_rsp.bind(this));
        this.reg_method("updata_persisted_object_err", this.updata_persisted_object_err.bind(this));
        this.map_remove_object = new Map<number, hub_call_dbproxy_remove_object_cb>();
        this.reg_method("remove_object_rsp", this.remove_object_rsp.bind(this));
        this.reg_method("remove_object_err", this.remove_object_err.bind(this));
        this.map_get_object_count = new Map<number, hub_call_dbproxy_get_object_count_cb>();
        this.reg_method("get_object_count_rsp", this.get_object_count_rsp.bind(this));
        this.reg_method("get_object_count_err", this.get_object_count_err.bind(this));
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

    public create_persisted_object_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607:any[] = [];
        var rsp = this.try_get_and_del_create_persisted_object_cb(uuid);
        if (rsp && rsp.event_create_persisted_object_handle_cb) {
            rsp.event_create_persisted_object_handle_cb.apply(null, _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
        }
    }

    public create_persisted_object_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607:any[] = [];
        var rsp = this.try_get_and_del_create_persisted_object_cb(uuid);
        if (rsp && rsp.event_create_persisted_object_handle_err) {
            rsp.event_create_persisted_object_handle_err.apply(null, _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
        }
    }

    public create_persisted_object_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_create_persisted_object_cb(cb_uuid);
        if (rsp){
            if (rsp.event_create_persisted_object_handle_timeout) {
                rsp.event_create_persisted_object_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_create_persisted_object_cb(uuid : number){
        var rsp = this.map_create_persisted_object.get(uuid);
        this.map_create_persisted_object.delete(uuid);
        return rsp;
    }

    public updata_persisted_object_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425:any[] = [];
        var rsp = this.try_get_and_del_updata_persisted_object_cb(uuid);
        if (rsp && rsp.event_updata_persisted_object_handle_cb) {
            rsp.event_updata_persisted_object_handle_cb.apply(null, _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
        }
    }

    public updata_persisted_object_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425:any[] = [];
        var rsp = this.try_get_and_del_updata_persisted_object_cb(uuid);
        if (rsp && rsp.event_updata_persisted_object_handle_err) {
            rsp.event_updata_persisted_object_handle_err.apply(null, _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
        }
    }

    public updata_persisted_object_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_updata_persisted_object_cb(cb_uuid);
        if (rsp){
            if (rsp.event_updata_persisted_object_handle_timeout) {
                rsp.event_updata_persisted_object_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_updata_persisted_object_cb(uuid : number){
        var rsp = this.map_updata_persisted_object.get(uuid);
        this.map_updata_persisted_object.delete(uuid);
        return rsp;
    }

    public remove_object_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da:any[] = [];
        var rsp = this.try_get_and_del_remove_object_cb(uuid);
        if (rsp && rsp.event_remove_object_handle_cb) {
            rsp.event_remove_object_handle_cb.apply(null, _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
        }
    }

    public remove_object_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da:any[] = [];
        var rsp = this.try_get_and_del_remove_object_cb(uuid);
        if (rsp && rsp.event_remove_object_handle_err) {
            rsp.event_remove_object_handle_err.apply(null, _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
        }
    }

    public remove_object_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_remove_object_cb(cb_uuid);
        if (rsp){
            if (rsp.event_remove_object_handle_timeout) {
                rsp.event_remove_object_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_remove_object_cb(uuid : number){
        var rsp = this.map_remove_object.get(uuid);
        this.map_remove_object.delete(uuid);
        return rsp;
    }

    public get_object_count_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_2632cded_162c_3a9b_86ee_462b614cbeea:any[] = [];
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.push(inArray[1]);
        var rsp = this.try_get_and_del_get_object_count_cb(uuid);
        if (rsp && rsp.event_get_object_count_handle_cb) {
            rsp.event_get_object_count_handle_cb.apply(null, _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
        }
    }

    public get_object_count_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_2632cded_162c_3a9b_86ee_462b614cbeea:any[] = [];
        var rsp = this.try_get_and_del_get_object_count_cb(uuid);
        if (rsp && rsp.event_get_object_count_handle_err) {
            rsp.event_get_object_count_handle_err.apply(null, _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
        }
    }

    public get_object_count_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_get_object_count_cb(cb_uuid);
        if (rsp){
            if (rsp.event_get_object_count_handle_timeout) {
                rsp.event_get_object_count_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_get_object_count_cb(uuid : number){
        var rsp = this.map_get_object_count.get(uuid);
        this.map_get_object_count.delete(uuid);
        return rsp;
    }

}

export let rsp_cb_hub_call_dbproxy_handle : hub_call_dbproxy_rsp_cb | null = null;
export class hub_call_dbproxy_caller extends abelkhan.Icaller {
    private uuid : number = Math.round(Math.random() * Number.MAX_VALUE);

    constructor(_ch:any, modules:abelkhan.modulemng){
        super("hub_call_dbproxy", _ch);
        if (rsp_cb_hub_call_dbproxy_handle == null){
            rsp_cb_hub_call_dbproxy_handle = new hub_call_dbproxy_rsp_cb(modules);
        }
    }

    public reg_hub(hub_uuid:string){
        let uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = this.uuid++;

        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7:any[] = [uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106];
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_uuid);
        this.call_module_method("reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

        let cb_reg_hub_obj = new hub_call_dbproxy_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_dbproxy_handle);
        if (rsp_cb_hub_call_dbproxy_handle){
            rsp_cb_hub_call_dbproxy_handle.map_reg_hub.set(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
        }
        return cb_reg_hub_obj;
    }

    public create_persisted_object(db:string, collection:string, object_info:Uint8Array){
        let uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb = this.uuid++;

        let _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607:any[] = [uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb];
        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.push(db);
        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.push(collection);
        _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.push(object_info);
        this.call_module_method("create_persisted_object", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);

        let cb_create_persisted_object_obj = new hub_call_dbproxy_create_persisted_object_cb(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb, rsp_cb_hub_call_dbproxy_handle);
        if (rsp_cb_hub_call_dbproxy_handle){
            rsp_cb_hub_call_dbproxy_handle.map_create_persisted_object.set(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb, cb_create_persisted_object_obj);
        }
        return cb_create_persisted_object_obj;
    }

    public updata_persisted_object(db:string, collection:string, query_json:Uint8Array, object_info:Uint8Array){
        let uuid_7864a402_2d75_5c02_b24b_50287a06732f = this.uuid++;

        let _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425:any[] = [uuid_7864a402_2d75_5c02_b24b_50287a06732f];
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.push(db);
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.push(collection);
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.push(query_json);
        _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.push(object_info);
        this.call_module_method("updata_persisted_object", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);

        let cb_updata_persisted_object_obj = new hub_call_dbproxy_updata_persisted_object_cb(uuid_7864a402_2d75_5c02_b24b_50287a06732f, rsp_cb_hub_call_dbproxy_handle);
        if (rsp_cb_hub_call_dbproxy_handle){
            rsp_cb_hub_call_dbproxy_handle.map_updata_persisted_object.set(uuid_7864a402_2d75_5c02_b24b_50287a06732f, cb_updata_persisted_object_obj);
        }
        return cb_updata_persisted_object_obj;
    }

    public remove_object(db:string, collection:string, query_json:Uint8Array){
        let uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f = this.uuid++;

        let _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da:any[] = [uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f];
        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.push(db);
        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.push(collection);
        _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.push(query_json);
        this.call_module_method("remove_object", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);

        let cb_remove_object_obj = new hub_call_dbproxy_remove_object_cb(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f, rsp_cb_hub_call_dbproxy_handle);
        if (rsp_cb_hub_call_dbproxy_handle){
            rsp_cb_hub_call_dbproxy_handle.map_remove_object.set(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f, cb_remove_object_obj);
        }
        return cb_remove_object_obj;
    }

    public get_object_info(db:string, collection:string, query_json:Uint8Array, callbackid:string){
        let _argv_1f17e6de_d423_391b_a599_7268e665a53f:any[] = [];
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.push(db);
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.push(collection);
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.push(query_json);
        _argv_1f17e6de_d423_391b_a599_7268e665a53f.push(callbackid);
        this.call_module_method("get_object_info", _argv_1f17e6de_d423_391b_a599_7268e665a53f);
    }

    public get_object_count(db:string, collection:string, query_json:Uint8Array){
        let uuid_975425f5_8baf_5905_beeb_4454e78907f6 = this.uuid++;

        let _argv_2632cded_162c_3a9b_86ee_462b614cbeea:any[] = [uuid_975425f5_8baf_5905_beeb_4454e78907f6];
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.push(db);
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.push(collection);
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.push(query_json);
        this.call_module_method("get_object_count", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);

        let cb_get_object_count_obj = new hub_call_dbproxy_get_object_count_cb(uuid_975425f5_8baf_5905_beeb_4454e78907f6, rsp_cb_hub_call_dbproxy_handle);
        if (rsp_cb_hub_call_dbproxy_handle){
            rsp_cb_hub_call_dbproxy_handle.map_get_object_count.set(uuid_975425f5_8baf_5905_beeb_4454e78907f6, cb_get_object_count_obj);
        }
        return cb_get_object_count_obj;
    }

}
/*this module code is codegen by abelkhan codegen for typescript*/
export class dbproxy_call_hub_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("dbproxy_call_hub");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("ack_get_object_info", this.ack_get_object_info.bind(this));
        this.reg_method("ack_get_object_info_end", this.ack_get_object_info_end.bind(this));
        this.cb_ack_get_object_info = null;

        this.cb_ack_get_object_info_end = null;

    }

    public cb_ack_get_object_info : (callbackid:string, object_info:Uint8Array)=>void | null;
    ack_get_object_info(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_ack_get_object_info){
            this.cb_ack_get_object_info.apply(null, _argv_);
        }
    }

    public cb_ack_get_object_info_end : (callbackid:string)=>void | null;
    ack_get_object_info_end(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        if (this.cb_ack_get_object_info_end){
            this.cb_ack_get_object_info_end.apply(null, _argv_);
        }
    }

}
export class hub_call_dbproxy_reg_hub_rsp extends abelkhan.Icaller {
    private uuid : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("hub_call_dbproxy_rsp_cb", _ch);
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

export class hub_call_dbproxy_create_persisted_object_rsp extends abelkhan.Icaller {
    private uuid : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("hub_call_dbproxy_rsp_cb", _ch);
        this.uuid = _uuid;
    }

    public rsp(){
        let _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607:any[] = [this.uuid];
        this.call_module_method("create_persisted_object_rsp", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
    }

    public err(){
        let _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607:any[] = [this.uuid];
        this.call_module_method("create_persisted_object_err", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
    }

}

export class hub_call_dbproxy_updata_persisted_object_rsp extends abelkhan.Icaller {
    private uuid : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("hub_call_dbproxy_rsp_cb", _ch);
        this.uuid = _uuid;
    }

    public rsp(){
        let _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425:any[] = [this.uuid];
        this.call_module_method("updata_persisted_object_rsp", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
    }

    public err(){
        let _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425:any[] = [this.uuid];
        this.call_module_method("updata_persisted_object_err", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
    }

}

export class hub_call_dbproxy_remove_object_rsp extends abelkhan.Icaller {
    private uuid : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("hub_call_dbproxy_rsp_cb", _ch);
        this.uuid = _uuid;
    }

    public rsp(){
        let _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da:any[] = [this.uuid];
        this.call_module_method("remove_object_rsp", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
    }

    public err(){
        let _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da:any[] = [this.uuid];
        this.call_module_method("remove_object_err", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
    }

}

export class hub_call_dbproxy_get_object_count_rsp extends abelkhan.Icaller {
    private uuid : number;
    constructor(_ch:abelkhan.Ichannel, _uuid:number){
        super("hub_call_dbproxy_rsp_cb", _ch);
        this.uuid = _uuid;
    }

    public rsp(count:number){
        let _argv_2632cded_162c_3a9b_86ee_462b614cbeea:any[] = [this.uuid];
        _argv_2632cded_162c_3a9b_86ee_462b614cbeea.push(count);
        this.call_module_method("get_object_count_rsp", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
    }

    public err(){
        let _argv_2632cded_162c_3a9b_86ee_462b614cbeea:any[] = [this.uuid];
        this.call_module_method("get_object_count_err", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
    }

}

export class hub_call_dbproxy_module extends abelkhan.Imodule {
    private modules:abelkhan.modulemng;
    constructor(modules:abelkhan.modulemng){
        super("hub_call_dbproxy");
        this.modules = modules;
        this.modules.reg_module(this);

        this.reg_method("reg_hub", this.reg_hub.bind(this));
        this.reg_method("create_persisted_object", this.create_persisted_object.bind(this));
        this.reg_method("updata_persisted_object", this.updata_persisted_object.bind(this));
        this.reg_method("remove_object", this.remove_object.bind(this));
        this.reg_method("get_object_info", this.get_object_info.bind(this));
        this.reg_method("get_object_count", this.get_object_count.bind(this));
        this.cb_reg_hub = null;

        this.cb_create_persisted_object = null;

        this.cb_updata_persisted_object = null;

        this.cb_remove_object = null;

        this.cb_get_object_info = null;

        this.cb_get_object_count = null;

    }

    public cb_reg_hub : (hub_uuid:string)=>void | null;
    reg_hub(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        this.rsp = new hub_call_dbproxy_reg_hub_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reg_hub){
            this.cb_reg_hub.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_create_persisted_object : (db:string, collection:string, object_info:Uint8Array)=>void | null;
    create_persisted_object(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        this.rsp = new hub_call_dbproxy_create_persisted_object_rsp(this.current_ch, _cb_uuid);
        if (this.cb_create_persisted_object){
            this.cb_create_persisted_object.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_updata_persisted_object : (db:string, collection:string, query_json:Uint8Array, object_info:Uint8Array)=>void | null;
    updata_persisted_object(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        _argv_.push(inArray[4]);
        this.rsp = new hub_call_dbproxy_updata_persisted_object_rsp(this.current_ch, _cb_uuid);
        if (this.cb_updata_persisted_object){
            this.cb_updata_persisted_object.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_remove_object : (db:string, collection:string, query_json:Uint8Array)=>void | null;
    remove_object(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        this.rsp = new hub_call_dbproxy_remove_object_rsp(this.current_ch, _cb_uuid);
        if (this.cb_remove_object){
            this.cb_remove_object.apply(null, _argv_);
        }
        this.rsp = null;
    }

    public cb_get_object_info : (db:string, collection:string, query_json:Uint8Array, callbackid:string)=>void | null;
    get_object_info(inArray:any[]){
        let _argv_:any[] = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        if (this.cb_get_object_info){
            this.cb_get_object_info.apply(null, _argv_);
        }
    }

    public cb_get_object_count : (db:string, collection:string, query_json:Uint8Array)=>void | null;
    get_object_count(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        _argv_.push(inArray[3]);
        this.rsp = new hub_call_dbproxy_get_object_count_rsp(this.current_ch, _cb_uuid);
        if (this.cb_get_object_count){
            this.cb_get_object_count.apply(null, _argv_);
        }
        this.rsp = null;
    }

}
