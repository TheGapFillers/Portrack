module App {
    'use strict';

    export var app = angular.module('portTrack', [
        'ngRoute', // routing module
    ]);
    
    app.config(['$routeProvider',
        ($routeProvider: ng.route.IRouteProvider) => {
            $routeProvider.when("/application", {
                templateUrl: 'app/application/application.html',
                controller: 'ApplicationController'
            })

                .otherwise({

                templateUrl: 'app/home/home.html'
            })
        }
    ]);

    // Handle routing errors and success events
    app.run(['$route', function ($route) {
        // Include $route to kick start the router.
    }]);
}