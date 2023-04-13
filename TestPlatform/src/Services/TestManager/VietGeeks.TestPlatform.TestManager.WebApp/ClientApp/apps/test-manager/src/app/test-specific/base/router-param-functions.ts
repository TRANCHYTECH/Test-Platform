import { ActivatedRoute, ActivatedRouteSnapshot, Params } from "@angular/router";

export const getTestId = (route: ActivatedRoute) => {
    return getParams(route)['id'];
}

export const getParams = (route: ActivatedRoute) => {
    let params: Params = {};
    const stack: ActivatedRouteSnapshot[] = [route.snapshot.root];
    while (stack.length > 0) {
        const route = stack.pop();
        if (route !== undefined) {
            params = { ...params, ...route.params };
            stack.push(...route.children);
        }
    }

    return params;
}