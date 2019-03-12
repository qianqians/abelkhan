/*this caller file is codegen by juggle for js*/
function client_call_hub_caller(ch){
    Icaller.call(this, "client_call_hub", ch);

    this.client_connect = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "client_connect", _argv);
    }

    this.call_hub = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "call_hub", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    client_call_hub_caller.prototype = new Super();
})();
client_call_hub_caller.prototype.constructor = client_call_hub_caller;

