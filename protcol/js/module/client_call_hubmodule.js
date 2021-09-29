/*this module file is codegen by juggle for js*/
function client_call_hub_module(){
    eventobj.call(this);
    Imodule.call(this, "client_call_hub");

    this.client_connect = function(argv0){
        this.call_event("client_connect", [argv0]);
    }

    this.call_hub = function(argv0, argv1, argv2, argv3){
        this.call_event("call_hub", [argv0, argv1, argv2, argv3]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    client_call_hub_module.prototype = new Super();
})();
client_call_hub_module.prototype.constructor = client_call_hub_module;

