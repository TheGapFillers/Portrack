module app.blocks {
    'use strict';

    export interface IApiEndpointConfig {
        baseUrl: string;
    }

    export interface IApiEndpointProvider {
        configure(baseUrl: string): void;
    }

    class ApiEndpointProvider implements ng.IServiceProvider, IApiEndpointProvider {
        config: IApiEndpointConfig;

        configure(baseUrl: string): void {
            this.config = {
                baseUrl: baseUrl
            };
        }

        $get(): IApiEndpointConfig {
            return this.config;
        }
    }

    angular
        .module('app.blocks')
        .provider('app.blocks.ApiEndpoint', ApiEndpointProvider);
}