/*this module file is codegen by juggle for js*/
function client_call_gate_module(){
    eventobj.call(this);
    Imodule.call(this, "client_call_gate");

    this.connect_server = function(argv0, argv1){
        this.call_event("connect_server", [argv0, argv1]);
    }

    this.cancle_server = function(){
        this.call_event("cancle_server", []);
    }

    this.connect_hub = function(argv0){
        this.call_event("connect_hub", [argv0]);
    }

    this.enable_heartbeats = function(){
        this.call_event("enable_heartbeats", []);
    }

    this.disable_heartbeats = function(){
        this.call_event("disable_heartbeats", []);
    }

    this.forward_client_call_hub = function(argv0, argv1, argv2, argv3){
        this.call_event("forward_client_call_hub", [argv0, argv1, argv2, argv3]);
    }

    this.heartbeats = function(argv0){
        this.call_event("heartbeats", [argv0]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    client_call_gate_module.prototype = new Super();
})();
client_call_gate_module.prototype.constructor = client_call_gate_module;

