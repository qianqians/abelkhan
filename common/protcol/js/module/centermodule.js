/*this module file is codegen by juggle for js*/
function center_module(){
    eventobj.call(this);
    Imodule.call(this, "center");

    this.reg_server = function(argv0, argv1, argv2, argv3){
        this.call_event("reg_server", [argv0, argv1, argv2, argv3]);
    }

}
(function(){
    var Super = function(){};
    Super.prototype = Imodule.prototype;
    center_module.prototype = new Super();
})();
center_module.prototype.constructor = center_module;

