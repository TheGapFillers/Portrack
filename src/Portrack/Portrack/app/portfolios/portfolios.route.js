(function () {
    'use strict';
    angular.module('app.portfolios').config([
        '$routeProvider',
        '$locationProvider',
        config
    ]);
    function config($routeProvider, $locationProvider) {
        $routeProvider.when('/portfolios/:id', {
            template: 'Portfolios',
            controller: function () {
            },
            controllerAs: 'vm'
        });
    }
})();
//# sourceMappingURL=portfolios.route.js.map