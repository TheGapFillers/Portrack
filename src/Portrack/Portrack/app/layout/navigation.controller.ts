module app.layout {
    'use strict';

    interface INavigationScope {
        fullName: string;
    }

    class NavigationController implements INavigationScope {
        fullName: string;

        static $inject = [
            'currentUser',
            'app.services.UserService'
        ];
        constructor(currentUser: ICurrentUser, userService: app.service.IUserService) {
            var vm = this;
            userService.getById(currentUser.userId)
                .then((user: app.service.IUser): void => {
                vm.fullName = user.firstName + ' ' + user.lastName;
            });
        }
    }

    angular
        .module('app.layout')
        .controller('app.layoyt.NavigationController', NavigationController);
}