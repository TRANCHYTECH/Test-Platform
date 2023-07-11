import { ChangeDetectionStrategy, Component, Input, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';

@Component({
  selector: 'viet-geeks-submit-button',
  template: `
  <button type="buttonType" [ngClass]="class" (click)="onClick()" [disabled]="!canSubmit || (isProcessing$ | async)">
    {{(isProcessing$ | async) ? processingText: text}}
  </button>`,
  styles: [``],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SubmitButtonComponent {
  translateService = inject(TranslateService);

  isProcessing$ = new Subject<boolean>();
  @Input()
  class!: string;

  @Input()
  text = this.translateService.instant('labels.save');

  @Input()
  processingText =  this.translateService.instant('labels.saving');

  @Input()
  isDisabled?: boolean;

  @Input()
  submitFunc!: () => Promise<void>;

  @Input()
  canSubmit?: boolean;

  onClick() {
    this.isProcessing$.next(true);
    this.submitFunc().finally(() => this.isProcessing$.next(false));
  }
}
