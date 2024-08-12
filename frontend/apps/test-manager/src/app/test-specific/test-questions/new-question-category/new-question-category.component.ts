import { Component, Input, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { errorTailorImports } from '@ngneat/error-tailor';
import { SharedModule, ToastService } from '@viet-geeks/shared';
import { firstValueFrom } from 'rxjs';
import { QuestionCategoriesService } from '../../_state/question-categories/question-categories.service';

@Component({
  selector: 'viet-geeks-new-question-category',
  standalone: true, // Important to use it as standardalone, so could share between modules efficently.
  imports: [ReactiveFormsModule, SharedModule, errorTailorImports],
  templateUrl: './new-question-category.component.html',
  styleUrls: ['./new-question-category.component.scss']
})
export class NewQuestionCategoryComponent implements OnInit {
  @Input()
  testId!: string;

  form!: FormGroup;

  private _fb = inject(FormBuilder);
  private _activeModal = inject(NgbActiveModal);
  private _questionCategoryService = inject(QuestionCategoriesService);
  private _notifyService = inject(ToastService);

  get canSubmit() {
    return this.form.valid;
  }

  ngOnInit(): void {
    this.form = this._fb.nonNullable.group({
      name: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(50)]]
    });
  }

  async submit() {
    await firstValueFrom(this._questionCategoryService.add(this.testId, this.form.value));
  }

  submitFunc = async () => {
    if (!this.canSubmit) {
      return;
    }

    try {
      await this.submit();
      this._notifyService.success('Created new question category');
      this._activeModal.close();
    } catch (error) {
      this._notifyService.error('Failed to create new question category');
    }
  };

  cancel() {
    this._activeModal.dismiss('cancel');
  }
}
