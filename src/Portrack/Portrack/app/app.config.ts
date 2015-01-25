((): void => {
    'use strict';

    angular
        .module('app')
        .config(['$locationProvider', config]);

    function config($locationProvider: ng.ILocationProvider): void {
        $locationProvider.html5Mode(true);
    }
})(); 