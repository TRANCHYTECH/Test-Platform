import { Component } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { UntilDestroy } from '@ngneat/until-destroy';
import { assign, padStart } from 'lodash';
import { CompleteQuestionDuration, CompleteTestDuration, ManualTestActivation, TimePeriodActivation, TimeSettings } from '../../../state/test.model';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';
import { createMask, InputmaskOptions } from '@ngneat/input-mask';
import { duration } from 'moment';
import * as moment from 'moment';

export const TestDurationMethod =
{
  CompleteTestTime: 1,
  CompleteQuetsionTime: 2
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
  readonly testDurationMethodOptions = [
    {
      id: TestDurationMethod.CompleteTestTime,
      textKey: 'Time to complete the test: (hh:mm)'
    },
    {
      id: TestDurationMethod.CompleteQuetsionTime,
      textKey: 'Time limit for each test question (mm:ss)'
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

  hhmmInputMask = createMask({
    mask: "99:99",
    
    placeholder: "00:00",
    inputmode: 'text',
    parser: (value: string) => {
      console.log('parse', value);
      return `${value}:00`;
    },
    formatter: (value: string) => {
      console.log('hhmmInputMask', value);
      const hhmmss = duration(value);
      return `${formatDruationValue(hhmmss.hours())}:${formatDruationValue(hhmmss.minutes())}`;
    }
  });

  mmssInputMask = createMask({
    mask: "99:99",
    placeholder: "00:00",
    inputmode: 'text',
    parser: (value: string) => `00:${value}`,
    formatter: (value: string) => {

      const hhmmss = duration(value);
      const rs = `${formatDruationValue(hhmmss.minutes())}:${formatDruationValue(hhmmss.seconds())}`;
      console.log('mmssInputMask', rs);

      return rs;
    }
  });



  readonly durationInputMasks: {[key: string]: InputmaskOptions<string>} = {
    1: this.hhmmInputMask,
    2: this.mmssInputMask
  }

  onInit(): void {
    //
  }

  afterGetTest(): void {
    //
    const timeSettings = this.test.timeSettings;

    //todo(tau): refactor durationxxx control name
    this.timeSettingsForm = this.fb.group({
      testDurationMethod: this.fb.group({
        type: timeSettings?.testDurationMethod.$type || TestDurationMethod.CompleteTestTime,
        1: this.fb.group({
          duration: timeSettings?.testDurationMethod.$type === TestDurationMethod.CompleteTestTime ? (<CompleteTestDuration>timeSettings?.testDurationMethod).duration : null,
        }),
        2: this.fb.group({
          duration: timeSettings?.testDurationMethod.$type === TestDurationMethod.CompleteQuetsionTime ? (<CompleteQuestionDuration>timeSettings?.testDurationMethod).duration : null
        })
      }),
      testActivationMethod: this.fb.group({
        type: timeSettings?.testActivationMethod.$type || TestActivationMethodType.ManualTest,
        1: this.fb.group({
          activeUntil: [null, [Validators.required]]
        }),
        2: this.fb.group({
          activeFromDate: this.fb.control(null, [Validators.required]),
          activeUntilDate: this.fb.control(null, [Validators.required])
        })
      })
    });

    switch (timeSettings?.testActivationMethod.$type) {
      case TestActivationMethodType.ManualTest: {
        const method = <ManualTestActivation>timeSettings.testActivationMethod;
        this.timeSettingsForm.get(['testActivationMethod', TestActivationMethodType.ManualTest])?.patchValue({
          activeUntil: method.activeUntil
        });
        break;
      }
      case TestActivationMethodType.TimePeriod: {
        const method = <TimePeriodActivation>timeSettings.testActivationMethod;
        this.timeSettingsForm.get(['testActivationMethod', TestActivationMethodType.TimePeriod])?.setValue({
          activeFromDate: moment(method.activeFromDate).toISOString().substring(0, 16),
          activeUntilDate: moment(method.activeUntilDate).toISOString().substring(0, 16)
        });
        break;
      }
      default:
        break;
    }

    this.maskReadyForUI();
  }

  async save() {
    const testDuration = this.timeSettingsForm.controls['testDurationMethod'] as FormGroup;
    const testActivationMethod = this.timeSettingsForm.controls['testActivationMethod'] as FormGroup;

    const model: TimeSettings = {
      testDurationMethod: this.getSelectedValue(testDuration),
      testActivationMethod: this.getSelectedValue(testActivationMethod)
    };

    await this.testsService.update(this.testId, { timeSettings: model });
    this.notifyService.success('Time settings updated');
  }

  private getSelectedValue(form: FormGroup) {
    const type = form.controls['type'].value;
    return assign({ $type: type }, form.controls[`${type}`].value);
  }
}
