import { Component, Input } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
    selector: 'viet-geeks-create-test-category',
    standalone: true,
    imports: [ReactiveFormsModule],
    template: `
		<div class="modal-header">
			<h4 class="modal-title">Add Test Category</h4>
			<button type="button" class="btn-close" aria-label="Close" (click)="cancel()"></button>
		</div>
		<div class="modal-body">
			<div [formGroup]="form">
                <div class="mb-3">
                <label for="name-field" class="form-label">Test Category Name</label>
                <input type="text" id="name-field" class="form-control" placeholder="Enter Name" required
                formControlName="name" />
                </div>
            </div>
		</div>
		<div class="modal-footer">
			<button type="button" class="btn btn-outline-dark" (click)="save()">Save</button>
		</div>
	`,
})
export class CreateTestCategoryComponent {
    @Input()
    name?: string;

    form: FormGroup;

    constructor(private fb: FormBuilder, private activeModal: NgbActiveModal) {
        this.form = this.fb.group({
            name: ''
        })
    }

    save() {
        if(this.form.invalid) {
            return;
        }

        this.activeModal.close(this.form.value);
    }

    cancel() {
        this.activeModal.dismiss('cancel');
    }
}