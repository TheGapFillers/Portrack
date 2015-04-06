var App;
(function (App) {
    var Shared;
    (function (Shared) {
        //#region explanation
        //-------STARTING COMMON MODULE----------
        // NB! script for this file must get loaded before the "child" script files
        // THIS CREATES THE ANGULAR CONTAINER NAMED 'common', A BAG THAT HOLDS SERVICES
        // CREATION OF A MODULE IS DONE USING ...module('moduleName', []) => retrieved using ...module.('...')
        // Contains services:
        //  - common
        //  - logger
        //  - spinner
        //#endregion
        Shared.commonBootstrapModule = angular.module('common.bootstrap', []);
    })(Shared = App.Shared || (App.Shared = {}));
})(App || (App = {}));
//# sourceMappingURL=common.bootstapModule.js.map