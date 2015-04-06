/// <reference path="../common/common.ts" />
'use strict';
var App;
(function (App) {
    var Controllers;
    (function (Controllers) {
        var AdminCtrl = (function () {
            //#endregion
            function AdminCtrl(common) {
                this.common = common;
                this.controllerId = AdminCtrl.controllerId;
                this.title = "Admin";
                this.log = this.common.logger.getLogFn(AdminCtrl.controllerId);
                this.activate([]);
            }
            //#region private methods
            AdminCtrl.prototype.activate = function (promises) {
                var _this = this;
                this.common.activateController([], AdminCtrl.controllerId).then(function () {
                    _this.log('Activated Admin View');
                });
            };
            AdminCtrl.controllerId = 'adminCtrl';
            return AdminCtrl;
        })();
        Controllers.AdminCtrl = AdminCtrl;
        // Register with angular
        App.app.controller(AdminCtrl.controllerId, ['common', function (common) { return new AdminCtrl(common); }]);
    })(Controllers = App.Controllers || (App.Controllers = {}));
})(App || (App = {}));
//# sourceMappingURL=admin.js.map