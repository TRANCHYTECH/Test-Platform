import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { UntilDestroy } from '@ngneat/until-destroy';
import { assign, range } from 'lodash';

import { GroupPasswordType, PrivateAccessCodeType, PublicLinkType, TestAccess } from '../../../state/test.model';
import { TestSpecificBaseComponent } from '../base/test-specific-base.component';

export const TestAccessType = {
  PublicLink: 1,
  PrivateAccessCode: 2,
  GroupPassword: 3,
  Training: 4
}
//todo(tau): Implement test set selection
@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-access',
  templateUrl: './test-access.component.html',
  styleUrls: ['./test-access.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestAccessComponent extends TestSpecificBaseComponent {
  testAccessForm: FormGroup;
  codeGenerationForm: FormGroup;
  testAccessFormConfig = {
    //todo: generate link
    testLink: 'https://www.dev.testmaster.io/test/DxJCp7RSFqk',
    attemptsPerRespondentRange: range(1, 11, 1)
  }

  readonly testAccessTypeDef = TestAccessType;

  get activatedAccessType() {
    return this.accessTypeCtrl.value;
  }


  get activatedAccessTypeForm() {
    return this.testAccessForm.controls[this.activatedAccessType] as FormGroup;
  }

  get accessTypeCtrl() {
    return this.testAccessForm.controls['accessType'] as FormControl<number>;
  }

  get privateAccessCodeCtrl() {
    return this.testAccessForm.controls[TestAccessType.PrivateAccessCode] as FormGroup;
  }

  get privateAccessCodeConfigsCtrl() {
    return this.privateAccessCodeCtrl.controls['configs'] as FormArray<FormGroup>;
  }

  constructor() {
    super();
    this.testAccessForm = this.fb.group({});
    this.codeGenerationForm = this.fb.group({ count: null });
  }

  onInit(): void {
    //
  }

  afterGetTest(): void {
    // Init default form.
    this.testAccessForm = this.fb.group({
      accessType: TestAccessType.PublicLink
    });
    this.testAccessForm.addControl(TestAccessType.PublicLink.toString(), this.fb.group({
      requireAccessCode: this.fb.control(false),
      attempts: [1, [Validators.required, Validators.min(1), Validators.max(10)]]
    }));
    this.testAccessForm.addControl(TestAccessType.PrivateAccessCode.toString(), this.fb.group({
      configs: this.fb.array([]),
      attempts: [1, [Validators.required, Validators.min(1), Validators.max(10)]]
    }));
    this.testAccessForm.addControl(TestAccessType.GroupPassword.toString(), this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]],
      attempts: [1, [Validators.required, Validators.min(1), Validators.max(10)]]
    }));
    this.testAccessForm.addControl(TestAccessType.Training.toString(), this.fb.group({}));

    // Path current values.
    switch (this.test.testAccessSettings?.accessType) {
      case TestAccessType.PublicLink:
        this.updatePublicLinkForm();
        break;
      case TestAccessType.PrivateAccessCode:
        this.updatePrivateAccessCodeForm();
        break;
      case TestAccessType.GroupPassword:
        this.updateGroupPasswordForm();
        break;
      case TestAccessType.Training:
        this.updateTrainingTypeForm();
        break;
      default:
        break;
    }

    // Set ready to use.
    this.maskReadyForUI();
  }

  onAccessTypeSelected(accessType: number) {
    this.accessTypeCtrl.setValue(accessType);
  }

  generateCodes() {
    const control = this.codeGenerationForm.controls['count'];
    const count = control.value;
    if (count <= 0) {
      return;
    }

    const codes = this.testsService.generateAccessCodes(count);
    codes.forEach(code => this.privateAccessCodeConfigsCtrl.push(this.newAccessCodeConfigCtrl({ code: code })));

    control.patchValue(null);
  }

  generateGroupPassword() {
    const codes = this.testsService.generateAccessCodes(1);
    this.activatedAccessTypeForm.patchValue({
      password: codes[0]
    });
  }

  get canSubmit(): boolean {
    return this.activatedAccessTypeForm.valid;
  }

  async submit() {
    const accessType = this.activatedAccessType;
    const model: TestAccess = {
      accessType: accessType,
      settings: assign({ $type: accessType }, this.activatedAccessTypeForm.value)
    };

    await this.testsService.update(this.testId, { testAccessSettings: model });
    this.notifyService.success('Test access updated');
  }

  private updatePrivateAccessCodeForm() {
    this.accessTypeCtrl.patchValue(TestAccessType.PrivateAccessCode);
    const settings = <PrivateAccessCodeType>this.test.testAccessSettings?.settings;
    const ctrls = settings.configs.map(code => this.newAccessCodeConfigCtrl(code));
    this.privateAccessCodeCtrl.setControl('configs', this.fb.array<FormGroup>(ctrls));
    this.activatedAccessTypeForm.patchValue({
      attempts: settings.attempts
    });
  }

  private updatePublicLinkForm() {
    this.accessTypeCtrl.patchValue(TestAccessType.PublicLink);
    const settings = <PublicLinkType>this.test.testAccessSettings?.settings;
    this.activatedAccessTypeForm.patchValue({
      requireAccessCode: settings.requireAccessCode,
      attempts: settings.attempts
    });
  }

  private updateGroupPasswordForm() {
    this.accessTypeCtrl.patchValue(TestAccessType.GroupPassword);
    const settings = <GroupPasswordType>this.test.testAccessSettings?.settings;
    this.activatedAccessTypeForm.patchValue({
      password: settings.password,
      attempts: settings.attempts
    });
  }

  updateTrainingTypeForm() {
    this.accessTypeCtrl.patchValue(TestAccessType.Training);
  }

  private newAccessCodeConfigCtrl(code: { code: string; email?: string; sendCode?: boolean; setId?: string; }) {
    return this.fb.group({
      code: [code.code, [Validators.required]],
      setId: code.setId,
      email: [code.email, [Validators.email]],
      sendCode: code.sendCode || false
    });
  }
}
