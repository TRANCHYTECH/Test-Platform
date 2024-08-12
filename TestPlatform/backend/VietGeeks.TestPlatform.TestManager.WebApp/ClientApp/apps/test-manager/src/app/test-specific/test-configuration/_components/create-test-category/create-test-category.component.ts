import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { errorTailorImports } from '@ngneat/error-tailor';
import { TranslateModule } from "@ngx-translate/core";

@Component({
	selector: 'viet-geeks-create-category',
	standalone: true,
	imports: [ReactiveFormsModule, errorTailorImports, TranslateModule],
	template: `
		<div class="modal-header">
			<h4 class="modal-title">{{'labels.addTestCategory'}}</h4>
			<button type="button" class="btn-close" aria-label="Close" (click)="cancel()"></button>
		</div>
		<div class="modal-body">
			<div [formGroup]="form">
                <div class="mb-3">
                <label for="name-field" class="form-label">{{'labels.testCategoryName' | translate }}</label>
                <input type="text" id="name-field" class="form-control" placeholder="{{'labels.testCategoryNameHint' | translate }}" required
                formControlName="name" />
                </div>
            </div>
		</div>
		<div class="modal-footer">
			<button type="button" class="btn btn-primary w-sm" (click)="save()" [disabled]="form.invalid">{{ 'labels.save' | translate }}</button>
		</div>
	`,
})
export class CreateCategoryComponent {
	@Input()
	name?: string;

	form: FormGroup;

	constructor(private fb: FormBuilder, private activeModal: NgbActiveModal) {
		this.form = this.fb.group({
			name: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(50)]]
		})
	}

	save() {
		if (this.form.invalid) {
			return
		}

		this.activeModal.close(this.form.value);
	}

	cancel() {
		this.activeModal.dismiss('cancel');
	}
}
