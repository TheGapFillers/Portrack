/// <reference path="../../typings/metismenu-manually added/jquery.metis.menu.d.ts" />
'use strict';
module App {

    interface IApplicationController {
    }

    class ApplicationController implements IApplicationController {
        static controllerId = "ApplicationController";

        static $inject = [];
        constructor() {
        }
    }

    app.controller(ApplicationController.controllerId, ApplicationController);
} 