/// <reference path="../app.ts" />
/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/jquery/jquery.d.ts" />
'use strict';
var App;
(function (App) {
    var Directives;
    (function (Directives) {
        var CcSpinner = (function () {
            function CcSpinner($window) {
                var _this = this;
                this.$window = $window;
                this.restrict = "A";
                this.link = function (scope, element, attrs) {
                    scope.spinner = null;
                    scope.$watch(attrs.ccSpinner, function (options) {
                        if (scope.spinner) {
                            scope.spinner.stop();
                        }
                        scope.spinner = new _this.$window.Spinner(options);
                        scope.spinner.spin(element[0]);
                    }, true);
                };
            }
            CcSpinner.directiveId = 'ccSpinner';
            return CcSpinner;
        })();
        // Register in angular app
        App.app.directive(CcSpinner.directiveId, ['$window', function ($window) { return new CcSpinner($window); }]);
    })(Directives = App.Directives || (App.Directives = {}));
})(App || (App = {}));
//# sourceMappingURL=cc-spinner.js.map