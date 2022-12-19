import { Route } from '@angular/router';

export const appRoutes: Route[] = [
    {
        path: '',
        loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule)
    },
    {
        path: 'tests',
        loadChildren: () => import('./test-configuration/test-configuration.module').then(m => m.TestConfigurationModule)
    }
];
