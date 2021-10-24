import * as client_handle from "client_handle";
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
/*this module code is codegen by abelkhan codegen for typescript*/
export class test_s2c_ping_rsp {
    private uuid_94d71f95_a670_3916_89a9_44df18fb711b : number;
    private hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d:string;
    private _client_handle:client_handle.client ;

    constructor(_client_handle_:client_handle.client, current_hub:string, _uuid:number){
        this._client_handle = _client_handle_;
        this.hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d = current_hub;
        this.uuid_94d71f95_a670_3916_89a9_44df18fb711b = _uuid;
    }

    public rsp(){
        let _argv_ca6794ee_a403_309d_b40e_f37578d53e8d:any[] = [this.uuid_94d71f95_a670_3916_89a9_44df18fb711b];
        this._client_handle.call_hub(this.hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d, "test_s2c_rsp_cb", "ping_rsp", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);
    }

    public err(){
        let _argv_ca6794ee_a403_309d_b40e_f37578d53e8d:any[] = [this.uuid_94d71f95_a670_3916_89a9_44df18fb711b];
        this._client_handle.call_hub(this.hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d, "test_s2c_rsp_cb", "ping_err", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);
    }

}

export class test_s2c_module extends client_handle.imodule {
    public _client_handle:client_handle.client;
    constructor(_client_handle_:client_handle.client){
        super();
        this._client_handle = _client_handle_;
        this._client_handle._modulemng.add_module("test_s2c", this);

        this.reg_cb("ping", this.ping.bind(this));

        this.cb_ping = null;

    }

    public cb_ping : ()=>void | null;
    ping(inArray:any[]){
        let _cb_uuid = inArray[0];
        let _argv_:any[] = [];
        this.rsp = new test_s2c_ping_rsp(this._client_handle, this._client_handle.current_hub, _cb_uuid);
        if (this.cb_ping){
            this.cb_ping.apply(null, _argv_);
        }
        this.rsp = null;
    }

}
