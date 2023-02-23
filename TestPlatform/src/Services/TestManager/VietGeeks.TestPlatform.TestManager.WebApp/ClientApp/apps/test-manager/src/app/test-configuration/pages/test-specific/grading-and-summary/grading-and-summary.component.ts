import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { find, findIndex, forIn } from 'lodash';
import { GradeRangeCriteria, GradeRangeCriteriaDetail, GradingSettings, TestEndConfig } from '../../../state/test.model';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';
import { RxwebValidators } from '@rxweb/reactive-form-validators';

export const InformFactor =
{
  PercentageScore: 1,
  PointsScore: 2,
  Grade: 3,
  DescriptiveGrade: 4,
  CorrectAnwsers: 5,
  PassOrFailMessage: 6
}

export const GradeType =
{
  Grade: 1,
  Descriptive: 2,
  GradeAndDescriptive: 3
}

export const GradingCriteriaConfigType =
{
  PassMask: 1,
  GradeRanges: 2
}

export const RangeUnit =
{
  Percent: 1,
  Point: 2
}

@UntilDestroy()
@Component({
  selector: 'viet-geeks-grading-and-summary',
  templateUrl: './grading-and-summary.component.html',
  styleUrls: ['./grading-and-summary.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GradingAndSummaryComponent extends TestSpecificBaseComponent {
  GradingCriteriaConfigTypeRef = GradingCriteriaConfigType;
  GradeTypeRef = GradeType;
  InformFactorRef = InformFactor;

  Editor = ClassicEditor;
  gradeForm!: FormGroup;
  gradeFormConfigs = {
    rangeUnits: [{
      value: RangeUnit.Percent,
      key: '%'
    },
    {
      value: RangeUnit.Point,
      key: 'p.'
    }],
    gradeTypes: [
      {
        value: GradeType.Grade,
        key: 'Grades'
      },
      {
        value: GradeType.Descriptive,
        key: 'Descriptive grades'
      },
      {
        value: GradeType.GradeAndDescriptive,
        key: 'Grade and descriptive grades'
      },
    ],
    maxPoint: 12
  };

  get gradingCriteriasCtrl() {
    return this.gradeForm.controls['gradingCriterias'] as FormGroup;
  }

  get testEndConfigCtrl() {
    return this.gradeForm.get(['testEndConfig']) as FormGroup;
  }

  getGradingCriteriaCtrl(control: number) {
    return this.gradingCriteriasCtrl.controls[control.toString()] as FormGroup;
  }

  get gradeRangesDetailsCtrl() {
    return this.getGradingCriteriaCtrl(GradingCriteriaConfigType.GradeRanges).controls['details'] as FormArray<FormGroup>;
  }

  get selectedGradeRangeUnit() {
    const unit = this.getGradingCriteriaCtrl(GradingCriteriaConfigType.GradeRanges).controls['unit'].value;

    return find(this.gradeFormConfigs.rangeUnits, c => c.value === unit)?.key;
  }

  getToValueOfGradeRangesDetails(atIndex: number) {
    if (atIndex < 0) {
      return 0;
    }

    const elementAt = this.gradeRangesDetailsCtrl.at(atIndex);
    return elementAt === undefined ? 0 : (elementAt.value as GradeRangeCriteriaDetail).to;
  }

  constructor() {
    super();
  }

  onInit(): void {
    //
  }

  afterGetTest(): void {
    this.gradeForm = this.fb.group({
      testEndConfig: this.fb.group({
        message: ['', [Validators.maxLength(200)]],
        redirectTo: false,
        toAddress: ['', [RxwebValidators.compose({
          validators: [Validators.required, RxwebValidators.url()], conditionalExpression: (f: TestEndConfig) => f.redirectTo === true
        })]]
      }),
      gradingCriterias: this.fb.group({})
    });

    // Triggers
    this.setupControlValidityTrigger(this.testEndConfigCtrl, ['redirectTo'], [['toAddress']]);

    this.gradingCriteriasCtrl.addControl(GradingCriteriaConfigType.PassMask.toString(), this.fb.group({
      $type: GradingCriteriaConfigType.PassMask,
      value: [0, [
        Validators.required,
        RxwebValidators.digit(),
        Validators.min(1),
        RxwebValidators.maxNumber({
          dynamicConfig: (parent) => {
            //todo(tau): get maxium values from server.
            console.log('PassMask validation');
            const value = (parent as GradeRangeCriteria).unit === RangeUnit.Percent ? 100 : this.gradeFormConfigs.maxPoint;
            return { value };
          }
        })]],
      unit: [RangeUnit.Percent, [Validators.required]]
    }));

    this.gradingCriteriasCtrl.addControl(GradingCriteriaConfigType.GradeRanges.toString(), this.fb.group({
      $type: GradingCriteriaConfigType.GradeRanges,
      gradeType: [GradeType.Grade, [Validators.required]],
      unit: [RangeUnit.Percent, [Validators.required]],
      details: this.fb.array([])
    }));

    // Path curremt values.
    const testEndConfig = this.test.gradingSettings?.testEndConfig;
    this.testEndConfigCtrl.patchValue(testEndConfig || {});

    const gradingCriterias = this.test.gradingSettings?.gradingCriterias;
    forIn(gradingCriterias, (v, k) => {
      switch (k) {
        case GradingCriteriaConfigType.PassMask.toString():
          this.gradingCriteriasCtrl.get(k)?.patchValue(v);
          break;
        case GradingCriteriaConfigType.GradeRanges.toString(): {
          const config: GradeRangeCriteria = v;
          this.gradingCriteriasCtrl.get(k)?.patchValue({
            gradeType: config.gradeType,
            unit: config.unit
          });
          const ctrls = config.details.map(d => this.createNewGradeRangeCtrl(d));
          const detailsCtrl = this.gradingCriteriasCtrl.get([k, 'details']) as FormArray;
          ctrls.forEach(ctrl => detailsCtrl.push(ctrl));
          break;
        }
        default:
          break;
      }
    });

    //todo: refactor approach, simplify new and patch
    const informRespondentConfig = this.test.gradingSettings?.informRespondentConfig;
    this.gradeForm.addControl('informRespondentConfig', this.fb.group({
      informViaEmail: informRespondentConfig?.informViaEmail || false,
      passedMessage: informRespondentConfig?.passedMessage,
      failedMessage: informRespondentConfig?.failedMessage,
      informFactors: this.createInformFactorsCtrl(informRespondentConfig?.informFactors)
    }))

    // Setup triggers
    // this.setupControlValidityTrigger(this.gradeForm.get(['gradingCriterias', GradingCriteriaConfigType.PassMask.toString()]) as FormGroup, ['unit'], [['value']]);
    // this.gradeForm.get(['gradingCriterias', GradingCriteriaConfigType.GradeRanges.toString(), 'unit'])?.valueChanges.pipe(untilDestroyed(this)).subscribe(() => {
    //   this.gradeRangesDetailsCtrl.controls.forEach(c => {
    //     c.controls['to'].updateValueAndValidity();
    //   });
    // });

    this.maskReadyForUI();
  }

  private createNewGradeRangeCtrl(detail: Partial<GradeRangeCriteriaDetail>) {
    const formGroup = this.fb.group({
      id: this.testsService.generateRandomCode(),
      to: [detail?.to, [
        Validators.required,
        RxwebValidators.minNumber({
          dynamicConfig: (parent, root) => {
            //todo(tau): refactor this code block.
            console.log('min number validation');
            // const details = ((root as GradingSettings).gradingCriterias[GradingCriteriaConfigType.GradeRanges.toString()] as GradeRangeCriteria).details as GradeRangeCriteriaDetail[];
            // const foundIdx = findIndex(details, d => d.id === parent['id']);
            // const value = foundIdx <= 0 ? 1 : details[foundIdx - 1].to;
            return { value : 12 };
          }
        }),
        // RxwebValidators.maxNumber({
        //   dynamicConfig: (parent, root) => {
        //     console.log('max number config', parent);
        //     const criteria = ((root as GradingSettings).gradingCriterias[GradingCriteriaConfigType.GradeRanges.toString()] as GradeRangeCriteria);
        //     const value = criteria.unit === RangeUnit.Percent ? 100 : this.gradeFormConfigs.maxPoint;
        //     return { value };
        //   }
        // })
      ]],
      grades: this.createGradeMask(detail?.grades)
    });

    //todo: refactor this, reuse it.
    formGroup.controls['to'].valueChanges.pipe(untilDestroyed(this)).subscribe(() => {
      setTimeout(() => {
        this.gradeRangesDetailsCtrl.controls.forEach(c => {
          c.get(['to'])?.updateValueAndValidity();
        });
      });
    });

    return formGroup;
  }

  private createInformFactorsCtrl(factors?: { [key: string]: string }) {
    const form = this.fb.group({});
    forIn(InformFactor, (v) => {
      const existingValue = factors && factors[v];
      form.addControl(v.toString(), this.fb.control(existingValue || false));
    });

    return form;
  }

  private createGradeMask(grades?: { [key: string]: string }) {
    const form = this.fb.group({});
    forIn(GradeType, (v) => {
      const existingValue = grades && grades[v];
      form.addControl(v.toString(), this.fb.control(existingValue));
    });

    return form;
  }

  toggleGradeCriteria(event: { id: number, originEvent: Event }) {
    const enabled = (event.originEvent.target as HTMLInputElement).checked;
    if (enabled) {
      this.getGradingCriteriaCtrl(event.id).enable();
    } else {
      this.getGradingCriteriaCtrl(event.id).disable();
    }
  }

  addNewGradeRange() {
    this.gradeRangesDetailsCtrl.push(this.createNewGradeRangeCtrl({}));
  }

  deleteGrade(i: number) {
    console.log('delete grade', i);
  }

   detailsTrackBy(index: number, detail: FormGroup) {
    return (detail.value as GradeRangeCriteriaDetail).id;
  }

  get canSubmit(): boolean {
    return this.gradeForm.valid;
  }

  async submit() {
    const model = <GradingSettings>this.gradeForm.value;

    await this.testsService.update(this.testId, { gradingSettings: model });
    this.notifyService.success('Grade settings updated');
  }
}
