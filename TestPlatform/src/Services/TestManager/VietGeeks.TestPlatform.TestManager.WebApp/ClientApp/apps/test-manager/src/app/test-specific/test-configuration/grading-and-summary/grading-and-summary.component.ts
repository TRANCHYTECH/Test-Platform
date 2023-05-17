import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { DeactivatableComponent } from '@viet-geeks/shared';
import { find, findIndex, forIn, sumBy } from 'lodash-es';
import { Subject } from 'rxjs';
import { QuestionSummary } from '../../../../../../../libs/shared/src/lib/models/question.model';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { QuestionService } from '../../_state/questions/question.service';
import { GradeRangeCriteria, GradeRangeCriteriaDetail, GradingSettings, PassMaskCriteria, TestEndConfig } from '../../_state/tests/test.model';
import { GradeType, GradeTypeUI, GradingCriteriaConfigType, GradingCriteriaConfigTypeUI, InformFactor, InformFactorCriteriaUI, InformFactorUI, RangeDetailsUI, RangeUnit } from '../../_state/ui/grading-summary-ui.model';

@Component({
  selector: 'viet-geeks-grading-and-summary',
  templateUrl: './grading-and-summary.component.html',
  styleUrls: ['./grading-and-summary.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GradingAndSummaryComponent extends TestSpecificBaseComponent implements DeactivatableComponent {
  GradingCriteriaConfigTypeUI = GradingCriteriaConfigTypeUI;
  GradeTypeUI = GradeTypeUI;
  InformFactorUI = InformFactorUI;

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
    maxPoint: 0,
    maxPercentage: 100
  };

  canDeactivate: () => boolean | Promise<boolean> = () => !this.gradeForm.dirty;

  private _questionService = inject(QuestionService);
  private _refreshInforFactorFormReq$ = new Subject<{ criteriaId: number, enabled: boolean }>();

  get gradingCriteriasCtrl() {
    return this.gradeForm.controls['gradingCriterias'] as FormGroup;
  }

  get testEndConfigCtrl() {
    return this.gradeForm.get(['testEndConfig']) as FormGroup;
  }

  get informRespondentConfigCtrl() {
    return this.gradeForm.controls['informRespondentConfig'] as FormGroup;
  }

  getGradingCriteriaCtrl(control: string) {
    return this.gradingCriteriasCtrl.controls[control] as FormGroup;
  }

  get passMaskGradeCtrl() {
    return this.getGradingCriteriaCtrl(GradingCriteriaConfigTypeUI.PassMask);
  }

  get gradeRangesCtrl() {
    return this.getGradingCriteriaCtrl(GradingCriteriaConfigTypeUI.GradeRanges);
  }

  get gradeRangesDetailsCtrl() {
    return this.gradeRangesCtrl.controls['details'] as FormArray<FormGroup>;
  }

  get gradeRangeUnit() {
    return this.gradeRangesCtrl.controls['unit'].value;
  }

  get selectedGradeRangeUnit() {
    const unit = this.gradeRangeUnit;
    return find(this.gradeFormConfigs.rangeUnits, c => c.value === unit)?.key;
  }

  get selectedGradeRangeType() {
    return this.gradeRangesCtrl.controls['gradeType'].value;
  }

  get isPassMaskGradeCtrlEnabled() {
    return this.passMaskGradeCtrl.enabled;
  }

  get isGradeRangesCtrlEnabled() {
    return this.gradeRangesCtrl.enabled;
  }

  getToValueOfGradeRangesDetails(atIndex: number) {
    if (atIndex < 0) {
      return 0;
    }

    const elementAt = this.gradeRangesDetailsCtrl.at(atIndex);
    return elementAt === undefined ? 0 : (elementAt.value as GradeRangeCriteriaDetail).to;
  }

  async postLoadEntity(): Promise<void> {
    const configs = await Promise.all([this._questionService.getSummary(this.testId)]);
    this.gradeFormConfigs.maxPoint = sumBy(configs[0], (c: QuestionSummary) => c.totalPoints);
    const gradingSettings = this.test.gradingSettings;
    this.gradeForm = this.fb.group({
      testEndConfig: this.createTestEndConfigFormGroup(gradingSettings),
      gradingCriterias: this.createGradingCriteriaFormGroup(gradingSettings),
      informRespondentConfig: this.createInformFactorsFormGroup(gradingSettings)
    });

    // Triggers. 
    this.setupControlValidityTrigger(this._destroyRef, this.testEndConfigCtrl, ['redirectTo'], [['toAddress']]);
    this.setupControlValidityTrigger(this._destroyRef, this.passMaskGradeCtrl, ['unit'], [['value']]);
    this.listenToToggleControlState(this._destroyRef, this.testEndConfigCtrl, 'redirectTo', 'toAddress');

    // Trigger running validators of range [From, To].
    this.gradeRangesCtrl.controls['unit'].valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(() => {
      this.forceRunValidatorsOfGrandeRangeDetailsFormCtrls();
    });

    // Trigger to enable/disable grade ranges based on grade type.
    this.gradeRangesCtrl.controls['gradeType'].valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((gradeType) => {
      this.updateStatuesOfGradeRangeDetailsFormCtrls(gradeType);
      this._refreshInforFactorFormReq$.next({ criteriaId: GradingCriteriaConfigType.GradeRanges, enabled: true });
    });

    // Trigger to enable/disable controls of inform factor form.
    this._refreshInforFactorFormReq$.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(rs => {
      switch (rs.criteriaId) {
        case GradingCriteriaConfigType.PassMask:
          this.updateInformFactorFormByPassMask(rs.enabled);
          break;
        case GradingCriteriaConfigType.GradeRanges:
          this.updateInformFactorFormByGradeType(rs.enabled);
          break;
        default:
          break;
      }
    });
    //todo: validate word count html instead of all html length.
  }

  rangeChanged(atIndex: number) {
    this.forceRunValidatorsOfGrandeRangeDetailsFormCtrls(atIndex + 1);
  }

  removeRangeDetail(i: number) {
    this.gradeRangesDetailsCtrl.removeAt(i);
    this.gradeForm.markAsDirty();
  }

  private forceRunValidatorsOfGrandeRangeDetailsFormCtrls(fromIndex = 0) {
    setTimeout(() => {
      this.gradeRangesDetailsCtrl.controls.forEach((ctrl, idx) => {
        if (idx >= fromIndex) {
          ctrl.controls['to'].updateValueAndValidity();
          ctrl.controls['to'].markAsTouched();
        }
      });
    });
  }

  private createInformFactorsFormGroup(settings: GradingSettings) {
    const informRespondentConfig = settings.informRespondentConfig;
    // Get disabled controls based on grading criteria and selected grade type.
    const disabled: string[] = [];
    forIn(GradingCriteriaConfigTypeUI, (type) => {
      const criterion = settings.gradingCriterias[type];
      let factorKey: string;
      switch (type) {
        case GradingCriteriaConfigTypeUI.PassMask:
          factorKey = type;
          break;
        case GradingCriteriaConfigTypeUI.GradeRanges:
          factorKey = criterion === undefined ? `${type}_${GradeTypeUI.Grade}` : `${type}_${(<GradeRangeCriteria>criterion).gradeType}`;
          break;
        default:
          throw new Error('not supported type');
          break;
      }

      const factor = InformFactorCriteriaUI[factorKey];
      disabled.push(...(factor.disabled ?? []));
      if (criterion === undefined) {
        disabled.push(...(factor.enabled ?? []));
      }
    });

    return this.fb.group({
      informViaEmail: informRespondentConfig?.informViaEmail ?? false,
      passedMessage: [informRespondentConfig?.passedMessage, [Validators.maxLength(200)], [this.textEditorConfigs.editorMaxLength('informRespondentConfig.passedMessage', 1000)]],
      failedMessage: [informRespondentConfig?.failedMessage, [Validators.maxLength(200)], [this.textEditorConfigs.editorMaxLength('informRespondentConfig.failedMessage', 1000)]],
      informFactors: this.createInformFactorsCtrl(disabled, informRespondentConfig?.informFactors)
    });
  }

  private createGradingCriteriaFormGroup(settings: GradingSettings) {
    const result = this.fb.group({});

    forIn(GradingCriteriaConfigTypeUI, (type) => {
      const criterion = settings.gradingCriterias[type];
      switch (type) {
        case GradingCriteriaConfigTypeUI.PassMask: {
          const passMaskCriteria = criterion as PassMaskCriteria;
          result.addControl(type, this.createPassMaskCriteriaFormGroup(passMaskCriteria));
          break;
        }
        case GradingCriteriaConfigTypeUI.GradeRanges: {
          const gradeRangeCriteria = criterion as GradeRangeCriteria;
          result.addControl(type, this.createGradeRangeCriteriaFormGroup(gradeRangeCriteria));
          break;
        }
        default:
          break;
      }
    });

    return result;
  }

  private createTestEndConfigFormGroup(settings: GradingSettings) {
    const testEndConfig = settings.testEndConfig;
    return this.fb.group({
      message: this.fb.control(testEndConfig.message, {
        validators: [Validators.required],
        asyncValidators: [this.textEditorConfigs.editorMaxLength('testEndConfig.message', 1000)]
      }),
      redirectTo: testEndConfig.redirectTo,
      toAddress: this.fb.control({ value: testEndConfig.toAddress, disabled: !testEndConfig.redirectTo }, [RxwebValidators.compose({
        validators: [
          Validators.required,
          Validators.maxLength(1000),
          RxwebValidators.url()
        ],
        conditionalExpression: (cfg: TestEndConfig) => cfg.redirectTo === true
      })])
    });
  }

  private updateStatuesOfGradeRangeDetailsFormCtrls(gradeType?: number) {
    const selectedGrade = gradeType ?? this.getGradingCriteriaCtrl(GradingCriteriaConfigTypeUI.GradeRanges).get('gradeType')?.getRawValue();
    console.log('This should not be called first time. updateStatuesOfGradeRangeDetailsFormCtrls called. Grade type', gradeType);
    const details = this.gradeRangesDetailsCtrl;
    const affected = RangeDetailsUI[selectedGrade];
    affected.enabled?.forEach((c: string) => {
      details.controls.forEach(groupCtrl => {
        groupCtrl.get(['grades', c])?.enable();
      });
    });

    affected.disabled?.forEach((c: string) => {
      details.controls.forEach(groupCtrl => {
        groupCtrl.get(['grades', c])?.disable();
      });
    });
  }

  private updateInformFactorFormByPassMask(isEnabled: boolean) {
    this.updateInformFactorFormBy(isEnabled, GradingCriteriaConfigTypeUI.PassMask);
  }

  //todo: rename it
  private updateInformFactorFormByGradeType(isEnabled: boolean, gradeType?: number) {
    const selectedGrade = gradeType ?? this.getGradingCriteriaCtrl(GradingCriteriaConfigTypeUI.GradeRanges).get('gradeType')?.getRawValue();
    this.updateInformFactorFormBy(isEnabled, GradingCriteriaConfigTypeUI.GradeRanges, selectedGrade);
  }

  private updateInformFactorFormBy(isEnabled: boolean, gradeType: string, subGradeType = '0') {
    const ctrls = this.informRespondentConfigCtrl.get('informFactors') as FormGroup;
    const affectedInformFactors = InformFactorCriteriaUI[subGradeType === '0' ? gradeType : `${gradeType}_${subGradeType}`];
    if (isEnabled) {
      affectedInformFactors.enabled?.forEach((c: string) => {
        ctrls.controls[c].enable();
      });
      affectedInformFactors.disabled?.forEach((c: string) => {
        const ctrl = ctrls.controls[c];
        ctrl.reset(false);
        ctrl.disable();
      });
    } else {
      affectedInformFactors.enabled?.forEach((c: string) => {
        const ctrl = ctrls.controls[c];
        ctrl.reset(false);
        ctrl.disable();
      });

      affectedInformFactors.disabled?.forEach((c: string) => {
        const ctrl = ctrls.controls[c];
        ctrl.reset(false);
        ctrl.disable();
      });
    }
  }

  private createGradeRangeCriteriaFormGroup(criteria?: GradeRangeCriteria): FormGroup {
    const gradeRanges = this.fb.array<FormGroup>([], Validators.required);
    criteria?.details.forEach(d => {
      const ctrl = this.createNewGradeRangeCtrl(criteria?.gradeType ?? GradeType.Grade, d);
      gradeRanges.push(ctrl);
    });

    const result = this.fb.group({
      $type: GradingCriteriaConfigType.GradeRanges,
      gradeType: [criteria?.gradeType ?? GradeType.Grade, [Validators.required]],
      unit: [criteria?.unit ?? RangeUnit.Percent, [Validators.required]],
      details: gradeRanges
    });

    if (criteria === undefined || criteria === null)
      result.disable();

    return result;
  }

  private createPassMaskCriteriaFormGroup(criteria?: PassMaskCriteria): FormGroup {
    const result = this.fb.group({
      $type: [GradingCriteriaConfigType.PassMask],
      value: [criteria?.value, [
        Validators.required,
        RxwebValidators.digit(),
        Validators.min(1),
        RxwebValidators.maxNumber({
          dynamicConfig: (parent) => {
            const maxValue = (<PassMaskCriteria>parent).unit === RangeUnit.Percent ? this.gradeFormConfigs.maxPercentage : this.gradeFormConfigs.maxPoint;
            return { value: maxValue };
          }
        })
      ]],
      unit: [criteria?.unit ?? RangeUnit.Percent, [Validators.required]]
    });

    // In case there is no existing persisted criteria, we disable this form group, to prevent affects on validations
    if (criteria === undefined || criteria === null) {
      result.disable();
    }

    return result;
  }

  private createNewGradeRangeCtrl(gradeType: number, detail: Partial<GradeRangeCriteriaDetail>) {
    const formGroup = this.fb.nonNullable.group({
      id: detail?.id ?? this.testsService.generateRandomCode(),
      to: [detail?.to, [
        Validators.required,
        RxwebValidators.minNumber({
          dynamicConfig: (parent) => {
            const unit = this.gradeRangeUnit;
            const details = this.gradeRangesDetailsCtrl.value as GradeRangeCriteriaDetail[];
            const foundIdx = findIndex(details, d => d.id === parent['id']);
            let minValue = 0;
            if (foundIdx <= 0) {
              minValue = 1;
            } else if (foundIdx < details.length - 1) {
              minValue = details[foundIdx - 1].to;
            } else {
              minValue = unit === RangeUnit.Percent ? 100 : this.gradeFormConfigs.maxPoint;
            }

            return { value: minValue };
          }
        }),
        RxwebValidators.maxNumber({
          dynamicConfig: () => {
            const unit = this.gradeRangeUnit;
            const maxValue = unit === RangeUnit.Percent ? 100 : this.gradeFormConfigs.maxPoint;

            return { value: maxValue };
          }
        })
      ]],
      grades: this.createGradeMask(gradeType, detail?.grades)
    });

    return formGroup;
  }

  private createInformFactorsCtrl(disabled: string[], factors?: { [key: string]: string }) {
    const form = this.fb.group({});
    forIn(InformFactor, (f) => {
      const id = f.toString();
      const existingValue = factors && factors[id];
      form.addControl(id, this.fb.control({ value: existingValue || false, disabled: disabled.indexOf(id) > -1 }));
    });

    return form;
  }

  private createGradeMask(gradeType: number, grades?: { [key: string]: string }) {
    const form = this.fb.group({});
    forIn(GradeTypeUI, gt => {
      if (gt === GradeTypeUI.GradeAndDescriptive) {
        return;
      }

      const existingValue = grades && grades[gt];
      const enabled = gradeType === GradeType.GradeAndDescriptive || gradeType === parseInt(gt);
      form.addControl(gt, this.fb.control({ value: existingValue, disabled: !enabled }, { validators: [Validators.required] }));
    });

    return form;
  }

  // Enable/disable grading criteria.
  toggleGradeCriteria(event: { id: string, originEvent?: Event, enabled?: boolean }) {
    const enabled = event.enabled ?? (event.originEvent?.target as HTMLInputElement).checked;
    const criteriaId = parseInt(event.id);
    const action = this.getChangeControlStateMethod(enabled);
    // Note: it's important to set emitEvent = false
    this.getGradingCriteriaCtrl(event.id)[action]({ emitEvent: false });
    // In case grade range details form control is enabled, we should restore status of individual controls.
    if (enabled && criteriaId === GradingCriteriaConfigType.GradeRanges) {
      this.updateStatuesOfGradeRangeDetailsFormCtrls();
    }

    // Also publish event to update status of inform factor controls.
    this._refreshInforFactorFormReq$.next({ criteriaId: parseInt(event.id), enabled });
  }

  addNewGradeRange() {
    this.gradeRangesDetailsCtrl.push(this.createNewGradeRangeCtrl(this.selectedGradeRangeType ?? GradeType.Grade, {}));
  }

  deleteGrade(i: number) {
    console.log('delete grade', i);
  }

  detailsTrackBy(index: number, detail: FormGroup) {
    return (detail.value as GradeRangeCriteriaDetail).id;
  }

  get canSubmit(): boolean {
    return this.gradeForm.dirty && this.gradeForm.valid;
  }

  async submit() {
    const model = <GradingSettings>this.gradeForm.value;

    await this.testsService.update(this.testId, { gradingSettings: model });
    this.notifyService.success('Grade settings updated');
  }
}
