/*this module file is codegen by juggle for js*/
function hub_call_hub_module(){
    eventobj.call(this);
    Imodule.call(this, "hub_call_hub");

    this.reg_hub = function(argv0){
        this.call_event("reg_hub", [argv0]);
    }

    this.reg_hub_sucess = function(){
        this.call_event("reg_hub_sucess", []);
    }

    this.hub_call_hub_mothed = function(argv0, argv1, argv2){
        this.call_event("hub_call_hub_mothed", [argv0, argv1, argv2]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    hub_call_hub_module.prototype = new Super();
})();
hub_call_hub_module.prototype.constructor = hub_call_hub_module;

