import { IAppSettings } from '@viet-geeks/core';

export class AppSettings implements IAppSettings {
    appTitle?: string;
    testRunnerApiBaseUrl!: string;
}