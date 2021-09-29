/*this caller file is codegen by juggle for js*/
function gate_call_client_caller(ch){
    Icaller.call(this, "gate_call_client", ch);

    this.ntf_uuid = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "ntf_uuid", _argv);
    }

    this.connect_gate_sucess = function(){
        var _argv = [];
        this.call_module_method.call(this, "connect_gate_sucess", _argv);
    }

    this.connect_hub_sucess = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "connect_hub_sucess", _argv);
    }

    this.ack_heartbeats = function(){
        var _argv = [];
        this.call_module_method.call(this, "ack_heartbeats", _argv);
    }

    this.call_client = function( argv0, argv1, argv2){
        var _argv = [argv0,argv1,argv2];
        this.call_module_method.call(this, "call_client", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    gate_call_client_caller.prototype = new Super();
})();
gate_call_client_caller.prototype.constructor = gate_call_client_caller;

