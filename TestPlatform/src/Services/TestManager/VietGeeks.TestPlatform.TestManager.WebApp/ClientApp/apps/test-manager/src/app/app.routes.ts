import { Route } from '@angular/router';

export const appRoutes: Route[] = [
    {
        path: '',
        loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule)
    },
    {
        path: 'test-categories',
        loadChildren: () => import('./test-categories/test-categories.module').then(m => m.TetsCategoriesModule)
    },
    {
        path: 'tests',
        loadChildren: () => import('./test-list/test-list.module').then(m => m.TestListModule)
    },
    {
        path: 'tests/:id',
        loadChildren: () => import('./test-specific/test-specific.module').then(m => m.TestSpecificModule)
    },
    {
        path: 'account',
        loadChildren: () => import('./account/account.module').then(m => m.AccountModule)
    }
];