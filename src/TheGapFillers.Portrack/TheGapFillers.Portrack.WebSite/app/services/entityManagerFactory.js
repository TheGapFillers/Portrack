/// <reference path="../../scripts/typings/breeze/breeze.d.ts" />
'use strict';
var App;
(function (App) {
    var Services;
    (function (Services) {
        var EntityManagerFactory = (function () {
            function EntityManagerFactory(breeze, config) {
                this.breeze = breeze;
                this.config = config;
                this.setNamingConventionToCamelCase();
                this.preventValidateOnAttach();
                this.metadataStore = new breeze.MetadataStore();
                this.serviceName = config.remoteServiceName;
            }
            EntityManagerFactory.prototype.newManager = function () {
                var mgr = new breeze.EntityManager({
                    serviceName: this.serviceName,
                    metadataStore: this.metadataStore
                });
                return mgr;
            };
            EntityManagerFactory.prototype.setNamingConventionToCamelCase = function () {
                // Convert server - side PascalCase to client - side camelCase property names
                breeze.NamingConvention.camelCase.setAsDefault();
            };
            EntityManagerFactory.prototype.preventValidateOnAttach = function () {
                new breeze.ValidationOptions({ validateOnAttach: false }).setAsDefault();
            };
            EntityManagerFactory.serviceId = 'entityManagerFactory';
            return EntityManagerFactory;
        })();
        Services.EntityManagerFactory = EntityManagerFactory;
        App.app.factory(EntityManagerFactory.serviceId, ['breeze', 'config', function (b, c) { return new EntityManagerFactory(b, c); }]);
    })(Services = App.Services || (App.Services = {}));
})(App || (App = {}));
//# sourceMappingURL=entityManagerFactory.js.map