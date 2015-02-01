module app.service {
    'use strict';

    export interface IUserService {
        getById(uniqueId: string)
    }

    export interface IUser {
        uniqueId: string;
        email: string;
        firstName: string;
        lastName: string;
    }

    class UserService implements IUserService {

        static $inject = ['$http'];
        constructor(private $http: ng.IHttpService) {
        }

        getById(uniqueId: string): ng.IPromise<IUser> {
            return this.$http.get('/api/users/' + uniqueId)
                .then((response: ng.IHttpPromiseCallbackArg<IUser>): IUser => {
                return response.data;
            });
        }
    }

    angular
        .module('app.services')
        .service('app.services.UserService', UserService);
}
 