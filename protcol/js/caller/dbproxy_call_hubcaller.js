/*this caller file is codegen by juggle for js*/
function dbproxy_call_hub_caller(ch){
    Icaller.call(this, "dbproxy_call_hub", ch);

    this.reg_hub_sucess = function(){
        var _argv = [];
        this.call_module_method.call(this, "reg_hub_sucess", _argv);
    }

    this.ack_create_persisted_object = function( argv0, argv1){
        var _argv = [argv0,argv1];
        this.call_module_method.call(this, "ack_create_persisted_object", _argv);
    }

    this.ack_updata_persisted_object = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "ack_updata_persisted_object", _argv);
    }

    this.ack_get_object_count = function( argv0, argv1){
        var _argv = [argv0,argv1];
        this.call_module_method.call(this, "ack_get_object_count", _argv);
    }

    this.ack_get_object_info = function( argv0, argv1){
        var _argv = [argv0,argv1];
        this.call_module_method.call(this, "ack_get_object_info", _argv);
    }

    this.ack_get_object_info_end = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "ack_get_object_info_end", _argv);
    }

    this.ack_remove_object = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "ack_remove_object", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    dbproxy_call_hub_caller.prototype = new Super();
})();
dbproxy_call_hub_caller.prototype.constructor = dbproxy_call_hub_caller;

