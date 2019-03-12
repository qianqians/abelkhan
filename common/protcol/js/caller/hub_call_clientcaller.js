/*this caller file is codegen by juggle for js*/
function hub_call_client_caller(ch){
    Icaller.call(this, "hub_call_client", ch);

    this.call_client = function( argv0, argv1, argv2){
        var _argv = [argv0,argv1,argv2];
        this.call_module_method.call(this, "call_client", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    hub_call_client_caller.prototype = new Super();
})();
hub_call_client_caller.prototype.constructor = hub_call_client_caller;

