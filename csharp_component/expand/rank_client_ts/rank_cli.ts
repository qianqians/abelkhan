import * as client_handle from "./client_handle";
import * as rank_comm from "./rank_comm";
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
export class rank_cli_service_get_rank_guid_cb{
    private cb_uuid : number;
    private module_rsp_cb : rank_cli_service_rsp_cb;

    public event_get_rank_guid_handle_cb : (rank:rank_comm.rank_item)=>void | null;
    public event_get_rank_guid_handle_err : ()=>void | null;
    public event_get_rank_guid_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : rank_cli_service_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_rank_guid_handle_cb = null;
        this.event_get_rank_guid_handle_err = null;
        this.event_get_rank_guid_handle_timeout = null;
    }

    callBack(_cb:(rank:rank_comm.rank_item)=>void, _err:()=>void)
    {
        this.event_get_rank_guid_handle_cb = _cb;
        this.event_get_rank_guid_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.get_rank_guid_timeout(this.cb_uuid); }, tick);
        this.event_get_rank_guid_handle_timeout = timeout_cb;
    }

}

export class rank_cli_service_get_rank_range_cb{
    private cb_uuid : number;
    private module_rsp_cb : rank_cli_service_rsp_cb;

    public event_get_rank_range_handle_cb : (rank_list:rank_comm.rank_item[])=>void | null;
    public event_get_rank_range_handle_err : ()=>void | null;
    public event_get_rank_range_handle_timeout : ()=>void | null;
    constructor(_cb_uuid : number, _module_rsp_cb : rank_cli_service_rsp_cb){
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_rank_range_handle_cb = null;
        this.event_get_rank_range_handle_err = null;
        this.event_get_rank_range_handle_timeout = null;
    }

    callBack(_cb:(rank_list:rank_comm.rank_item[])=>void, _err:()=>void)
    {
        this.event_get_rank_range_handle_cb = _cb;
        this.event_get_rank_range_handle_err = _err;
        return this;
    }

    timeout(tick:number, timeout_cb:()=>void)
    {
        setTimeout(()=>{ this.module_rsp_cb.get_rank_range_timeout(this.cb_uuid); }, tick);
        this.event_get_rank_range_handle_timeout = timeout_cb;
    }

}

/*this cb code is codegen by abelkhan for ts*/
export class rank_cli_service_rsp_cb extends client_handle.imodule {
    public map_get_rank_guid:Map<number, rank_cli_service_get_rank_guid_cb>;
    public map_get_rank_range:Map<number, rank_cli_service_get_rank_range_cb>;
    constructor(modules:client_handle.modulemng){
        super();
        this.map_get_rank_guid = new Map<number, rank_cli_service_get_rank_guid_cb>();
        modules.add_method("rank_cli_service_rsp_cb_get_rank_guid_rsp", this.get_rank_guid_rsp.bind(this));
        modules.add_method("rank_cli_service_rsp_cb_get_rank_guid_err", this.get_rank_guid_err.bind(this));
        this.map_get_rank_range = new Map<number, rank_cli_service_get_rank_range_cb>();
        modules.add_method("rank_cli_service_rsp_cb_get_rank_range_rsp", this.get_rank_range_rsp.bind(this));
        modules.add_method("rank_cli_service_rsp_cb_get_rank_range_err", this.get_rank_range_err.bind(this));
    }
    public get_rank_guid_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_90f752ce_ee17_38de_b679_4a35e21e4129:any[] = [];
        _argv_90f752ce_ee17_38de_b679_4a35e21e4129.push(rank_comm.protcol_to_rank_item(inArray[1]));
        var rsp = this.try_get_and_del_get_rank_guid_cb(uuid);
        if (rsp && rsp.event_get_rank_guid_handle_cb) {
            rsp.event_get_rank_guid_handle_cb.apply(null, _argv_90f752ce_ee17_38de_b679_4a35e21e4129);
        }
    }

    public get_rank_guid_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_90f752ce_ee17_38de_b679_4a35e21e4129:any[] = [];
        var rsp = this.try_get_and_del_get_rank_guid_cb(uuid);
        if (rsp && rsp.event_get_rank_guid_handle_err) {
            rsp.event_get_rank_guid_handle_err.apply(null, _argv_90f752ce_ee17_38de_b679_4a35e21e4129);
        }
    }

    public get_rank_guid_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_get_rank_guid_cb(cb_uuid);
        if (rsp){
            if (rsp.event_get_rank_guid_handle_timeout) {
                rsp.event_get_rank_guid_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_get_rank_guid_cb(uuid : number){
        var rsp = this.map_get_rank_guid.get(uuid);
        this.map_get_rank_guid.delete(uuid);
        return rsp;
    }

    public get_rank_range_rsp(inArray:any[]){
        let uuid = inArray[0];
        let _argv_17367d36_e3ba_3b3f_87b8_4f982846a886:any[] = [];
        let _array_6fa951c6_19a5_56e0_806f_8685b83ba249:any[] = [];        for(let v_e249ecbd_d64c_526b_901b_6d4ddee4b75a of inArray[1]){
            _array_6fa951c6_19a5_56e0_806f_8685b83ba249.push(rank_comm.protcol_to_rank_item(v_e249ecbd_d64c_526b_901b_6d4ddee4b75a));
        }
        _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.push(_array_6fa951c6_19a5_56e0_806f_8685b83ba249);
        var rsp = this.try_get_and_del_get_rank_range_cb(uuid);
        if (rsp && rsp.event_get_rank_range_handle_cb) {
            rsp.event_get_rank_range_handle_cb.apply(null, _argv_17367d36_e3ba_3b3f_87b8_4f982846a886);
        }
    }

    public get_rank_range_err(inArray:any[]){
        let uuid = inArray[0];
        let _argv_17367d36_e3ba_3b3f_87b8_4f982846a886:any[] = [];
        var rsp = this.try_get_and_del_get_rank_range_cb(uuid);
        if (rsp && rsp.event_get_rank_range_handle_err) {
            rsp.event_get_rank_range_handle_err.apply(null, _argv_17367d36_e3ba_3b3f_87b8_4f982846a886);
        }
    }

    public get_rank_range_timeout(cb_uuid : number){
        let rsp = this.try_get_and_del_get_rank_range_cb(cb_uuid);
        if (rsp){
            if (rsp.event_get_rank_range_handle_timeout) {
                rsp.event_get_rank_range_handle_timeout.apply(null);
            }
        }
    }

    private try_get_and_del_get_rank_range_cb(uuid : number){
        var rsp = this.map_get_rank_range.get(uuid);
        this.map_get_rank_range.delete(uuid);
        return rsp;
    }

}

let rsp_cb_rank_cli_service_handle : rank_cli_service_rsp_cb | null = null;
export class rank_cli_service_caller {
    private _hubproxy:rank_cli_service_hubproxy;
    constructor(_client:client_handle.client){
        if (rsp_cb_rank_cli_service_handle == null){
            rsp_cb_rank_cli_service_handle = new rank_cli_service_rsp_cb(_client._modulemng);
        }
        this._hubproxy = new rank_cli_service_hubproxy(_client);
    }

    public get_hub(hub_name:string)
    {
        this._hubproxy.hub_name_cdcf8098_c212_361e_95df_8b4d8ec6340e = hub_name;
        return this._hubproxy;
    }

}

export class rank_cli_service_hubproxy
{
    private uuid_cdcf8098_c212_361e_95df_8b4d8ec6340e : number = Math.round(Math.random() * 1000);

    public hub_name_cdcf8098_c212_361e_95df_8b4d8ec6340e:string;
    private _client_handle:client_handle.client;

    constructor(client_handle_:client_handle.client)
    {
        this._client_handle = client_handle_;
    }

    public get_rank_guid(rank_name:string, guid:number){
        let uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325 = Math.round(this.uuid_cdcf8098_c212_361e_95df_8b4d8ec6340e++);

        let _argv_90f752ce_ee17_38de_b679_4a35e21e4129:any[] = [uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325];
        _argv_90f752ce_ee17_38de_b679_4a35e21e4129.push(rank_name);
        _argv_90f752ce_ee17_38de_b679_4a35e21e4129.push(guid);
        this._client_handle.call_hub(this.hub_name_cdcf8098_c212_361e_95df_8b4d8ec6340e, "rank_cli_service_get_rank_guid", _argv_90f752ce_ee17_38de_b679_4a35e21e4129);
        let cb_get_rank_guid_obj = new rank_cli_service_get_rank_guid_cb(uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325, rsp_cb_rank_cli_service_handle);
        if (rsp_cb_rank_cli_service_handle){
            rsp_cb_rank_cli_service_handle.map_get_rank_guid.set(uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325, cb_get_rank_guid_obj);
        }
        return cb_get_rank_guid_obj;
    }

    public get_rank_range(rank_name:string, start:number, end:number){
        let uuid_502cc321_083d_54f3_be2e_94e03d09d51d = Math.round(this.uuid_cdcf8098_c212_361e_95df_8b4d8ec6340e++);

        let _argv_17367d36_e3ba_3b3f_87b8_4f982846a886:any[] = [uuid_502cc321_083d_54f3_be2e_94e03d09d51d];
        _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.push(rank_name);
        _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.push(start);
        _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.push(end);
        this._client_handle.call_hub(this.hub_name_cdcf8098_c212_361e_95df_8b4d8ec6340e, "rank_cli_service_get_rank_range", _argv_17367d36_e3ba_3b3f_87b8_4f982846a886);
        let cb_get_rank_range_obj = new rank_cli_service_get_rank_range_cb(uuid_502cc321_083d_54f3_be2e_94e03d09d51d, rsp_cb_rank_cli_service_handle);
        if (rsp_cb_rank_cli_service_handle){
            rsp_cb_rank_cli_service_handle.map_get_rank_range.set(uuid_502cc321_083d_54f3_be2e_94e03d09d51d, cb_get_rank_range_obj);
        }
        return cb_get_rank_range_obj;
    }

}
