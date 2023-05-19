import { Component, inject, OnInit } from "@angular/core";
import { ToastService } from "@viet-geeks/shared";
import { NgxSpinnerService } from "ngx-spinner";

@Component({ template: '' })
export abstract class TestRunnerBaseComponent {
  protected notifyService = inject(ToastService);
  protected spinner = inject(NgxSpinnerService);

  protected showLoadingIndicator() {
    this.spinner.show(undefined, {
      size: 'medium',
      bdColor: 'rgba(100,149,237, .2)',
      color: '#25a0e2',
      fullScreen: false
    });
  }

  protected hideLoadingIndicator() {
    this.spinner.hide();
  }

  async triggerWithLoadingIndicator(func: () => Promise<unknown>) {
    this.showLoadingIndicator();
    await func();
    this.hideLoadingIndicator();
   }
}
