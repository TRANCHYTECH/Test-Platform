import { Component } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { UntilDestroy } from '@ngneat/until-destroy';
import { RespondentIdentifyConfig } from '../../../state/test.model';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-start-page',
  templateUrl: './test-start-page.component.html',
  styleUrls: ['./test-start-page.component.scss'],
})
export class TestStartPageComponent extends TestSpecificBaseComponent {
  Editor = ClassicEditor;

  testStartPageForm!: FormGroup;

  fieldConfigs = [
    {
      key: 'First name - text field',
      value: 'firstName'
    },
    {
      key: 'Last name - text field',
      value: 'lastName'
    }
  ];

  get respondentIdentifyConfig() {
    return this.testStartPageForm.controls['respondentIdentifyConfig'] as FormArray<FormGroup>;
  }

  onInit(): void {
    //
  }

  afterGetTest(): void {
    //
    const testStartSettings = this.test.testStartSettings;
    this.testStartPageForm = this.fb.group({
      instruction: [testStartSettings?.instruction],
      consent: [testStartSettings?.consent],
      respondentIdentifyConfig: this.fb.array([])
    });

    testStartSettings?.respondentIdentifyConfig.forEach(c => {
      this.addField(c);
    });

    this.maskReadyForUI();
  }

  addNewField() {
    this.addField({ fieldId: '', isRequired: false });
  }

  private addField(c: RespondentIdentifyConfig) {
    this.respondentIdentifyConfig.push(this.fb.group({
      fieldId: [c.fieldId, [Validators.required]],
      isRequired: [c.isRequired, [Validators.required]]
    }));
  }

  async save() {
    if (this.testStartPageForm.invalid)
      return;

    const model = this.testStartPageForm.value;

    await this.testsService.update(this.testId, { testStartSettings: model });

    this.notifyService.success('Test Start Settings updated');
  }
}
