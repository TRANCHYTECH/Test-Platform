import { Route } from '@angular/router';

export const appRoutes: Route[] = [
    {
        path: 'test',
        loadChildren: () => import('./proctor/proctor.module').then(m => m.ProctorModule)
    },
    {
        path: '**',
        redirectTo: 'test/access'
    }
];
