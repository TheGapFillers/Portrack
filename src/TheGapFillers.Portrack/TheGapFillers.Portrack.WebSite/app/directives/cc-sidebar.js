/// <reference path="../app.ts" />
/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/jquery/jquery.d.ts" />
'use strict';
var App;
(function (App) {
    var Directives;
    (function (Directives) {
        var CcSidebar = (function () {
            function CcSidebar() {
                this.restrict = "A";
                this.link = function (scope, element, attrs) {
                    var $sidebarInner = element.find('.sidebar-inner');
                    var $dropdownElement = element.find('.sidebar-dropdown a');
                    element.addClass('sidebar');
                    $dropdownElement.click(dropdown);
                    var dropClass = 'dropy';
                    function dropdown(e) {
                        e.preventDefault();
                        if (!$dropdownElement.hasClass(dropClass)) {
                            hideAllSidebars();
                            $sidebarInner.slideDown(350);
                            $dropdownElement.addClass(dropClass);
                        }
                        else if ($dropdownElement.hasClass(dropClass)) {
                            $dropdownElement.removeClass(dropClass);
                            $sidebarInner.slideUp(350);
                        }
                    }
                    function hideAllSidebars() {
                        $sidebarInner.slideUp(350);
                        $('.sidebar-dropdown a').removeClass(dropClass);
                    }
                };
            }
            CcSidebar.directiveId = 'ccSidebar';
            return CcSidebar;
        })();
        //References angular app
        App.app.directive(CcSidebar.directiveId, [function () { return new CcSidebar(); }]);
    })(Directives = App.Directives || (App.Directives = {}));
})(App || (App = {}));
//# sourceMappingURL=cc-sidebar.js.map