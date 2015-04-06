/// <reference path="../scripts/typings/angularjs/angular.d.ts" />
'use strict';
var App;
(function (App) {
    var RouteConfigurator = (function () {
        function RouteConfigurator($routeProvider, routes) {
            routes.forEach(function (r) {
                $routeProvider.when(r.url, r.config);
            });
            $routeProvider.otherwise({ redirectTo: '/' });
        }
        return RouteConfigurator;
    })();
    App.RouteConfigurator = RouteConfigurator;
    // Define the routes - since this goes right to an app.constant, no use for a class
    // Could make it a static property of the RouteConfigurator class
    function getRoutes() {
        return [
            {
                url: '/',
                config: {
                    templateUrl: 'app/dashboard/dashboard.html',
                    title: 'dashboard',
                    settings: {
                        nav: 1,
                        content: '<i class="fa fa-dashboard"></i> Dashboard'
                    }
                }
            },
            {
                url: '/admin',
                config: {
                    title: 'admin',
                    templateUrl: 'app/admin/admin.html',
                    settings: {
                        nav: 2,
                        content: '<i class="fa fa-lock"></i> Admin'
                    }
                }
            }
        ];
    }
    // Collect the routes
    App.app.constant('routes', getRoutes());
    // Configure the routes and route resolvers
    App.app.config([
        '$routeProvider',
        'routes',
        function ($routeProvider, routes) { return new RouteConfigurator($routeProvider, routes); }
    ]);
})(App || (App = {}));
//# sourceMappingURL=config.route.js.map