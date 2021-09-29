/*this module file is codegen by juggle for js*/
function center_call_hub_module(){
    eventobj.call(this);
    Imodule.call(this, "center_call_hub");

    this.distribute_server_address = function(argv0, argv1, argv2, argv3){
        this.call_event("distribute_server_address", [argv0, argv1, argv2, argv3]);
    }

    this.reload = function(argv0){
        this.call_event("reload", [argv0]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    center_call_hub_module.prototype = new Super();
})();
center_call_hub_module.prototype.constructor = center_call_hub_module;

