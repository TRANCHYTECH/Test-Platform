import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TestCategory } from '../../../state/test-category.model';
import { firstValueFrom, lastValueFrom, Observable } from 'rxjs';
import { TestsService } from '../../../state/tests.service';
import { TestCategoriesService } from '../../../state/test-categories.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TestCategoriesQuery } from '../../../state/test-categories.query';
import { TestsQuery } from '../../../state/tests.query';
import { ToastService } from '@viet-geeks/shared';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateTestCategoryComponent } from '../../../components/create-test-category/create-test-category.component';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-basic-settings',
  templateUrl: './basic-settings.component.html',
  styleUrls: ['./basic-settings.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BasicSettingsComponent implements OnInit {
  basicSettingForm: FormGroup;
  testCategories$!: Observable<TestCategory[]>;
  constructor(
    private _router: Router,
    private route: ActivatedRoute,
    private _fb: FormBuilder,
    private _testsService: TestsService,
    private _testCategoriesQuery: TestCategoriesQuery,
    private _testQuery: TestsQuery,
    private _testCategoriesService: TestCategoriesService,
    private _toastService: ToastService,
    private _modalService: NgbModal) {
    this.basicSettingForm = this._fb.group({
      id: null,
      name: '',
      category: '',
      description: ''
    });
  }

  ngOnInit() {
    firstValueFrom(this._testCategoriesService.get());
    this.testCategories$ = this._testCategoriesQuery.selectAll();
    this.route.params.pipe(untilDestroyed(this)).subscribe(async p => {
      const testId = p['id'];
      if (testId !== 'new') {
        await firstValueFrom(this._testsService.getById(testId), { defaultValue: null });
        const test = this._testQuery.getEntity(testId);
        this.basicSettingForm.reset({
          id: test?.id,
          name: test?.basicSettings.name,
          category: test?.basicSettings.category,
          description: test?.basicSettings.description
        });
      }
    });
  }

  async submit() {
    if (this.basicSettingForm.invalid) {
      return;
    }

    const formValue = this.basicSettingForm.value;
    const basicSettings = { name: formValue.name, category: formValue.category, description: formValue.description };
    if (formValue.id === null) {
      const createdTest = await lastValueFrom(this._testsService.add({ basicSettings: basicSettings }));
      this._router.navigate(['tests', createdTest.id, 'basic-settings']);
      this._toastService.show('Test created');
    } else {
      await this._testsService.update(formValue.id, { basicSettings: basicSettings });
      this._toastService.show('Test basic settings updated');
    }
  }

  openAddTestCategoryModal() {
    const modalRef = this._modalService.open(CreateTestCategoryComponent, { size: 'md', centered: true });
    modalRef.result.then(async (formValue: Partial<TestCategory>) => {
      await firstValueFrom(this._testCategoriesService.add(formValue));
    }, reason => {
      console.log(reason);
    })
  }
}
