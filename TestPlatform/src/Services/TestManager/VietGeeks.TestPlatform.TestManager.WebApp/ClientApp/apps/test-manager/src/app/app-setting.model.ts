import {IAppSettings} from '@viet-geeks/core';

export class AppSettings implements IAppSettings {
    auth!: {
        domain: string,
        clientId: string
    };

    appTitle?: string;
}