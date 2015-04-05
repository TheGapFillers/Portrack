((): void => {
    'use strict';

    angular
        .module('app', [
            'app.core',
            'app.layout',
            'app.services',
            'app.widgets',
            'app.blocks',
            /*
             * Features areas
             */
            'app.portfolios',
            'app.instruments'
        ]);
})(); 
