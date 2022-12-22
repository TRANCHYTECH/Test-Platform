import { ApiRouteDefinition } from '@auth0/auth0-angular';
import {IAppSettings} from '@viet-geeks/core';

export class AppSettings implements IAppSettings {
    auth!: {
        scope: string,
        audience: string,
        domain: string,
        clientId: string,
        intercepters: ApiRouteDefinition[]
    };

    appTitle?: string;
    testManagerApiBaseUrl!: string;
}