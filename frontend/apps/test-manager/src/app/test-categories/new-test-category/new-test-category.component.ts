import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { errorTailorImports } from '@ngneat/error-tailor';
import { SharedModule, ToastService } from '@viet-geeks/shared';
import { TestCategoryService } from '../../_state/test-category.service';

@Component({
  selector: 'viet-geeks-new-test-category',
  standalone: true,
  imports: [ReactiveFormsModule, SharedModule, errorTailorImports],
  templateUrl: './new-test-category.component.html',
  styleUrls: ['./new-test-category.component.scss'],
})
export class NewTestCategoryComponent implements OnInit {
  form!: FormGroup;

  private _fb = inject(FormBuilder);
  private _activeModal = inject(NgbActiveModal);
  private _testCategoryService = inject(TestCategoryService);
  private _notifyService = inject(ToastService);

  get canSubmit() {
    return this.form.valid;
  }

  ngOnInit(): void {
    this.form = this._fb.nonNullable.group({
      name: [
        '',
        [
          Validators.required,
          Validators.minLength(5),
          Validators.maxLength(50),
        ],
      ],
    });
  }

  async submit() {
    await this._testCategoryService.add(this.form.value);
  }

  submitFunc = async () => {
    if (!this.canSubmit) {
      return;
    }

    try {
      await this.submit();
      this._notifyService.success('Created new test category');
      this._activeModal.close();
    } catch (error) {
      this._notifyService.error('Failed to create new test category');
    }
  };

  cancel() {
    this._activeModal.dismiss('cancel');
  }
}
