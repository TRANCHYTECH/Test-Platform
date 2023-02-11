import { Component } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { UntilDestroy } from '@ngneat/until-destroy';
import { forIn } from 'lodash';
import { GradeRangeCriteria, GradeRangeCriteriaDetail, GradingSettings } from '../../../state/test.model';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';

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
        key: 'Descriptive grdades'
      },
      {
        value: GradeType.GradeAndDescriptive,
        key: 'Grade and descriptive grades'
      },
    ]
  };

  get gradingCriteriasCtrl() {
    return this.gradeForm.controls['gradingCriterias'] as FormGroup;
  }

  getGradingCriteriaCtrl(control: number) {
    return this.gradingCriteriasCtrl.controls[control.toString()] as FormGroup;
  }

  get gradeRangesDetailsCtrl() {
    return this.getGradingCriteriaCtrl(GradingCriteriaConfigType.GradeRanges).controls['details'] as FormArray<FormGroup>;
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
        message: ['', [Validators.maxLength(500)]],
        redirectTo: false,
        toAddress: ['', [Validators.pattern('(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?')]]
      }),
      gradingCriterias: this.fb.group({})
    });

    this.gradingCriteriasCtrl.addControl(GradingCriteriaConfigType.PassMask.toString(), this.fb.group({
      $type: GradingCriteriaConfigType.PassMask,
      value: [0, [Validators.required]],
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
    this.gradeForm.get('testEndConfig')?.patchValue(testEndConfig);

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

    this.maskReadyForUI();
  }

  private createNewGradeRangeCtrl(detail?: GradeRangeCriteriaDetail) {
    return this.fb.group({
      from: [detail?.from || 0, [Validators.required]],
      to: [detail?.to, [Validators.required]],
      grades: this.createGradeMask(detail?.grades)
    });
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
      this.getGradingCriteriaCtrl(event.id).disable()
    }
  }

  addNewGradeRange() {
    this.gradeRangesDetailsCtrl.push(this.createNewGradeRangeCtrl())
  }

  deleteGrade(i: number) {
    console.log('delete grade', i);
  }

  async save() {
    console.log('form', this.gradeForm.value);
    if (this.gradeForm.invalid) {
      console.log('grade form error', this.gradeForm.errors);
      return;
    }

    const model = <GradingSettings>this.gradeForm.value;

    await this.testsService.update(this.testId, { gradingSettings: model });
    this.notifyService.success('Grade settings updated');
  }
}
