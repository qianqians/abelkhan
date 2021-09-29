/*this caller file is codegen by juggle for js*/
function hub_call_gate_caller(ch){
    Icaller.call(this, "hub_call_gate", ch);

    this.reg_hub = function( argv0, argv1){
        var _argv = [argv0,argv1];
        this.call_module_method.call(this, "reg_hub", _argv);
    }

    this.connect_sucess = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "connect_sucess", _argv);
    }

    this.disconnect_client = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "disconnect_client", _argv);
    }

    this.forward_hub_call_client = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "forward_hub_call_client", _argv);
    }

    this.forward_hub_call_group_client = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "forward_hub_call_group_client", _argv);
    }

    this.forward_hub_call_global_client = function( argv0, argv1, argv2){
        var _argv = [argv0,argv1,argv2];
        this.call_module_method.call(this, "forward_hub_call_global_client", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    hub_call_gate_caller.prototype = new Super();
})();
hub_call_gate_caller.prototype.constructor = hub_call_gate_caller;

