/*this caller file is codegen by juggle for js*/
function center_call_hub_caller(ch){
    Icaller.call(this, "center_call_hub", ch);

    this.distribute_server_address = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "distribute_server_address", _argv);
    }

    this.reload = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "reload", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    center_call_hub_caller.prototype = new Super();
})();
center_call_hub_caller.prototype.constructor = center_call_hub_caller;

