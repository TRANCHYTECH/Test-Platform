import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { TestRespondentField } from '../../../state/test.model';
import { ProctorService } from '../../proctor.service';
import { TestStartConfig } from './data';

@Component({
  selector: 'viet-geeks-test-start',
  templateUrl: './test-start.component.html',
  styleUrls: ['./test-start.component.scss']
})
export class TestStartComponent implements OnInit {

  proctorService = inject(ProctorService);
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
    this.config.respondentIdentifyFields.forEach((f, i) => {
      this.addField(f);
      this.labels[i] = f.fieldLabel;
    });

    //todo: remove demo
    await firstValueFrom(this.proctorService.verifyTest('0efy7IyYsEG5ciOPuDOl1RAs12p'));
    await firstValueFrom(this.proctorService.provideExamineeInfo({ firstName: 'tau', lastName: 'dang' }));
    const questions = await firstValueFrom(this.proctorService.startExam());
    console.log('exam', questions);
  }

  startTest() {
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
}
