import { Component } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { DeactivatableComponent } from '@viet-geeks/shared';
import { differenceWith } from 'lodash-es';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { RespondentIdentifyConfig } from '../../_state/tests/test.model';

@Component({
  selector: 'viet-geeks-test-start-page',
  templateUrl: './test-start-page.component.html',
  styleUrls: ['./test-start-page.component.scss'],
})
export class TestStartPageComponent extends TestSpecificBaseComponent implements DeactivatableComponent {
  testStartPageForm!: FormGroup;

  fieldConfigs = [
    {
      key: 'firstName',
      value: 'FirstName'
    },
    {
      key: 'lastName',
      value: 'LastName'
    },
    {
      key: 'city',
      value: 'City'
    },
    {
      key: 'organizationName',
      value: 'OrganizationName'
    },
    {
      key: 'phone',
      value: 'Phone'
    }
  ];

  canDeactivate: () => boolean | Promise<boolean> = () => !this.testStartPageForm.dirty;

  get respondentIdentifyConfig() {
    return this.testStartPageForm.controls['respondentIdentifyConfig'] as FormArray<FormGroup>;
  }

  postLoadEntity(): void {
    const testStartSettings = this.test.testStartSettings;
    this.testStartPageForm = this.fb.group({
      instruction: [testStartSettings?.instruction],
      consent: [testStartSettings?.consent],
      respondentIdentifyConfig: this.fb.array([])
    });

    testStartSettings?.respondentIdentifyConfig.forEach(c => {
      this.addField(c);
    });
  }

  addNewField() {
    const available = this.availableFields;
    if (available.length === 0)
      return;

    this.addField({ fieldId: available[0].value, isRequired: false });
  }

  get availableFields() {
    const selectedFields = this.respondentIdentifyConfig.value as { fieldId: string }[];
    return differenceWith(this.fieldConfigs, selectedFields, (a, b) => a.value === b.fieldId);
  }

  private addField(c: RespondentIdentifyConfig) {
    this.respondentIdentifyConfig.push(this.fb.group({
      fieldId: [c.fieldId, [Validators.required]],
      isRequired: [c.isRequired, [Validators.required]]
    }));
  }

  get canSubmit(): boolean {
    return this.testStartPageForm.valid;
  }

  async submit() {
    const model = this.testStartPageForm.value;

    await this.testsService.update(this.testId, { testStartSettings: model });

    this.notifyService.success('Test Start Settings updated');
  }
}
