/// <reference path="common.ts" />
'use strict';
var App;
(function (App) {
    var Shared;
    (function (Shared) {
        var Logger = (function () {
            //#endregion
            function Logger($log) {
                this.service = {
                    getLogFn: this.getLogFn,
                    log: this.log,
                    logError: this.logError,
                    logSuccess: this.logSuccess,
                    logWarning: this.logWarning
                };
                this.$log = $log;
            }
            //#region Public Methods
            //TODO: see if there is a way to solve this more intuitive than returning an anonymous function
            Logger.prototype.getLogFn = function (moduleId, logFunctionName) {
                var _this = this;
                logFunctionName = logFunctionName || 'log';
                switch (logFunctionName.toLowerCase()) {
                    case 'success':
                        logFunctionName = 'logSuccess';
                        break;
                    case 'error':
                        logFunctionName = 'logError';
                        break;
                    case 'warn':
                        logFunctionName = 'logWarning';
                        break;
                    case 'warning':
                        logFunctionName = 'logWarning';
                        break;
                }
                return function (msg, data, showToast) {
                    _this.logFn = _this.service[logFunctionName] || _this.service.log;
                    _this.logFn(msg, data, moduleId, (showToast === undefined) ? true : showToast);
                };
            };
            Logger.prototype.log = function (message, data, source, showToast) {
                this.logIt(message, data, source, showToast, 'info');
            };
            Logger.prototype.logWarning = function (message, data, source, showToast) {
                this.logIt(message, data, source, showToast, 'warning');
            };
            Logger.prototype.logSuccess = function (message, data, source, showToast) {
                this.logIt(message, data, source, showToast, 'success');
            };
            Logger.prototype.logError = function (message, data, source, showToast) {
                this.logIt(message, data, source, showToast, 'error');
            };
            //#endregion
            Logger.prototype.logIt = function (message, data, source, showToast, toastType) {
                var write = (toastType === 'error') ? this.$log.error : this.$log.log;
                source = source ? '[' + source + '] ' : '';
                write(source, message, data);
                if (showToast) {
                    if (toastType === 'error') {
                        toastr.error(message);
                    }
                    else if (toastType === 'warning') {
                        toastr.warning(message);
                    }
                    else if (toastType === 'success') {
                        toastr.success(message);
                    }
                    else {
                        toastr.info(message);
                    }
                }
            };
            Logger.serviceId = 'logger';
            return Logger;
        })();
        Shared.Logger = Logger;
        // Register with angular
        Shared.commonModule.factory(Logger.serviceId, ['$log', function ($log) { return new Logger($log); }]);
    })(Shared = App.Shared || (App.Shared = {}));
})(App || (App = {}));
//# sourceMappingURL=logger.js.map