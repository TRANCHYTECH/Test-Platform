import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { firstValueFrom, lastValueFrom, Observable } from 'rxjs';
import { UntilDestroy } from '@ngneat/until-destroy';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { TestCategory, TestCategoryUncategorizedId } from '../../_state/test-category.model';
import { TestCategoriesQuery } from '../../_state/test-categories.query';
import { TestCategoriesService } from '../../_state/test-categories.service';
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
    private _testCategoriesQuery: TestCategoriesQuery,
    private _testCategoriesService: TestCategoriesService,
    private _modalService: NgbModal) {
    super();
    this.basicSettingForm = this.fb.group({
      id: null,
      name: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],
      category: [TestCategoryUncategorizedId, [Validators.required]],
      description: ''
    });
  }

  afterGetTest(): void {
    if (!this.isNewTest) {
      this.basicSettingForm.reset({
        id: this.test.id,
        name: this.test.basicSettings.name,
        category: this.test.basicSettings.category,
        description: this.test.basicSettings.description
      });
    }

    this.maskReadyForUI();
  }

  override onInit() {
    firstValueFrom(this._testCategoriesService.get());
    this.testCategories$ = this._testCategoriesQuery.selectAll();
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
      await firstValueFrom(this._testCategoriesService.add(formValue));
      this.notifyService.success('Test Category created');
    });
  }
}
