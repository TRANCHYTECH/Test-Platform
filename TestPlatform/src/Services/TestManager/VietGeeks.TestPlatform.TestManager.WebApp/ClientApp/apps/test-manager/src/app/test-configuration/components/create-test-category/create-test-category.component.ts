import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { errorTailorImports } from '@ngneat/error-tailor';

@Component({
    selector: 'viet-geeks-create-category',
    standalone: true,
    imports: [ReactiveFormsModule, errorTailorImports],
    template: `
		<div class="modal-header">
			<h4 class="modal-title">Add Category</h4>
			<button type="button" class="btn-close" aria-label="Close" (click)="cancel()"></button>
		</div>
		<div class="modal-body">
			<div [formGroup]="form">
                <div class="mb-3">
                <label for="name-field" class="form-label">Category Name</label>
                <input type="text" id="name-field" class="form-control" placeholder="Enter Name" required
                formControlName="name" />
                </div>
            </div>
		</div>
		<div class="modal-footer">
			<button type="button" class="btn btn-primary w-sm" (click)="save()" [disabled]="form.invalid">Save</button>
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
