'use strict';
var App;
(function (App) {
    App.app = angular.module('app', [
        'ngAnimate',
        'ngRoute',
        'ngSanitize',
        'common',
        'common.bootstrap',
        'breeze.angular',
        'breeze.directives',
        'ui.bootstrap'
    ]);
    // Handle routing errors and success events
    App.app.run(['$route', function ($route) {
        // Include $route to kick start the router.
    }]);
})(App || (App = {}));
//# sourceMappingURL=app.js.map