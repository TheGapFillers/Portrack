/// <reference path="../app.ts" />
/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/jquery/jquery.d.ts" />
'use strict';
var App;
(function (App) {
    var Directives;
    (function (Directives) {
        var CcMenuItemRendered = (function () {
            function CcMenuItemRendered($timeout) {
                var _this = this;
                this.$timeout = $timeout;
                this.restrict = "A";
                this.link = function (scope, element, attrs) {
                    // Makes shure the menu closes after click on menuitem when viewed on a small screen
                    // <li class="nlightblue fade-selection-animation" data-ng-class="vm.isCurrent(r)"
                    // data-ng-repeat = "r in vm.navRoutes" >
                    // <a href = "#{{r.url}}" data-ng-bind-html = "r.config.settings.content" data-cc-menu-item-rendered >< / a >
                    // app.directive('ccMenuItemRendered',['$timeout', ccMenuItemRendered]);
                    // inspiration: http://stackoverflow.com/questions/15207788/calling-a-function-when-ng-repeat-has-finished
                    if (scope.$last === true) {
                        _this.$timeout(function () {
                            scope.$emit(attrs.onFinishRender);
                            var $menuItem = element.parent().parent().find('a');
                            $menuItem.click(function () {
                                if ($('.sidebar-dropdown a').hasClass('dropy')) {
                                    hideDropDown();
                                }
                            });
                        });
                    }
                    function hideDropDown() {
                        var $sidebarInner = $('.sidebar-inner');
                        $sidebarInner.slideUp(350);
                        $('.sidebar-dropdown a').removeClass('dropy');
                    }
                };
            }
            CcMenuItemRendered.directiveId = 'ccMenuItemRendered';
            return CcMenuItemRendered;
        })();
        // register in angular app
        App.app.directive(CcMenuItemRendered.directiveId, ['$timeout', function ($timeout) { return new CcMenuItemRendered($timeout); }]);
    })(Directives = App.Directives || (App.Directives = {}));
})(App || (App = {}));
//# sourceMappingURL=cc-menu-item-rendered.js.map