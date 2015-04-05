(function () {
    'use strict';
    angular.module('app').config(['$locationProvider', config]);
    function config($locationProvider) {
        $locationProvider.html5Mode(true);
    }
})();
//# sourceMappingURL=app.config.js.map