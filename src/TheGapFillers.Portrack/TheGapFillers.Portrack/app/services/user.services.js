var app;
(function (app) {
    var service;
    (function (service) {
        'use strict';
        var UserService = (function () {
            function UserService($http) {
                this.$http = $http;
            }
            UserService.prototype.getById = function (uniqueId) {
                return this.$http.get('/api/users/' + uniqueId).then(function (response) {
                    return response.data;
                });
            };
            UserService.$inject = ['$http'];
            return UserService;
        })();
        angular.module('app.services').service('app.services.UserService', UserService);
    })(service = app.service || (app.service = {}));
})(app || (app = {}));
//# sourceMappingURL=user.services.js.map