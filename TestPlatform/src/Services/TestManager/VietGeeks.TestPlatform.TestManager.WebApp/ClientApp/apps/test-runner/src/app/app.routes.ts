import { Route } from '@angular/router';

export const appRoutes: Route[] = [
    {
        path: 'test',
        loadChildren: () => import('./proctor/proctor-routing.module').then(m => m.ProctorRoutingModule)
    },
    {
        path: '**',
        redirectTo: 'test/start'
    }
];
