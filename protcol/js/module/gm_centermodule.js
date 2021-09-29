/*this module file is codegen by juggle for js*/
function gm_center_module(){
    eventobj.call(this);
    Imodule.call(this, "gm_center");

    this.confirm_gm = function(argv0){
        this.call_event("confirm_gm", [argv0]);
    }

    this.close_clutter = function(argv0){
        this.call_event("close_clutter", [argv0]);
    }

    this.close_zone = function(argv0, argv1){
        this.call_event("close_zone", [argv0, argv1]);
    }

    this.reload = function(argv0, argv1){
        this.call_event("reload", [argv0, argv1]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    gm_center_module.prototype = new Super();
})();
gm_center_module.prototype.constructor = gm_center_module;

