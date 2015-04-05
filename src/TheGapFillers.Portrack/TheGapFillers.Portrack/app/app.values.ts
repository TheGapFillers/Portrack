interface ICurrentUser {
    userId: string;
    fullName: string;
}

((): void => {
    'use strict';

    var currentUser: ICurrentUser = {
        userId: '',
        fullName: ''
    };

    angular
        .module('app')
        .value('currentUser', currentUser);
})(); 