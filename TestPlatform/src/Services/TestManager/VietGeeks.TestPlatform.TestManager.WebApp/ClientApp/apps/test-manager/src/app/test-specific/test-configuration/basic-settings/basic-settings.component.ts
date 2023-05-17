import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { TestCategory, TestCategoryUncategorizedId } from '../../../_state/test-category.model';
import { TestCategoryQuery } from '../../../_state/test-category.query';
import { TestCategoryService } from '../../../_state/test-category.service';
import { UiIntegrationService } from '../../../_state/ui-integration.service';
import { Observable, lastValueFrom } from 'rxjs';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';

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
  private _uiIntegrationService = inject(UiIntegrationService);

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
    if (!this.isNewTest) {
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
      await this.router.navigate(['test', createdTest.id, 'config', 'basic-settings']);
      this.notifyService.success('Test created');
    } else {
      await this.testsService.update(formValue.id, { basicSettings: basicSettings });
      this.notifyService.success('Test basic settings updated');
    }
  }

  openAddTestCategoryModal() {
    this._uiIntegrationService.openNewTestCategoryModal();
  }
}
