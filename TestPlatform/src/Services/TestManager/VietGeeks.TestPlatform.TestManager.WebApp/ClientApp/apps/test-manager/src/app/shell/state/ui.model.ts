export type RouteInfo = {
  path: string;
  title: string;
  icon: string;
  class: string;
}

export interface Ui {
  id: string;
  routes: RouteInfo[];
}

export function createUi(params: Partial<Ui>) {
  return {

  } as Ui;
}
