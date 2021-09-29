/*this module file is codegen by juggle for js*/
function hub_call_center_module(){
    eventobj.call(this);
    Imodule.call(this, "hub_call_center");

    this.closed = function(){
        this.call_event("closed", []);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    hub_call_center_module.prototype = new Super();
})();
hub_call_center_module.prototype.constructor = hub_call_center_module;

