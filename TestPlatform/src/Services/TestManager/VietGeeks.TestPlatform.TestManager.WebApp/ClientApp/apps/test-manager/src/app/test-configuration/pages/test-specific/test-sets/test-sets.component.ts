import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormArray, FormGroup, Validators, FormControl } from '@angular/forms';
import { find, forEach } from 'lodash';
import { TestSets } from '../../../state/test.model';
import { UntilDestroy } from '@ngneat/until-destroy';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';

export const GeneratorTypes = {
  Default: 1,
  RandomFromCategories: 2
};

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-sets',
  templateUrl: './test-sets.component.html',
  styleUrls: ['./test-sets.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSetsComponent extends TestSpecificBaseComponent {

  testSetsForm: FormGroup;

  //todo: get from question manager
  questionCategories: { id: string, name: string, totalQuestions: number }[] = [
    {
      id: '2345',
      name: 'Problem solving',
      totalQuestions: 5
    },
    {
      id: '2346',
      name: 'Indepentent',
      totalQuestions: 7
    }
  ]

  get isRandomByCategorySelected() {
    return this.generatorType?.value === GeneratorTypes.RandomFromCategories;
  }

  get generatorType() {
    return this.testSetsForm.get('generatorType') as FormControl<number>;
  }

  get randomByCategoriesGenerator() {
    return this.testSetsForm.controls['generator'] as FormGroup;
  }

  get randomByCategoriesGeneratorConfigs() {
    return this.randomByCategoriesGenerator.controls['configs'] as FormArray<FormGroup>;
  }

  set randomByCategoriesGeneratorConfigs(value) {
    this.randomByCategoriesGenerator.setControl('configs', value);
  }

  readonly generatorTypeOptions = [
    { id: GeneratorTypes.Default, textKey: 'Pages.TestSets.GeneratorTypes.Default' },
    { id: GeneratorTypes.RandomFromCategories, textKey: 'Pages.TestSets.GeneratorTypes.RandomFromCategories' }
  ]

  constructor() {
    super();
    this.testSetsForm = this.fb.group({});
  }

  onInit() {
    // 
  }

  afterGetTest(): void {
    this.testSetsForm = this.fb.group({
      generatorType: this.test?.testSetSettings?.generatorType || GeneratorTypes.Default,
      generator: this.fb.group({
        $type: GeneratorTypes.RandomFromCategories,
        configs: this.createGeneratorConfigsForm()
      })
    });

    this.maskReadyForUI();
  }

  async saveTestSets() {
    if (this.testSetsForm.invalid)
      return;

    const model = <TestSets>this.testSetsForm.value;
    if (model.generatorType === GeneratorTypes.Default) {
      model.generator = undefined;
    }
    await this.testsService.update(this.testId, { testSetSettings: model });
    this.notifyService.show('Test sets updated');
  }

  private createGeneratorConfigsForm() {
    const generatorConfigsForm = this.fb.array<FormGroup>([]);
    forEach(this.questionCategories, (value) => {
      const existingDrawValue = find(this.test.testSetSettings?.generator?.configs, { id: value.id });
      generatorConfigsForm.push(this.fb.group({
        id: value.id,
        draw: this.fb.control(existingDrawValue?.draw || value.totalQuestions, [Validators.min(0), Validators.max(value.totalQuestions)]),
        name: this.fb.control({ value: value.name, disabled: true }),
        totalQuestions: this.fb.control({ value: value.totalQuestions, disabled: true })
      }));
    });

    return generatorConfigsForm;
  }
}
