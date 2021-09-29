/*this caller file is codegen by juggle for js*/
function center_caller(ch){
    Icaller.call(this, "center", ch);

    this.reg_server = function( argv0, argv1, argv2, argv3){
        var _argv = [argv0,argv1,argv2,argv3];
        this.call_module_method.call(this, "reg_server", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    center_caller.prototype = new Super();
})();
center_caller.prototype.constructor = center_caller;

