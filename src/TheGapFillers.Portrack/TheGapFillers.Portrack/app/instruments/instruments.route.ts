((): void => {
    'use strict';

    angular
        .module('app.instruments')
        .config([
        '$routeProvider',
        '$locationProvider',
        config]);

    function config(
        $routeProvider: ng.route.IRouteProvider,
        $locationProvider: ng.ILocationProvider): void {
        $routeProvider
            .when('/instruments/:id', {
            template: 'Instruments',
            controller: (): void => {
            },
            controllerAs: 'vm'
        });
    }
})(); 