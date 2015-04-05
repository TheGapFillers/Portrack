((): void => {
    'use strict';

    angular
        .module('app.core', [
            /*
             * Angular Modules
             */
            'ngRoute',
            'ngSanitize',
            'ngCookies',
            /*
            * Angular Modules
            */
            'hc.marked'
        ]);
})();