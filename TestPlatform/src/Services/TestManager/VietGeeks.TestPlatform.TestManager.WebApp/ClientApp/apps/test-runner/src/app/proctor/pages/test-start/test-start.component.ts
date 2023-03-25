import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom, Timestamp } from 'rxjs';
import { StartExamOutput, TestDuration, TimeSpan } from '../../../api/models';
import { TestSessionService } from '../../../services/test-session.service';
import { TimeSettings } from '../../../state/test-session.model';
import { TestRespondentField } from '../../../state/test.model';
import { ProctorService } from '../../proctor.service';
import { TestStartConfig } from './data';

@Component({
  selector: 'viet-geeks-test-start',
  templateUrl: './test-start.component.html',
  styleUrls: ['./test-start.component.scss']
})
export class TestStartComponent implements OnInit {

  private _proctorService = inject(ProctorService);
  private _testSessionService = inject(TestSessionService);
  router = inject(Router);
  config = TestStartConfig;
  respondentIdentifyForm: FormGroup;
  labels: string[] = [];

  constructor(private _fb: FormBuilder) {
    this.respondentIdentifyForm = this._fb.group({
      fields: this._fb.array([])
    });
  }

  async ngOnInit(): Promise<void> {
    this.setupSessionData();

    this.config.respondentIdentifyFields.forEach((f, i) => {
      this.addField(f);
      this.labels[i] = f.fieldLabel;
    });
  }

  async startTest() {
    const respondentIdentify = this.respondentIdentifyForm.value;
    await firstValueFrom(this._proctorService.provideExamineeInfo(respondentIdentify));
    const startExamOutput = await firstValueFrom(this._proctorService.startExam());
    this._testSessionService.setSessionData({
      startTime: new Date(startExamOutput.startedAt ?? ''),
      timeSettings: this.mapToTimeSettings(startExamOutput.testDuration)
    });

    this._testSessionService.setQuestions(startExamOutput.questions ?? []);
    this.router.navigate(['test/question']);
  }

  get fields() {
    return this.respondentIdentifyForm.get('fields') as FormArray;
  }

  private addField(field: TestRespondentField) {
    const formGroup = this._fb.group({
      id: field.fieldId,
      fieldValue: ['', field.isRequired ? Validators.required : undefined]
    });
    this.fields.push(formGroup);

    return formGroup;
  }

  private setupSessionData() {
    const sessionData = this._testSessionService.getSessionData();
    this.config.instruction = sessionData.instructionMessage ?? '';
    this.config.name = sessionData.testDescription ?? '';
    this.config.consentMessage = sessionData.consentMessage ?? '';
  }


  private mapToTimeSettings(testDuration?: TestDuration): TimeSettings {
    if (testDuration == null) {
      return {};
    }

    const durationString = testDuration.duration?.toString();
    if (durationString == null) {
      return {};
    }

    const duration = durationString.split(':').reduce((r, v, i) => {
      switch (i) {
        case 0:
          r.hours = parseInt(v);
          break;
        case 1:
          r.minutes = parseInt(v);
          break;
      }

      return r;
    }, {} as TimeSpan)

    return {
      duration: duration,
      method: testDuration.method
    };
  }
}
