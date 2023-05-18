import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { DeactivatableComponent } from '@viet-geeks/shared';
import { find, forEach } from 'lodash-es';
import { firstValueFrom } from 'rxjs';
import { QuestionSummary } from '../../../../../../../libs/shared/src/lib/models/question.model';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { QuestionCategory } from '../../_state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../_state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../_state/question-categories/question-categories.service';
import { QuestionService } from '../../_state/questions/question.service';
import { GeneratorTypes, TestSets } from '../../_state/tests/test.model';

@Component({
  selector: 'viet-geeks-test-sets',
  templateUrl: './test-sets.component.html',
  styleUrls: ['./test-sets.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestSetsComponent extends TestSpecificBaseComponent implements DeactivatableComponent {
  private _questionService = inject(QuestionService);
  private _questionCategoriesService = inject(QuestionCategoriesService);
  private _questionCategoriesQuery = inject(QuestionCategoriesQuery);

  testSetsForm!: FormGroup;

  questionCategories: QuestionCategory[] = [];
  questionSummaries: QuestionSummary[] = [];

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

  canDeactivate: () => boolean | Promise<boolean> = () => !this.testSetsForm.dirty;

  async postLoadEntity(): Promise<void> {
    const configs = await Promise.all([this._questionService.getSummary(this.testId), firstValueFrom(this._questionCategoriesService.get(this.testId))]);
    this.questionSummaries = configs[0];
    this.questionCategories = this._questionCategoriesQuery.getAll();

    this.testSetsForm = this.fb.group({
      generatorType: this.test?.testSetSettings?.generatorType || GeneratorTypes.Default,
      generator: this.fb.group({
        $type: GeneratorTypes.RandomFromCategories,
        configs: this.createGeneratorConfigsForm()
      })
    });
  }

  get canSubmit(): boolean {
    return this.testSetsForm.dirty && this.testSetsForm.valid;
  }

  async submit() {
    const model = <TestSets>this.testSetsForm.value;
    if (model.generatorType === GeneratorTypes.Default) {
      model.generator = { $type: model.generatorType, configs: undefined };
    }
    await this.testsService.update(this.testId, { testSetSettings: model });
    this.notifyService.success('Test sets updated');
  }

  private createGeneratorConfigsForm() {
    const generatorConfigsForm = this.fb.array<FormGroup>([]);
    forEach(this.questionSummaries, (value) => {
      const existingDrawValue = find(this.test.testSetSettings?.generator?.configs, { id: value.categoryId });
      generatorConfigsForm.push(this.fb.group({
        id: value.categoryId,
        draw: this.fb.control(existingDrawValue?.draw || value.numberOfQuestions, [Validators.required, Validators.min(1), Validators.max(value.numberOfQuestions)]),
        name: this.fb.control({ value: this.questionCategories.find(c => c.id === value.categoryId)?.name, disabled: true }),
        totalQuestions: this.fb.control({ value: value.numberOfQuestions, disabled: true })
      }));
    });

    return generatorConfigsForm;
  }
}
