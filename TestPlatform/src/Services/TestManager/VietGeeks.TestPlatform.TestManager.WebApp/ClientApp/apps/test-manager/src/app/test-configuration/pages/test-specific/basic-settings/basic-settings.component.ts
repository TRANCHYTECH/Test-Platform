import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { TestCategory } from '../../../state/test-category.model';
import { firstValueFrom, lastValueFrom, Observable } from 'rxjs';
import { TestCategoriesService } from '../../../state/test-categories.service';
import { TestCategoriesQuery } from '../../../state/test-categories.query';
import { UntilDestroy } from '@ngneat/until-destroy';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateCategoryComponent } from '../../../components/create-test-category/create-test-category.component';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';

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
      name: ['', [Validators.required]],
      category: ['', [Validators.required]],
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

  onInit() {
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
      this.router.navigate(['tests', createdTest.id, 'basic-settings']);
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
    }, reason => {
      console.log(reason);
    })
  }
}
