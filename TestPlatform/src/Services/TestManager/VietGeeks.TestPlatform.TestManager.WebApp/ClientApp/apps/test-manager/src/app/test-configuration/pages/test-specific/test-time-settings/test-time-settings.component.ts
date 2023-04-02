import { Component } from '@angular/core';
import { AbstractControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { UntilDestroy } from '@ngneat/until-destroy';
import { assign, isEmpty, isNull, padStart } from 'lodash-es';
import { CompleteQuestionDuration, CompleteTestDuration, ManualTestActivation, TestStatus, TimePeriodActivation, TimeSettings } from '../../../state/test.model';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';
import { createMask, InputmaskOptions } from '@ngneat/input-mask';
import { duration } from 'moment';
import { RxwebValidators } from '@rxweb/reactive-form-validators';
import { utcToZonedTime, format, zonedTimeToUtc } from 'date-fns-tz';
import { values } from 'lodash-es';

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

export function formatDruationValue(value: number) {
  return padStart(value.toString(), 2, '0');
}

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-time-settings',
  templateUrl: './test-time-settings.component.html',
  styleUrls: ['./test-time-settings.component.scss'],
})
export class TestTimeSettingsComponent extends TestSpecificBaseComponent {
  TestActivationMethodTypeRef = TestActivationMethodType;
  timeSettingsForm!: FormGroup;
  currentUserTimeZone = 'Asia/Ho_Chi_Minh';
  currentUserTime = format(new Date(), 'd.MM.yyyy HH:mm', { timeZone: this.currentUserTimeZone });

  dhhmmssInputMask = createMask({
    mask: "9{1,2}.99:99:99",
    inputmode: 'text',
    // parser: (value: string) => {
    //   return `${value}:00`;
    // },
    // formatter: (value: string) => {
    //   const hhmmss = duration(value);
    //   return `${formatDruationValue(hhmmss.hours())}:${formatDruationValue(hhmmss.minutes())}`;
    // },
    isComplete: this.isCompleteCheck()
  });

  hhmmInputMask = createMask({
    mask: "99:99",
    inputmode: 'text',
    parser: (value: string) => {
      return `${value}:00`;
    },
    formatter: (value: string) => {
      const hhmmss = duration(value);
      return `${formatDruationValue(hhmmss.hours())}:${formatDruationValue(hhmmss.minutes())}`;
    },
    isComplete: this.isCompleteCheck()
  });

  mmssInputMask = createMask({
    mask: "99:99",
    inputmode: 'text',
    parser: (value: string) => `00:${value}`,
    formatter: (value: string) => {
      const hhmmss = duration(value);
      return `${formatDruationValue(hhmmss.minutes())}:${formatDruationValue(hhmmss.seconds())}`;
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

  get testDurationMethodCtrl() {
    return this.timeSettingsForm.get(['testDurationMethod']) as FormGroup;
  }

  get testActivationMethodCtrl() {
    return this.timeSettingsForm.get(['testActivationMethod']) as FormGroup;
  }

  get answerQuestionConfigCtrl() {
    return this.timeSettingsForm.get(['answerQuestionConfig']) as FormGroup;
  }

  onInit(): void {
    //
  }

  afterGetTest(): void {
    //todo: add validation rules for activation in set time period.
    const timeSettings = this.test.timeSettings;
    this.timeSettingsForm = this.fb.group({
      testDurationMethod: this.fb.group({
        type: [TestDurationMethod.CompleteTestTime],
        1: this.fb.group({
          duration: [null, [RxwebValidators.required(), this.hhmmValidator]]
        }),
        2: this.fb.group({
          duration: [null, [RxwebValidators.required(), this.mmssValidator]]
        })
      }),
      testActivationMethod: this.fb.group({
        type: [TestActivationMethodType.ManualTest],
        1: this.fb.group({
          activeUntil: [null, [Validators.required]]
        }),
        2: this.fb.group({
          activeFromDate: this.fb.control(null, [Validators.required]),
          activeUntilDate: this.fb.control(null, [Validators.required])
        })
      }),
      answerQuestionConfig: this.fb.group({
        skipQuestion: [false, [Validators.required]]
      })
    });

    this.listenTypeChange(this.testDurationMethodCtrl, this, values(TestDurationMethod));
    this.listenTypeChange(this.testActivationMethodCtrl, this, values(TestActivationMethodType));

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
          activeFromDate: this.convertUtcToLocalDateString(method.activeFromDate),
          activeUntilDate: this.convertUtcToLocalDateString(method.activeUntilDate)
        });
        break;
      }
      default:
        break;
    }

    // Fullfill answer question config
    this.answerQuestionConfigCtrl.setValue({
      skipQuestion: timeSettings?.answerQuestionConfig?.skipQuestion ?? false
    });

    this.maskReadyForUI();
  }

  get canSubmit(): boolean {
    return this.test.status === TestStatus.Draft && this.timeSettingsForm.valid;
  }

  async submit() {
    const testDuration = this.timeSettingsForm.controls['testDurationMethod'] as FormGroup;
    const testActivationMethod = this.timeSettingsForm.controls['testActivationMethod'] as FormGroup;

    const model: TimeSettings = {
      testDurationMethod: this.getDurationTime(testDuration),
      testActivationMethod: this.getActivationTime(testActivationMethod),
      answerQuestionConfig: this.timeSettingsForm.controls['answerQuestionConfig'].value
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
        activeFromDate: zonedTimeToUtc(dates.activeFromDate, this.currentUserTimeZone),
        activeUntilDate: zonedTimeToUtc(dates.activeUntilDate, this.currentUserTimeZone)
      };
    }
  }

  private convertUtcToLocalDateString(input: Date) {
    return format(utcToZonedTime(input, this.currentUserTimeZone), 'yyyy-MM-dd HH:mm');
  }
}
