/*this module file is codegen by juggle for js*/
function hub_call_client_module(){
    eventobj.call(this);
    Imodule.call(this, "hub_call_client");

    this.call_client = function(argv0, argv1, argv2){
        this.call_event("call_client", [argv0, argv1, argv2]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    hub_call_client_module.prototype = new Super();
})();
hub_call_client_module.prototype.constructor = hub_call_client_module;

