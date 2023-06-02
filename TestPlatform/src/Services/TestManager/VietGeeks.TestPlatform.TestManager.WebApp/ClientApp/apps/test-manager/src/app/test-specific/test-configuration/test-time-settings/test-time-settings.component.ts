import { Component, inject } from '@angular/core';
import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { createMask } from '@ngneat/input-mask';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { UserProfileService } from '@viet-geeks/core';
import { DeactivatableComponent } from '@viet-geeks/shared';
import { assign, isEmpty, isNull, isUndefined, values } from 'lodash-es';
import { TestStatus } from '../../../_state/test-support.model';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { CompleteQuestionDuration, CompleteTestDuration, ManualTestActivation, TimePeriodActivation, TimeSettings } from '../../_state/tests/test.model';

export const TestDurationMethod =
{
  CompleteTestTime: 1,
  CompleteQuestionTime: 2
}

export const TestActivationMethodType =
{
  ManualTest: 1,
  TimePeriod: 2
}

@Component({
  selector: 'viet-geeks-test-time-settings',
  templateUrl: './test-time-settings.component.html',
  styleUrls: ['./test-time-settings.component.scss'],
})
export class TestTimeSettingsComponent extends TestSpecificBaseComponent implements DeactivatableComponent {
  userProfileService = inject(UserProfileService);

  TestActivationMethodTypeRef = TestActivationMethodType;
  timeSettingsForm!: FormGroup;

  dhhmmssInputMask = createMask({
    mask: "9{1,2}.99:99:99",
    inputmode: 'text',
    isComplete: this.isCompleteCheck()
  });

  hhmmInputMask = createMask({
    mask: "99:99",
    inputmode: 'text',
    parser: (value: string) => {
      return `${value}:00`;
    },
    formatter: (value: string) => {
      if (isNull(value) || isUndefined(value) || value === '')
        return '';

      const hhmmss = value.split(/\.|:/);
      return `${hhmmss[0]}:${hhmmss[1]}`;
    },
    isComplete: this.isCompleteCheck()
  });

  mmssInputMask = createMask({
    mask: "99:99",
    inputmode: 'text',
    parser: (value: string) => `00:${value}`,
    formatter: (value: string) => {
      if (isNull(value) || isUndefined(value) || value === '')
        return '';

      const hhmmss = value.split(/\.|:/);
      return `${hhmmss[1]}:${hhmmss[2]}`;
    },
    isComplete: this.isCompleteCheck()
  });

  readonly testDurationMethodOptions = [
    {
      id: TestDurationMethod.CompleteTestTime,
      textKey: 'Time to complete the test: (hh:mm). Max value is 23:59',
      inputMask: this.hhmmInputMask
    },
    {
      id: TestDurationMethod.CompleteQuestionTime,
      textKey: 'Time limit for each test question (mm:ss). Max value is 59:59',
      inputMask: this.mmssInputMask
    }
  ];

  readonly testActivationMethodOptions = [
    {
      id: TestActivationMethodType.ManualTest,
      textKey: 'Manual test activation'
    },
    {
      id: TestActivationMethodType.TimePeriod,
      textKey: 'Activation in set time period'
    }
  ];

  canDeactivate: () => boolean | Promise<boolean> = () => this.isReadonly || !this.timeSettingsForm.dirty;

  private dhhmmssValidator = (control: AbstractControl<string>): ValidationErrors | null => {
    if (isNull(control.value) || isEmpty(control.value) || control.value.indexOf('_') > -1)
      return null;

    // Validate day.
    const valueParts = control.value.split(/\.|:/);
    if (parseInt(valueParts[0]) > 99) {
      return { duration: false };
    }

    if (parseInt(valueParts[1]) > 23) {
      return { duration: false };
    }

    return null;
  }

  private furtureDateValidator(): ValidatorFn {
    return (control: AbstractControl<string>): ValidationErrors | null => {
      const value = control.value;
      if (isNull(value) || isUndefined(value)) {
        return null;
      }

      if (new Date() >= new Date(value)) {
        return { furtureDate: true }
      }

      return null;
    }
  }

  private dateGreaterThanValidator({ fieldName }: { fieldName: string }): ValidatorFn {
    return (control: AbstractControl<string>): ValidationErrors | null => {
      const comparedValue = control.parent?.get([fieldName])?.value;
      const value = control.value;
      if (isNull(comparedValue) || isUndefined(comparedValue) || isNull(value) || isUndefined(value)) {
        return null;
      }

      if (new Date(comparedValue) >= new Date(value)) {
        return { greaterThanDate: { comparedDate: comparedValue } }
      }

      return null;
    }
  }

  private hhmmValidator = (control: AbstractControl<string>): ValidationErrors | null => {
    if (isNull(control.value) || isEmpty(control.value) || control.value.indexOf('_') > -1)
      return null;

    const parts = control.value.split(':');
    const hh = parseInt(parts[0]);
    const mm = parseInt(parts[1]);
    if (isNaN(hh) || isNaN(mm)) {
      return null;
    }


    const maxHH = 23;
    if (hh > maxHH) {
      return {
        maxHours: {
          max: maxHH,
          actual: hh
        }
      };
    }

    const maxMM = 59;
    if (mm > maxMM) {
      return {
        maxMins: {
          max: maxMM,
          actual: mm
        }
      };
    }

    return null;
  };


  private mmssValidator = (control: AbstractControl<string>): ValidationErrors | null => {
    if (isNull(control.value) || isEmpty(control.value) || control.value.indexOf('_') > -1)
      return null;

    const parts = control.value.split(':');
    const mm = parseInt(parts[1]);
    const ss = parseInt(parts[2]);
    if (isNaN(mm) || isNaN(ss)) {
      return null;
    }

    const maxMM = 59;
    if (mm > maxMM) {
      return {
        maxMins: {
          max: maxMM,
          actual: mm
        }
      };
    }

    const maxSS = 59;
    if (ss > maxSS) {
      return {
        maxSeconds: {
          max: maxSS,
          actual: ss
        }
      };
    }

    return null;
  };

  get testDurationMethodCtrl() {
    return this.timeSettingsForm.get(['testDurationMethod']) as FormGroup;
  }

  get testActivationMethodCtrl() {
    return this.timeSettingsForm.get(['testActivationMethod']) as FormGroup;
  }

  get answerQuestionConfigCtrl() {
    return this.timeSettingsForm.get(['answerQuestionConfig']) as FormGroup;
  }

  postLoadEntity(): void {
    const timeSettings = this.test.timeSettings;
    this.timeSettingsForm = this.fb.group({
      testDurationMethod: this.fb.group({
        type: [TestDurationMethod.CompleteTestTime],
        [TestDurationMethod.CompleteTestTime]: this.fb.group({
          duration: [null, [RxwebValidators.required(), this.hhmmValidator]]
        }),
        [TestDurationMethod.CompleteQuestionTime]: this.fb.group({
          duration: [null, [RxwebValidators.required(), this.mmssValidator]]
        })
      }, {}),
      testActivationMethod: this.fb.group({
        type: [TestActivationMethodType.ManualTest],
        [TestActivationMethodType.ManualTest]: this.fb.group({
          activeUntil: [null, [Validators.required, this.dhhmmssValidator]]
        }),
        [TestActivationMethodType.TimePeriod]: this.fb.group({
          activeFromDate: this.fb.control(null, [Validators.required, this.furtureDateValidator()]),
          activeUntilDate: this.fb.control(null, [Validators.required, this.dateGreaterThanValidator({ fieldName: 'activeFromDate' })])
        })
      }),
      answerQuestionConfig: this.fb.group({
        _placeholder: [''],
        skipQuestion: [false, [Validators.required]]
      })
    });

    // Use this in combination with dateGreaterThanValidator to trigger validation again when activeFromDate is changed.
    this.setupControlValidityTrigger(this._destroyRef, this.testActivationMethodCtrl, [TestActivationMethodType.TimePeriod.toString(), 'activeFromDate'], [[TestActivationMethodType.TimePeriod.toString(), 'activeUntilDate']]);
    this.listenTypeChange(this._destroyRef, this.testDurationMethodCtrl, values(TestDurationMethod));
    this.listenTypeChange(this._destroyRef, this.testActivationMethodCtrl, values(TestActivationMethodType));
    // Disable skip question option if the duration method is complete question.
    this.testDurationMethodCtrl.controls['type'].valueChanges.pipe().subscribe((c: number) => {
      const action = this.getChangeControlStateMethod(c === TestDurationMethod.CompleteTestTime);
      this.answerQuestionConfigCtrl.controls['skipQuestion'][action]();
    });

    // Fullfill existing test duration.
    this.testDurationMethodCtrl.patchValue({
      type: timeSettings?.testDurationMethod.$type || TestDurationMethod.CompleteTestTime
    });
    switch (timeSettings?.testDurationMethod.$type) {
      case TestDurationMethod.CompleteTestTime: {
        const method = <CompleteTestDuration>timeSettings.testDurationMethod;
        this.testDurationMethodCtrl.get([TestDurationMethod.CompleteTestTime])?.patchValue({
          duration: method.duration
        });
        break;
      }
      case TestDurationMethod.CompleteQuestionTime: {
        const method = <CompleteQuestionDuration>timeSettings.testDurationMethod;
        this.testDurationMethodCtrl.get([TestDurationMethod.CompleteQuestionTime])?.patchValue({
          duration: method.duration
        });
        break;
      }
    }

    // Fullfill test activation
    this.testActivationMethodCtrl.patchValue({
      type: timeSettings?.testActivationMethod.$type || TestActivationMethodType.ManualTest
    });
    switch (timeSettings?.testActivationMethod.$type) {
      case TestActivationMethodType.ManualTest: {
        const method = <ManualTestActivation>timeSettings.testActivationMethod;
        this.testActivationMethodCtrl.get([TestActivationMethodType.ManualTest])?.setValue({
          activeUntil: method.activeUntil
        });
        break;
      }
      case TestActivationMethodType.TimePeriod: {
        const method = <TimePeriodActivation>timeSettings.testActivationMethod;
        this.testActivationMethodCtrl.get([TestActivationMethodType.TimePeriod])?.setValue({
          activeFromDate: this.userProfileService.convertUtcToLocalDateString(method.activeFromDate),
          activeUntilDate: this.userProfileService.convertUtcToLocalDateString(method.activeUntilDate)
        });
        break;
      }
      default:
        break;
    }

    // Fullfill answer question config
    this.answerQuestionConfigCtrl.patchValue({
      skipQuestion: timeSettings?.answerQuestionConfig?.skipQuestion ?? false
    });
  }

  get canSubmit(): boolean {
    return this.test.status === TestStatus.Draft && this.timeSettingsForm.dirty && this.timeSettingsForm.valid;
  }

  async submit() {
    const model: TimeSettings = {
      testDurationMethod: this.getDurationTime(this.testDurationMethodCtrl),
      testActivationMethod: this.getActivationTime(this.testActivationMethodCtrl),
      answerQuestionConfig: this.answerQuestionConfigCtrl.value
    };

    await this.testsService.update(this.testId, { timeSettings: model });
    this.notifyService.success('Time settings updated');
  }

  private getDurationTime(form: FormGroup) {
    const type = <number>form.controls['type'].value;

    return assign({ $type: type }, form.controls[`${type}`].value);
  }

  private getActivationTime(form: FormGroup) {
    const type = <number>form.controls['type'].value;
    if (type === TestActivationMethodType.ManualTest) {
      return assign({ $type: type }, form.controls[`${type}`].value);
    } else {
      const dates = form.controls[`${type}`].value;
      return {
        $type: type,
        activeFromDate: this.userProfileService.zonedTimeToUtc(dates.activeFromDate),
        activeUntilDate: this.userProfileService.zonedTimeToUtc(dates.activeUntilDate)
      };
    }
  }

  private isCompleteCheck() {
    return (buffer: string[]) => {
      if (buffer.indexOf('_') > -1)
        return false;

      const timeParts = buffer.join('').split(':');
      const part1 = parseInt(timeParts[0]);
      const part2 = parseInt(timeParts[1]);

      return (!isNaN(part1) && !isNaN(part2)) && (part1 !== 0 || part2 !== 0);
    };
  }
}
