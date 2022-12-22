import { MenuItem } from './menu.model';

export const MENU: MenuItem[] = [
  {
    id: 1,
    label: 'MENUITEMS.MENU.TEXT',
    isTitle: true
  },
  {
    id: 2,
    label: 'MENUITEMS.DASHBOARD.TEXT',
    icon: 'home',
    subItems: [
      {
        id: 3,
        label: 'MENUITEMS.DASHBOARD.LIST.tests',
        link: '/tests',
        parentId: 2
      }
    ]
  },
  {
    id: 131,
    label: 'MENUITEMS.account.text',
    icon: 'user',
    link: '/account'
  }

];
