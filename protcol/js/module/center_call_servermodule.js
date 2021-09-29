/*this module file is codegen by juggle for js*/
function center_call_server_module(){
    eventobj.call(this);
    Imodule.call(this, "center_call_server");

    this.reg_server_sucess = function(){
        this.call_event("reg_server_sucess", []);
    }

    this.close_server = function(){
        this.call_event("close_server", []);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    center_call_server_module.prototype = new Super();
})();
center_call_server_module.prototype.constructor = center_call_server_module;

