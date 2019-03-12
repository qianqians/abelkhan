/*this caller file is codegen by juggle for js*/
function hub_call_hub_caller(ch){
    Icaller.call(this, "hub_call_hub", ch);

    this.reg_hub = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "reg_hub", _argv);
    }

    this.reg_hub_sucess = function(){
        var _argv = [];
        this.call_module_method.call(this, "reg_hub_sucess", _argv);
    }

    this.hub_call_hub_mothed = function( argv0, argv1, argv2){
        var _argv = [argv0,argv1,argv2];
        this.call_module_method.call(this, "hub_call_hub_mothed", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    hub_call_hub_caller.prototype = new Super();
})();
hub_call_hub_caller.prototype.constructor = hub_call_hub_caller;

