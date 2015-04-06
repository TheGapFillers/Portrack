'use strict';
var App;
(function (App) {
    var Directives;
    (function (Directives) {
        var CcScrollToTop = (function () {
            function CcScrollToTop($window) {
                var _this = this;
                this.$window = $window;
                this.restrict = "A";
                this.template = '<a href="#"><i class="fa fa-chevron-up"></i></a>';
                this.link = function (scope, element, attrs) {
                    var $win = $(_this.$window);
                    element.addClass('totop');
                    $win.scroll(toggleIcon);
                    element.find('a').click(function (e) {
                        e.preventDefault();
                        // Learning Point: $anchorScroll works, but no animation
                        //$anchorScroll();
                        $('body').animate({ scrollTop: 0 }, 500);
                    });
                    function toggleIcon() {
                        $win.scrollTop() > 300 ? element.slideDown() : element.slideUp();
                    }
                };
            }
            CcScrollToTop.directiveId = 'ccScrollToTop';
            return CcScrollToTop;
        })();
        // Register in angular app
        App.app.directive(CcScrollToTop.directiveId, ['$window', function ($window) { return new CcScrollToTop($window); }]);
    })(Directives = App.Directives || (App.Directives = {}));
})(App || (App = {}));
//# sourceMappingURL=cc-scroll-to-top.js.map