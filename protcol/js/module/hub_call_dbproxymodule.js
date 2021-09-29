/*this module file is codegen by juggle for js*/
function hub_call_dbproxy_module(){
    eventobj.call(this);
    Imodule.call(this, "hub_call_dbproxy");

    this.reg_hub = function(argv0){
        this.call_event("reg_hub", [argv0]);
    }

    this.create_persisted_object = function(argv0, argv1, argv2, argv3){
        this.call_event("create_persisted_object", [argv0, argv1, argv2, argv3]);
    }

    this.updata_persisted_object = function(argv0, argv1, argv2, argv3, argv4){
        this.call_event("updata_persisted_object", [argv0, argv1, argv2, argv3, argv4]);
    }

    this.get_object_count = function(argv0, argv1, argv2, argv3){
        this.call_event("get_object_count", [argv0, argv1, argv2, argv3]);
    }

    this.get_object_info = function(argv0, argv1, argv2, argv3){
        this.call_event("get_object_info", [argv0, argv1, argv2, argv3]);
    }

    this.remove_object = function(argv0, argv1, argv2, argv3){
        this.call_event("remove_object", [argv0, argv1, argv2, argv3]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    hub_call_dbproxy_module.prototype = new Super();
})();
hub_call_dbproxy_module.prototype.constructor = hub_call_dbproxy_module;

