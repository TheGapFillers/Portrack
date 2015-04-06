/// <reference path="../common/common.ts" />
'use strict';
var App;
(function (App) {
    var Controllers;
    (function (Controllers) {
        var ShellCtrl = (function () {
            //#endregion
            function ShellCtrl($rootScope, common, config) {
                //#region Variables
                this.busyMessage = 'Please wait...';
                this.controllerId = ShellCtrl.controllerId;
                this.isBusy = true;
                this.spinnerOperations = {
                    radius: 40,
                    lines: 7,
                    length: 0,
                    width: 30,
                    speed: 1.7,
                    corners: 1.0,
                    trail: 100,
                    color: '#F58A00'
                };
                this.common = common;
                this.config = config;
                this.$rootScope = $rootScope;
                this.activate();
                this.registerEvents();
            }
            ShellCtrl.prototype.toggleSpinner = function (on) {
                this.isBusy = on;
            };
            ShellCtrl.prototype.activate = function () {
                var logger = this.common.logger.getLogFn(this.controllerId, 'success');
                logger('Hot Towel Angular loaded!', null, true);
                this.common.activateController([], this.controllerId);
            };
            ShellCtrl.prototype.registerEvents = function () {
                var _this = this;
                var events = this.config.events;
                this.$rootScope.$on('$routeChangeStart', function (event, next, current) {
                    _this.toggleSpinner(true);
                });
                this.$rootScope.$on(events.controllerActivateSuccess, function (data) {
                    _this.toggleSpinner(false);
                });
                this.$rootScope.$on(events.spinnerToggle, function (data) {
                    _this.toggleSpinner(data.show);
                });
            };
            ShellCtrl.controllerId = 'shellCtrl';
            return ShellCtrl;
        })();
        Controllers.ShellCtrl = ShellCtrl;
        // Register with angular
        App.app.controller(ShellCtrl.controllerId, ['$rootScope', 'common', 'config', function ($rS, com, con) { return new ShellCtrl($rS, com, con); }]);
    })(Controllers = App.Controllers || (App.Controllers = {}));
})(App || (App = {}));
//# sourceMappingURL=shellCtrl.js.map