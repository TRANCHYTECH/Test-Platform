import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { UntilDestroy } from '@ngneat/until-destroy';
import { assign, forIn, isNumber, range } from 'lodash-es';

import { AppSettingsService } from '@viet-geeks/core';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { TestAccessType, TestInvitationStats, TestAccessTypeUI, TestStatus, TestAccess, GroupPasswordType, PublicLinkType, PrivateAccessCodeType } from '../../_state/test.model';
import { AppSettings } from '../../../app-setting.model';

//todo(tau): PLAN - Implement test set selection
@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-access',
  templateUrl: './test-access.component.html',
  styleUrls: ['./test-access.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestAccessComponent extends TestSpecificBaseComponent {
  readonly TestAccessType = TestAccessType;

  appSettingsService = inject(AppSettingsService);

  testInvitationStats: TestInvitationStats[] = [];
  testAccessForm: FormGroup;
  codeGenerationForm: FormGroup;
  testAccessFormConfig = {
    attemptsPerRespondentRange: range(1, 11, 1)
  }

  private _publicTestAccessLink?: string;

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

  get codeGenerationCountCtrl() {
    return this.codeGenerationForm.controls['count'] as FormControl<number>;
  }

  get privateAccessCodeConfigsCtrl() {
    return this.privateAccessCodeCtrl.controls['configs'] as FormArray<FormGroup>;
  }

  get publicTestAccessLink() {
    if (this._publicTestAccessLink !== undefined) {
      return this._publicTestAccessLink;
    }

    //todo: encrypt id instead of real id.
    this._publicTestAccessLink = `${this.appSettingsService.get<AppSettings>().testRunnerBaseUrl}/test/${this.testId}`;
    return this._publicTestAccessLink;
  }

  constructor() {
    super();
    this.testAccessForm = this.fb.group({});
    this.codeGenerationForm = this.fb.group({ count: ['', [Validators.min(1), Validators.max(50)]] });
  }

  async afterGetTest(): Promise<void> {
    // Init default form.
    this.testAccessForm = this.fb.group({
      accessType: this.test.testAccessSettings.accessType ?? TestAccessType.PublicLink,
      [TestAccessTypeUI.PublicLink]: this.createPublicLinkFormGroup(this.test.testAccessSettings),
      [TestAccessTypeUI.PrivateAccessCode]: this.createPrivateAccessCodeFormGroup(this.test.testAccessSettings),
      [TestAccessTypeUI.GroupPassword]: this.createGroupPasswordFormGroup(this.test.testAccessSettings),
      [TestAccessTypeUI.Training]: this.createTrainingFormGroup(this.test.testAccessSettings),
    });

    // Set ready to use.
    this.maskReadyForUI();

    //todo: move to func
    // Async get test invitation statistics.
    if (this.test.status === TestStatus.Activated) {
      this.testsService.getTestInvitationStats(this.test.id).then(rs => {
        this.testInvitationStats = rs;
        this.changeRef.markForCheck();
      });
    }
  }

  getTestInvitationStat(code: string) {
    const events = this.testInvitationStats.find(c => c.accessCode === code)?.events;
    return events === undefined ? 'Not send yet' : events.map(c => c.event).join(',');
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  createTrainingFormGroup(testAccessSettings: TestAccess) {
    return this.fb.group({});
  }

  createGroupPasswordFormGroup(setting: TestAccess) {
    const details = setting.accessType === TestAccessType.GroupPassword ? <GroupPasswordType>setting.settings : null;

    const result = this.fb.group({
      password: [details?.password ?? '', [Validators.required, Validators.minLength(8)]],
      attempts: [details?.attempts ?? 1, [Validators.required, Validators.min(1), Validators.max(10)]]
    });

    if(details === undefined || details === null) {
      result.disable();
    }

    return result;
  }

  createPublicLinkFormGroup(setting: TestAccess) {
    const details = setting.accessType === TestAccessType.PublicLink ? <PublicLinkType>setting.settings : null;
    const result = this.fb.group({
      requireAccessCode: this.fb.control({ value: details?.requireAccessCode ?? false, disabled: true }),
      attempts: [details?.attempts ?? 1, [Validators.required, Validators.min(1), Validators.max(10)]]
    });

    if(details === undefined || details === null) {
      result.disable();
    }

    return result;
  }

  createPrivateAccessCodeFormGroup(setting: TestAccess) {
    const details = setting.accessType === TestAccessType.PrivateAccessCode ? <PrivateAccessCodeType>setting.settings : null;
    const configCtrls = this.fb.array<FormGroup>([]);
    details?.configs.forEach(cfg => configCtrls.push(this.newAccessCodeConfigCtrl(cfg)));

    const result = this.fb.group({
      configs: configCtrls,
      attempts: [details?.attempts ?? 1, [Validators.required, Validators.min(1), Validators.max(10)]]
    });

    if(details === undefined || details === null) {
      result.disable();
    }

    return result;
  }


  onAccessTypeSelected(accessType: number) {
    this.accessTypeCtrl.setValue(accessType);
    forIn(TestAccessTypeUI, (t) => {
      const action = t === accessType.toString() ? 'enable' : 'disable';
      this.testAccessForm.controls[t][action]();
    });
    //todo: remove this action after refactoring model
    this.testAccessForm.markAsDirty();
  }

  get canGenerateCodes() {
    return this.codeGenerationCountCtrl.valid && isNumber(this.codeGenerationCountCtrl.value);
  }

  async generateCodes() {
    const count = this.codeGenerationCountCtrl.value;
    if (count <= 0) {
      return;
    }

    const codes = await this.testsService.generateAccessCodes(this.testId, count);
    codes.forEach(code => this.privateAccessCodeConfigsCtrl.push(this.newAccessCodeConfigCtrl({ code: code })));
    this.codeGenerationCountCtrl.reset(undefined);
    this.changeRef.markForCheck();
  }

  generateCodesFunc = async () => {
    if (this.test.testAccessSettings.accessType !== TestAccessType.PrivateAccessCode) {
      this.notifyService.info('Please save current settings first');
      return;
    }

    await this.generateCodes();
  };

  async removeAccessCode(codeIndex: number) {
    const ctrl = this.privateAccessCodeConfigsCtrl.controls[codeIndex];
    const code = ctrl.controls['code'].value;
    await this.testsService.removeAccessCode(this.testId, code);
    this.privateAccessCodeConfigsCtrl.removeAt(codeIndex);
    this.changeRef.markForCheck();
  }

  generateGroupPassword() {
    const codes = this.testsService.generateRandomCode();
    this.activatedAccessTypeForm.patchValue({
      password: codes[0]
    });
  }

  toggleSelectAllSendCode() {
    this.privateAccessCodeConfigsCtrl.controls.forEach(groupCtrl => {
      groupCtrl.controls['sendCode'].patchValue(true);
    });
  }

  get canSubmit(): boolean {
    return this.testAccessForm.dirty && this.testAccessForm.valid;
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

  private newAccessCodeConfigCtrl(code: { code: string; email?: string; sendCode?: boolean; setId?: string }) {
    return this.fb.group({
      code: [code.code, [Validators.required]],
      setId: code.setId,
      email: [code.email, [Validators.email]],
      sendCode: code.sendCode ?? false
    });
  }
}
