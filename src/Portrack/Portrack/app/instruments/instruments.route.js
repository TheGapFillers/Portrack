(function () {
    'use strict';
    angular.module('app.instruments').config([
        '$routeProvider',
        '$locationProvider',
        config
    ]);
    function config($routeProvider, $locationProvider) {
        $routeProvider.when('/instruments/:id', {
            template: 'Instruments',
            controller: function () {
            },
            controllerAs: 'vm'
        });
    }
})();
//# sourceMappingURL=instruments.route.js.map