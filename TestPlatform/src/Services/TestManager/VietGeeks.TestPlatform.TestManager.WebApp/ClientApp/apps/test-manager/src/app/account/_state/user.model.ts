export interface User {
  id: string;
  email: string;
  regionalSettings?: RegionalSettings
}

export interface RegionalSettings {
  language?: string;
  timeZone?: string;
}