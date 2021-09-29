/*this module file is codegen by juggle for js*/
function gate_call_hub_module(){
    eventobj.call(this);
    Imodule.call(this, "gate_call_hub");

    this.reg_hub_sucess = function(){
        this.call_event("reg_hub_sucess", []);
    }

    this.client_connect = function(argv0){
        this.call_event("client_connect", [argv0]);
    }

    this.client_disconnect = function(argv0){
        this.call_event("client_disconnect", [argv0]);
    }

    this.client_exception = function(argv0){
        this.call_event("client_exception", [argv0]);
    }

    this.client_call_hub = function(argv0, argv1, argv2, argv3){
        this.call_event("client_call_hub", [argv0, argv1, argv2, argv3]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    gate_call_hub_module.prototype = new Super();
})();
gate_call_hub_module.prototype.constructor = gate_call_hub_module;

