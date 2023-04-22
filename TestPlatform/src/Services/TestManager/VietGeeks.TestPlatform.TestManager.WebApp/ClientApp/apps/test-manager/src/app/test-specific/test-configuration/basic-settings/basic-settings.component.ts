import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Observable, lastValueFrom } from 'rxjs';
import { TestCategory, TestCategoryUncategorizedId } from '../../../_state/test-category.model';
import { TestCategoryQuery } from '../../../_state/test-category.query';
import { TestCategoryService } from '../../../_state/test-category.service';
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

  constructor(
    private _testCategoryQuery: TestCategoryQuery,
    private _testCategoryService: TestCategoryService,
    private _modalService: NgbModal) {
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
    const testCatetory = this._testCategoryQuery.getEntityWithFallback(this.test.basicSettings.category);
    if (!this.isNewTest) {
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
      this.router.navigate(['tests', createdTest.id, 'config', 'basic-settings']);
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
