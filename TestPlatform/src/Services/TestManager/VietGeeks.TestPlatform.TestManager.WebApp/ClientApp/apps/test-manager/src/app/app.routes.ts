import { Route } from '@angular/router';

export const appRoutes: Route[] = [
    {
        path: 'dashboard',
        loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule)
    },
    {
        path: 'test/categories',
        loadChildren: () => import('./test-categories/test-categories.module').then(m => m.TetsCategoriesModule)
    },
    {
        path: 'test/list',
        loadChildren: () => import('./test-list/test-list.module').then(m => m.TestListModule)
    },
    {
        path: 'test/:id',
        loadChildren: () => import('./test-specific/test-specific.module').then(m => m.TestSpecificModule)
    },
    {
        path: 'account',
        loadChildren: () => import('./account/account.module').then(m => m.AccountModule)
    }
];