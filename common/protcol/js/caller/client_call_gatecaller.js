/*this caller file is codegen by juggle for js*/
function client_call_gate_caller(ch){
    Icaller.call(this, "client_call_gate", ch);

    this.connect_server = function( argv0, argv1){
        var _argv = [argv0,argv1];
        this.call_module_method.call(this, "connect_server", _argv);
    }

    this.cancle_server = function(){
        var _argv = [];
        this.call_module_method.call(this, "cancle_server", _argv);
    }

    this.connect_hub = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "connect_hub", _argv);
    }

    this.enable_heartbeats = function(){
        var _argv = [];
        this.call_module_method.call(this, "enable_heartbeats", _argv);
    }

    this.disable_heartbeats = function(){
        var _argv = [];
        this.call_module_method.call(this, "disable_heartbeats", _argv);
    }

    this.forward_client_call_hub = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "forward_client_call_hub", _argv);
    }

    this.heartbeats = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "heartbeats", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    client_call_gate_caller.prototype = new Super();
})();
client_call_gate_caller.prototype.constructor = client_call_gate_caller;

