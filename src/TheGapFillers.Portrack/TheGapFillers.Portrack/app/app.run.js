(function () {
    'use strict';
    angular.module('app').run([
        '$rootScope',
        '$cookies',
        'currentUser',
        run
    ]);
    function run($rootScope, $cookies, currentUser) {
        $rootScope.$on('$routeChangeError', function () {
        });
        currentUser.userId = $cookies.userId;
    }
})();
//# sourceMappingURL=app.run.js.map