((): void => {
    'use strict';

    angular
        .module('app.portfolios')
        .config([
        '$routeProvider',
        '$locationProvider',
        config]);

    function config(
        $routeProvider: ng.route.IRouteProvider,
        $locationProvider: ng.ILocationProvider): void {
        $routeProvider
            .when('/portfolios/:id', {
            template: 'Portfolios',
            controller: (): void => {
            },
            controllerAs: 'vm'
        });
    }
})();  