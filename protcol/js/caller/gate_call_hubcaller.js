/*this caller file is codegen by juggle for js*/
function gate_call_hub_caller(ch){
    Icaller.call(this, "gate_call_hub", ch);

    this.reg_hub_sucess = function(){
        var _argv = [];
        this.call_module_method.call(this, "reg_hub_sucess", _argv);
    }

    this.client_connect = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "client_connect", _argv);
    }

    this.client_disconnect = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "client_disconnect", _argv);
    }

    this.client_exception = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "client_exception", _argv);
    }

    this.client_call_hub = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "client_call_hub", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    gate_call_hub_caller.prototype = new Super();
})();
gate_call_hub_caller.prototype.constructor = gate_call_hub_caller;

