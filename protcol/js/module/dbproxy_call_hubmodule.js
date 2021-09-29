/*this module file is codegen by juggle for js*/
function dbproxy_call_hub_module(){
    eventobj.call(this);
    Imodule.call(this, "dbproxy_call_hub");

    this.reg_hub_sucess = function(){
        this.call_event("reg_hub_sucess", []);
    }

    this.ack_create_persisted_object = function(argv0, argv1){
        this.call_event("ack_create_persisted_object", [argv0, argv1]);
    }

    this.ack_updata_persisted_object = function(argv0){
        this.call_event("ack_updata_persisted_object", [argv0]);
    }

    this.ack_get_object_count = function(argv0, argv1){
        this.call_event("ack_get_object_count", [argv0, argv1]);
    }

    this.ack_get_object_info = function(argv0, argv1){
        this.call_event("ack_get_object_info", [argv0, argv1]);
    }

    this.ack_get_object_info_end = function(argv0){
        this.call_event("ack_get_object_info_end", [argv0]);
    }

    this.ack_remove_object = function(argv0){
        this.call_event("ack_remove_object", [argv0]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    dbproxy_call_hub_module.prototype = new Super();
})();
dbproxy_call_hub_module.prototype.constructor = dbproxy_call_hub_module;

