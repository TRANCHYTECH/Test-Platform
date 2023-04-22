import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UntilDestroy } from '@ngneat/until-destroy';
import { TestCategory, TestCategoryQuery, TestCategoryService, TestCategoryUncategorizedId } from '@viet-geeks/test-manager/state';
import { Observable, lastValueFrom } from 'rxjs';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { CreateCategoryComponent } from '../_components/create-test-category/create-test-category.component';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-basic-settings',
  templateUrl: './basic-settings.component.html',
  styleUrls: ['./basic-settings.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BasicSettingsComponent extends TestSpecificBaseComponent {
  basicSettingForm: FormGroup;
  testCategories$!: Observable<TestCategory[]>;

  private _testCategoryQuery = inject(TestCategoryQuery);
  private _testCategoryService = inject(TestCategoryService);
  private _modalService = inject(NgbModal);

  constructor() {
    super();
    this.basicSettingForm = this.fb.group({
      id: null,
      name: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],
      category: [TestCategoryUncategorizedId, [Validators.required]],
      description: ''
    });
  }

  async postLoadEntity(): Promise<void> {
    await Promise.all([this._testCategoryService.get()]);
    if (this.isNewTest) {
      this._refreshAfterSubmit = false;
    }
    else {
      this._refreshAfterSubmit = true;
      const testCatetory = this._testCategoryQuery.getEntityWithFallback(this.test.basicSettings.category);
      this.basicSettingForm.reset({
        id: this.test.id,
        name: this.test.basicSettings.name,
        category: testCatetory?.id,
        description: this.test.basicSettings.description
      });
    }
  }

  override onInit() {
    this.testCategories$ = this._testCategoryQuery.selectAll();
  }

  get canSubmit(): boolean {
    return this.basicSettingForm.dirty && this.basicSettingForm.valid;
  }

  async submit() {
    const formValue = this.basicSettingForm.value;
    const basicSettings = { name: formValue.name, category: formValue.category, description: formValue.description };
    if (formValue.id === null) {
      const createdTest = await lastValueFrom(this.testsService.add({ basicSettings: basicSettings }));
      this.router.navigate(['test', createdTest.id, 'config', 'basic-settings']);
      this.notifyService.success('Test created');
    } else {
      await this.testsService.update(formValue.id, { basicSettings: basicSettings });
      this.notifyService.success('Test basic settings updated');
    }
  }

  openAddTestCategoryModal() {
    const modalRef = this._modalService.open(CreateCategoryComponent, { size: 'md', centered: true });
    modalRef.result.then(async (formValue: Partial<TestCategory>) => {
      await this._testCategoryService.add(formValue);
      this.notifyService.success('Test Category created');
    });
  }
}
