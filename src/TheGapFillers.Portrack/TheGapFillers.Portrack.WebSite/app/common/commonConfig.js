/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
'use strict';
var App;
(function (App) {
    var Shared;
    (function (Shared) {
        var CommonConfig = (function () {
            function CommonConfig() {
                var _this = this;
                this.config = {};
                this.$get = function () {
                    return { config: _this.config };
                };
            }
            CommonConfig.providerId = 'commonConfig';
            return CommonConfig;
        })();
        Shared.commonModule.provider(CommonConfig.providerId, function () { return new CommonConfig(); });
    })(Shared = App.Shared || (App.Shared = {}));
})(App || (App = {}));
//# sourceMappingURL=commonConfig.js.map