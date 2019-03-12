/*this caller file is codegen by juggle for js*/
function center_call_server_caller(ch){
    Icaller.call(this, "center_call_server", ch);

    this.reg_server_sucess = function(){
        var _argv = [];
        this.call_module_method.call(this, "reg_server_sucess", _argv);
    }

    this.close_server = function(){
        var _argv = [];
        this.call_module_method.call(this, "close_server", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    center_call_server_caller.prototype = new Super();
})();
center_call_server_caller.prototype.constructor = center_call_server_caller;

