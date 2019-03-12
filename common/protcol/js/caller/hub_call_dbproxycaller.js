/*this caller file is codegen by juggle for js*/
function hub_call_dbproxy_caller(ch){
    Icaller.call(this, "hub_call_dbproxy", ch);

    this.reg_hub = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "reg_hub", _argv);
    }

    this.create_persisted_object = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "create_persisted_object", _argv);
    }

    this.updata_persisted_object = function( argv0, argv1, argv2, argv3, argv4){
        var _argv = [argv0,argv1,argv2,argv3,argv4];
        this.call_module_method.call(this, "updata_persisted_object", _argv);
    }

    this.get_object_count = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "get_object_count", _argv);
    }

    this.get_object_info = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "get_object_info", _argv);
    }

    this.remove_object = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "remove_object", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    hub_call_dbproxy_caller.prototype = new Super();
})();
hub_call_dbproxy_caller.prototype.constructor = hub_call_dbproxy_caller;

