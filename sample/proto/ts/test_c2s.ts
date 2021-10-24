import * as client_handle from "client_handle";
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
export class test_c2s_get_svr_host_cb{
    private cb_uuid : number;
    private module_rsp_cb : test_c2s_rsp_cb;

    public event_get_svr_host_handle_cb : (ip:string, port:number)=>void | null;
    public event_get_svr_host_handle_err : ()=>void | null;
    public event_get_svr_host_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : test_c2s_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_svr_host_handle_cb = null;
        this.event_get_svr_host_handle_err = null;
        this.event_get_svr_host_handle_timeout = null;
    }

    callBack(_cb:(ip:string, port:number)=>void, _err:()=>void)
    {
        this.event_get_svr_host_handle_cb = _cb;
        this.event_get_svr_host_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.get_svr_host_timeout(this.cb_uuid); }, tick);
        this.event_get_svr_host_handle_timeout = timeout_cb;
    }

}

export class test_c2s_get_websocket_svr_host_cb{
    private cb_uuid : number;
    private module_rsp_cb : test_c2s_rsp_cb;

    public event_get_websocket_svr_host_handle_cb : (ip:string, port:number)=>void | null;
    public event_get_websocket_svr_host_handle_err : ()=>void | null;
    public event_get_websocket_svr_host_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : test_c2s_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_websocket_svr_host_handle_cb = null;
        this.event_get_websocket_svr_host_handle_err = null;
        this.event_get_websocket_svr_host_handle_timeout = null;
    }

    callBack(_cb:(ip:string, port:number)=>void, _err:()=>void)
    {
        this.event_get_websocket_svr_host_handle_cb = _cb;
        this.event_get_websocket_svr_host_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.get_websocket_svr_host_timeout(this.cb_uuid); }, tick);
        this.event_get_websocket_svr_host_handle_timeout = timeout_cb;
    }

}

/*this cb code is codegen by abelkhan for ts*/
export class test_c2s_rsp_cb extends client_handle.imodule {
    public map_get_svr_host:Map<number, test_c2s_get_svr_host_cb>;
    public map_get_websocket_svr_host:Map<number, test_c2s_get_websocket_svr_host_cb>;
    constructor(modules:client_handle.modulemng){
        super();
        modules.add_module("test_c2s_rsp_cb", this);

        this.map_get_svr_host = new Map<number, test_c2s_get_svr_host_cb>();
        this.reg_cb("get_svr_host_rsp", this.get_svr_host_rsp.bind(this));
        this.reg_cb("get_svr_host_err", this.get_svr_host_err.bind(this));
        this.map_get_websocket_svr_host = new Map<number, test_c2s_get_websocket_svr_host_cb>();
        this.reg_cb("get_websocket_svr_host_rsp", this.get_websocket_svr_host_rsp.bind(this));
        this.reg_cb("get_websocket_svr_host_err", this.get_websocket_svr_host_err.bind(this));
    }
    public get_svr_host_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_abbb842f_52d0_34e7_9d8d_642d072db165:any[] = [];
        _argv_abbb842f_52d0_34e7_9d8d_642d072db165.push(inArray[1]);
        _argv_abbb842f_52d0_34e7_9d8d_642d072db165.push(inArray[2]);
        var rsp = this.try_get_and_del_get_svr_host_cb(uuid);
        if (rsp && rsp.event_get_svr_host_handle_cb) {
            rsp.event_get_svr_host_handle_cb.apply(null, _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        }
    }

    public get_svr_host_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_abbb842f_52d0_34e7_9d8d_642d072db165:any[] = [];
        var rsp = this.try_get_and_del_get_svr_host_cb(uuid);
        if (rsp && rsp.event_get_svr_host_handle_err) {
            rsp.event_get_svr_host_handle_err.apply(null, _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        }
    }

    public get_svr_host_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_get_svr_host_cb(cb_uuid);
        if (rsp){
            if (rsp.event_get_svr_host_handle_timeout) {
                rsp.event_get_svr_host_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_get_svr_host_cb(uuid : number){
        var rsp = this.map_get_svr_host.get(uuid);
        this.map_get_svr_host.delete(uuid);
        return rsp;
    }

    public get_websocket_svr_host_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f:any[] = [];
        _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.push(inArray[1]);
        _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.push(inArray[2]);
        var rsp = this.try_get_and_del_get_websocket_svr_host_cb(uuid);
        if (rsp && rsp.event_get_websocket_svr_host_handle_cb) {
            rsp.event_get_websocket_svr_host_handle_cb.apply(null, _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);
        }
    }

    public get_websocket_svr_host_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f:any[] = [];
        var rsp = this.try_get_and_del_get_websocket_svr_host_cb(uuid);
        if (rsp && rsp.event_get_websocket_svr_host_handle_err) {
            rsp.event_get_websocket_svr_host_handle_err.apply(null, _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);
        }
    }

    public get_websocket_svr_host_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_get_websocket_svr_host_cb(cb_uuid);
        if (rsp){
            if (rsp.event_get_websocket_svr_host_handle_timeout) {
                rsp.event_get_websocket_svr_host_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_get_websocket_svr_host_cb(uuid : number){
        var rsp = this.map_get_websocket_svr_host.get(uuid);
        this.map_get_websocket_svr_host.delete(uuid);
        return rsp;
    }

}

let rsp_cb_test_c2s_handle : test_c2s_rsp_cb | null = null;
export class test_c2s_caller {
    private _hubproxy:test_c2s_hubproxy;
    constructor(_client:client_handle.client){
        if (rsp_cb_test_c2s_handle == null){
            rsp_cb_test_c2s_handle = new test_c2s_rsp_cb(_client._modulemng);
        }
        this._hubproxy = new test_c2s_hubproxy(_client);
    }

    public get_hub(hub_name:string)
    {
        this._hubproxy.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5 = hub_name;
        return this._hubproxy;
    }

}

export class test_c2s_hubproxy
{
    private uuid_c233fb06_7c62_3839_a7d5_edade25b16c5 : number = Math.round(Math.random() * Number.MAX_VALUE);

    public hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5:string;
    private _client_handle:client_handle.client;

    constructor(client_handle_:client_handle.client)
    {
        this._client_handle = client_handle_;
    }

    public login(){
        let _argv_d3bb20a7_d0fc_3440_bb9e_b3cc0630e2d1:any[] = [];
        this._client_handle.call_hub(this.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s", "login", _argv_d3bb20a7_d0fc_3440_bb9e_b3cc0630e2d1);
    }

    public get_svr_host(){
        let uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65 = this.uuid_c233fb06_7c62_3839_a7d5_edade25b16c5++;

        let _argv_abbb842f_52d0_34e7_9d8d_642d072db165:any[] = [uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65];
        this._client_handle.call_hub(this.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s", "get_svr_host", _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        let cb_get_svr_host_obj = new test_c2s_get_svr_host_cb(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65, rsp_cb_test_c2s_handle);
        if (rsp_cb_test_c2s_handle){
            rsp_cb_test_c2s_handle.map_get_svr_host.set(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65, cb_get_svr_host_obj);
        }
        return cb_get_svr_host_obj;
    }

    public get_websocket_svr_host(){
        let uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5 = this.uuid_c233fb06_7c62_3839_a7d5_edade25b16c5++;

        let _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f:any[] = [uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5];
        this._client_handle.call_hub(this.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s", "get_websocket_svr_host", _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);
        let cb_get_websocket_svr_host_obj = new test_c2s_get_websocket_svr_host_cb(uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5, rsp_cb_test_c2s_handle);
        if (rsp_cb_test_c2s_handle){
            rsp_cb_test_c2s_handle.map_get_websocket_svr_host.set(uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5, cb_get_websocket_svr_host_obj);
        }
        return cb_get_websocket_svr_host_obj;
    }

}
