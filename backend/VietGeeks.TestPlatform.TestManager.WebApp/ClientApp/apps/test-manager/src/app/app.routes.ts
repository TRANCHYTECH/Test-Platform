import { Route } from '@angular/router';
import { AuthGuard } from '@auth0/auth0-angular';

export const appRoutes: Route[] = [
    {
        path: 'dashboard',
        loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule),
        canActivate: [AuthGuard]
    },
    {
        path: 'test/categories',
        loadChildren: () => import('./test-categories/test-categories.module').then(m => m.TetsCategoriesModule),
        canActivate: [AuthGuard]
    },
    {
        path: 'test/list',
        loadChildren: () => import('./test-list/test-list.module').then(m => m.TestListModule),
        canActivate: [AuthGuard]
    },
    {
        path: 'test/:id',
        loadChildren: () => import('./test-specific/test-specific.module').then(m => m.TestSpecificModule),
        canActivate: [AuthGuard]
    },
    {
        path: 'account',
        loadChildren: () => import('./account/account.module').then(m => m.AccountModule),
        canActivate: [AuthGuard]
    },
    {
        path: '',
        redirectTo: '/test/list',
        pathMatch: 'full'
    }
];