import { CoreConfig } from '@viet-geeks/core';

export interface PortalConfig extends CoreConfig {
  production: boolean;
  testManagerApiBaseUrl: string;
  accountManagerApiBaseUrl: string;
  testRunnerBaseUrl: string;
  editorApiKey: string;
  uploadPubicKey: string;
}
