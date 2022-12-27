import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TestCategory } from '../../../state/test-category.model';
import { Observable, take } from 'rxjs';
import { TestCategoriesQuery } from '../../../state/test-categories.query';
import { TestsService } from '../../../state/tests.service';
import { TestCategoriesService } from '../../../state/test-categories.service';
import { ActivatedRoute } from '@angular/router';
import { TestsQuery } from '../../../state/tests.query';

@Component({
  selector: 'viet-geeks-basic-settings',
  templateUrl: './basic-settings.component.html',
  styleUrls: ['./basic-settings.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BasicSettingsComponent implements OnInit {
  basicSettingForm: FormGroup;
  testCategories$!: Observable<TestCategory[]>;
  constructor(private route: ActivatedRoute, private _testsQuery: TestsQuery, private _fb: FormBuilder, private _testsService: TestsService, private _testCategoriesService: TestCategoriesService, private _testCategoriesQuery: TestCategoriesQuery) {
    this.basicSettingForm = this._fb.group({
      name: '',
      category: '',
      description: ''
    });
  }

  ngOnInit() {
    const testId = this.route.snapshot.params['id'];
    if (testId !== 'new') {
        this.basicSettingForm = this._fb.group({
          name: 'abc',
          category: '124',
          description: 'description'
      });
    }

    this.testCategories$ = this._testCategoriesQuery.selectAll();
  }

  submit() {
    console.log('submited basic setting form', this.basicSettingForm.value);
  }

  addCategory() {
    this._testCategoriesService.add({ id: '123', text: 'React Expert' });
    this._testCategoriesService.add({ id: '124', text: 'Angular Expert' });
  }
}
