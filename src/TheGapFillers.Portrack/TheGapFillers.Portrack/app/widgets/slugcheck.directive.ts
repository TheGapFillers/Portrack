((): void => {
    'use strict';

    angular
        .module('app.widgets')
        .directive('blSlugCheck', slugCheck);

    function slugCheck(): ng.IDirective {
        var directive = <ng.IDirective> {
            restrict: 'A',
            link: link
        };

        function link(scope: ng.IScope, element: ng.IAugmentedJQuery): void {
            element.on('blur',(): void => {

            });
        }

        return directive;
    }
})(); 