import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TestCategory } from '../../../state/test-category.model';
import { filter, firstValueFrom, lastValueFrom, Observable, take } from 'rxjs';
import { TestsService } from '../../../state/tests.service';
import { TestCategoriesService } from '../../../state/test-categories.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TestCategoriesQuery } from '../../../state/test-categories.query';
import { TestsQuery } from '../../../state/tests.query';
import { ToastService } from '@viet-geeks/shared';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

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
    private _toastService: ToastService) {
    this.basicSettingForm = this._fb.group({
      id: null,
      name: '',
      category: '',
      description: ''
    });
  }

  ngOnInit() {
    lastValueFrom(this._testCategoriesService.get());
    this.testCategories$ = this._testCategoriesQuery.selectAll();

    this.route.params.subscribe(p => {
      const testId = p['id'];
      if (testId !== 'new') {
        firstValueFrom(this._testsService.getById(testId));
        this._testQuery.selectEntity(testId).pipe(filter(test => test !== undefined), untilDestroyed(this)).subscribe(test => {
          this.basicSettingForm.reset({
            id: test?.id,
            name: test?.basicSettings.name,
            category: test?.basicSettings.category,
            description: test?.basicSettings.description
          });
        });
      }
    });
  }

  async submit() {
    if(this.basicSettingForm.invalid) {
      return;
    }

    const formValue = this.basicSettingForm.value;
    const basicSettings = { name: formValue.name, category: formValue.category, description: formValue.description };
    if (formValue.id === null) {
      const createdTest = await lastValueFrom(this._testsService.add({ basicSettings: basicSettings }));
      this._router.navigate(['tests', createdTest.id, 'basic-settings']);
      this._toastService.show('Test created', { classname: 'bg-success text-light' });
    } else {
      await firstValueFrom(this._testsService.update(formValue.id, { basicSettings: basicSettings }));
      this._toastService.show('Test basic settings updated', { classname: 'bg-success text-light' });
    }
  }

  addCategory() {
    this._testCategoriesService.add({ id: '123', name: 'React Expert' });
    this._testCategoriesService.add({ id: '124', name: 'Angular Expert' });
  }
}
