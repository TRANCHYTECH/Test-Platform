import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ExamCurrentStep, RespondentField } from '../../../state/test-session.model';
import { TestRespondentField } from '../../../state/test.model';
import { ProctorService } from '../../services/proctor.service';
import { TestStartConfig } from './data';
import { TestDurationService } from '../../services/test-duration.service';
import { TestSessionService } from '../../services/test-session.service';

@Component({
  selector: 'viet-geeks-test-start',
  templateUrl: './test-start.component.html',
  styleUrls: ['./test-start.component.scss']
})
export class TestStartComponent implements OnInit {

  private _proctorService = inject(ProctorService);
  private _testDurationService = inject(TestDurationService);
  private _testSessionServive = inject(TestSessionService);

  router = inject(Router);
  config = TestStartConfig;
  respondentIdentifyForm: FormGroup;
  labels: string[] = [];

  constructor(private _fb: FormBuilder) {
    this.respondentIdentifyForm = this._fb.group({
      fields: this._fb.array([])
    });
  }

  ngOnInit(): void {
    this.setupSessionData();

    this.config.respondentIdentifyFields.forEach((f, i) => {
      this.addField(f);
      this.labels[i] = f.fieldLabel;
    });
  }

  async startTest() {
    const respondentIdentify = this.respondentIdentifyForm.value as { fields: RespondentField[] } ;
    const sessionData = this._testSessionServive.getSessionData();
    if ((sessionData?.examStep ?? 1) < ExamCurrentStep.ProvideExamineeInfo) {
      await firstValueFrom(this._proctorService.provideExamineeInfo(respondentIdentify.fields));
      this._testSessionServive.setSessionData({
        examStep: ExamCurrentStep.ProvideExamineeInfo
      });
    }

    const startExamOutput = await firstValueFrom(this._proctorService.startExam());

    this._testSessionServive.setSessionData({
      startTime: new Date(startExamOutput.startedAt ?? ''),
      timeSettings: this._testDurationService.mapToTimeSettings(startExamOutput.testDuration),
      respondentFields: respondentIdentify.fields,
      activeQuestion: startExamOutput.activeQuestion,
      questionCount: startExamOutput.totalQuestion,
      canSkipQuestion: startExamOutput.canSkipQuestion,
      examStep: ExamCurrentStep.Start
    });

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
    const sessionData = this._testSessionServive.getSessionData();
    this.config.instruction = sessionData.instructionMessage ?? '';
    this.config.name = sessionData.testDescription ?? '';
    this.config.consentMessage = sessionData.consentMessage ?? '';
  }
}
