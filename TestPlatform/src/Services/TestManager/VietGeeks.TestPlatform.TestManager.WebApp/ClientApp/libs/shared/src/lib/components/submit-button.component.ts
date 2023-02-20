import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
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
    isProcessing$ = new Subject<boolean>();
    @Input()
    class!: string;

    @Input()
    text = 'Save';

    @Input()
    processingText = 'Saving...';

    @Input()
    isDisabled?: boolean;

    @Input()
    submitFunc!: () => Promise<void>;

    @Input()
    canSubmit?: boolean;

    async onClick() {
        this.isProcessing$.next(true);
        await this.submitFunc();
        this.isProcessing$.next(false);
    }
}