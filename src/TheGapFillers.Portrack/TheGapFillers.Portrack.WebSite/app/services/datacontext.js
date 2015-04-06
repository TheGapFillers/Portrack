/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
var App;
(function (App) {
    var Services;
    (function (Services) {
        var Datacontext = (function () {
            function Datacontext(common) {
                this.common = common;
                this.$q = common.$q;
            }
            Datacontext.prototype.getMessageCount = function () {
                return this.$q.when(72);
            };
            Datacontext.prototype.getPeople = function () {
                var people = [
                    { firstName: 'John', lastName: 'Papa', age: 25, location: 'Florida' },
                    { firstName: 'Ward', lastName: 'Bell', age: 31, location: 'California' },
                    { firstName: 'Colleen', lastName: 'Jones', age: 21, location: 'New York' },
                    { firstName: 'Madelyn', lastName: 'Green', age: 18, location: 'North Dakota' },
                    { firstName: 'Ella', lastName: 'Jobs', age: 18, location: 'South Dakota' },
                    { firstName: 'Landon', lastName: 'Gates', age: 11, location: 'South Carolina' },
                    { firstName: 'Haley', lastName: 'Guthrie', age: 35, location: 'Wyoming' }
                ];
                return this.$q.when(people);
            };
            Datacontext.serviceId = 'datacontext';
            return Datacontext;
        })();
        Services.Datacontext = Datacontext;
        // Register with angular
        App.app.factory(Datacontext.serviceId, ['common', function (common) { return new Datacontext(common); }]);
    })(Services = App.Services || (App.Services = {}));
})(App || (App = {}));
//# sourceMappingURL=datacontext.js.map