/*this caller file is codegen by juggle for js*/
function hub_call_center_caller(ch){
    Icaller.call(this, "hub_call_center", ch);

    this.closed = function(){
        var _argv = [];
        this.call_module_method.call(this, "closed", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    hub_call_center_caller.prototype = new Super();
})();
hub_call_center_caller.prototype.constructor = hub_call_center_caller;

