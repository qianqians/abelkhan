/*this caller file is codegen by juggle for js*/
function gm_center_caller(ch){
    Icaller.call(this, "gm_center", ch);

    this.confirm_gm = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "confirm_gm", _argv);
    }

    this.close_clutter = function( argv0){
        var _argv = [argv0];
        this.call_module_method.call(this, "close_clutter", _argv);
    }

    this.close_zone = function( argv0, argv1){
        var _argv = [argv0,argv1];
        this.call_module_method.call(this, "close_zone", _argv);
    }

    this.reload = function( argv0, argv1){
        var _argv = [argv0,argv1];
        this.call_module_method.call(this, "reload", _argv);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Icaller.prototype;
    gm_center_caller.prototype = new Super();
})();
gm_center_caller.prototype.constructor = gm_center_caller;

