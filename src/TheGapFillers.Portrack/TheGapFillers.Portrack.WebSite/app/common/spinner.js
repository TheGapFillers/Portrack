/// <reference path="common.ts" />
/// <reference path="commonconfig.ts" />
'use strict';
var App;
(function (App) {
    var Shared;
    (function (Shared) {
        var Spinner = (function () {
            function Spinner(common, commonConfig) {
                this.common = common;
                this.commonConfig = commonConfig;
            }
            Spinner.prototype.spinnerHide = function () {
                this.spinnerToggle(false);
            };
            Spinner.prototype.spinnerShow = function () {
                this.spinnerToggle(true);
            };
            Spinner.prototype.spinnerToggle = function (show) {
                this.common.$broadcast(this.commonConfig.config.spinnerToggleEvent, { show: show });
            };
            Spinner.serviceId = 'spinner';
            return Spinner;
        })();
        Shared.Spinner = Spinner;
        // Must configure the common service and set its 
        // events via the commonConfigProvider
        Shared.commonModule.factory(Spinner.serviceId, ['common', 'commonConfig', function (c, cC) { return new Spinner(c, cC); }]);
    })(Shared = App.Shared || (App.Shared = {}));
})(App || (App = {}));
//# sourceMappingURL=spinner.js.map