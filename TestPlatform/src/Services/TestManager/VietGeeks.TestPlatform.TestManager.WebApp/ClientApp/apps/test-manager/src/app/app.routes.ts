import { Route } from '@angular/router';
import { LayoutComponent } from './shell/layout/layout.component';

export const appRoutes: Route[] = [
    {
        path: '',
        loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule)
    }
];
