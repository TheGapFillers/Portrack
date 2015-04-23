// Type definitions for metisMenu
// Project: https://github.com/onokumus/metisMenu

/// <reference path="../jquery/jquery.d.ts" />
declare module jquery.metisMenu {
    interface metisMenu {
        (): void;
    }
}

interface JQuery {
    metisMenu: jquery.metisMenu.metisMenu;
}