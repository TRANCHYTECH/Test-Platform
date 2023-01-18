import { Component, HostBinding, TemplateRef } from '@angular/core';

import { ToastService } from './toast-service';

@Component({
  selector: 'viet-geeks-notifications',
  template: `
    <ngb-toast
      *ngFor="let toast of toastService.toasts"
      [class]="toast.classname"
      [autohide]="true"
      [delay]="toast.delay || 5000"
      (hidden)="toastService.remove(toast)">
      <ng-template [ngIf]="isTemplate(toast)" [ngIfElse]="text">
        <ng-template [ngTemplateOutlet]="toast.textOrTpl"></ng-template>
      </ng-template>

      <ng-template #text>{{ toast.textOrTpl }}</ng-template>
    </ngb-toast>
  `,
})
export class ToastsContainerComponent {
  constructor(public toastService: ToastService) { }   

  @HostBinding('class') class = 'toast-container position-fixed end-0 p-3';

  isTemplate(toast: { textOrTpl: any }) { return toast.textOrTpl instanceof TemplateRef; }
}
