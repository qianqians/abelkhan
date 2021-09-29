/*this module file is codegen by juggle for js*/
function hub_call_gate_module(){
    eventobj.call(this);
    Imodule.call(this, "hub_call_gate");

    this.reg_hub = function(argv0, argv1){
        this.call_event("reg_hub", [argv0, argv1]);
    }

    this.connect_sucess = function(argv0){
        this.call_event("connect_sucess", [argv0]);
    }

    this.disconnect_client = function(argv0){
        this.call_event("disconnect_client", [argv0]);
    }

    this.forward_hub_call_client = function(argv0, argv1, argv2, argv3){
        this.call_event("forward_hub_call_client", [argv0, argv1, argv2, argv3]);
    }

    this.forward_hub_call_group_client = function(argv0, argv1, argv2, argv3){
        this.call_event("forward_hub_call_group_client", [argv0, argv1, argv2, argv3]);
    }

    this.forward_hub_call_global_client = function(argv0, argv1, argv2){
        this.call_event("forward_hub_call_global_client", [argv0, argv1, argv2]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    hub_call_gate_module.prototype = new Super();
})();
hub_call_gate_module.prototype.constructor = hub_call_gate_module;

