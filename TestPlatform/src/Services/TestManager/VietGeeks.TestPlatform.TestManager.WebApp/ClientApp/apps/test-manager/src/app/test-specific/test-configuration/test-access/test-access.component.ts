import { ChangeDetectionStrategy, Component, ElementRef, ViewChild, inject } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { AppSettingsService } from '@viet-geeks/core';
import { DeactivatableComponent } from '@viet-geeks/shared';
import { assign, findKey, forEach, forIn, isNumber, range } from 'lodash-es';
import { AppSettings } from '../../../app-setting.model';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { GroupPasswordType, PrivateAccessCodeType, PublicLinkType, TestAccess, TestAccessType, TestAccessTypeUI, TestInvitationStats } from '../../_state/tests/test.model';

//todo(tau): PLAN - Implement test set selection
@Component({
  selector: 'viet-geeks-test-access',
  templateUrl: './test-access.component.html',
  styleUrls: ['./test-access.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestAccessComponent extends TestSpecificBaseComponent implements DeactivatableComponent {
  readonly TestAccessType = TestAccessType;

  appSettingsService = inject(AppSettingsService);

  testInvitationStats: TestInvitationStats[] = [];
  testAccessForm: FormGroup = this.fb.group({});
  codeGenerationForm: FormGroup = this.fb.group({ count: ['', [Validators.min(1), Validators.max(50)]] });
  testAccessFormConfig = {
    attemptsPerRespondentRange: range(1, 11, 1)
  }

  @ViewChild('allCodesSelection')
  allCodesSelection!: ElementRef<HTMLInputElement>;

  codeSelections: { [key: string]: boolean } = {};

  get hasSelectedCode() {
    return findKey(this.codeSelections, v => v === true) !== undefined;
  }

  get selectedCodes() {
    const rs: string[] = [];
    forIn(this.codeSelections, (v, k) => { if (v === true) { rs.push(k) } });

    return rs;
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
    this._publicTestAccessLink = `${this.appSettingsService.get<AppSettings>().testRunnerBaseUrl}/test/public/${this.testId}`;
    return this._publicTestAccessLink;
  }

  canDeactivate: () => boolean | Promise<boolean> = () => !this.testAccessForm.dirty;

  async postLoadEntity(): Promise<void> {
    // Init default form.
    this.testAccessForm = this.fb.group({
      accessType: this.test.testAccessSettings.accessType ?? TestAccessType.PublicLink,
      [TestAccessTypeUI.PublicLink]: this.createPublicLinkFormGroup(this.test.testAccessSettings),
      [TestAccessTypeUI.PrivateAccessCode]: this.createPrivateAccessCodeFormGroup(this.test.testAccessSettings),
      [TestAccessTypeUI.GroupPassword]: this.createGroupPasswordFormGroup(this.test.testAccessSettings),
      [TestAccessTypeUI.Training]: this.createTrainingFormGroup(this.test.testAccessSettings),
    });

    //todo: move to func
    // Async get test invitation statistics.
    if (this.isActivatedTest) {
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

  checkTestInvitationSent(code: string) {
    const events = this.getTestInvitationStat(code);
    return events.includes('sent');
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

    if (details === undefined || details === null) {
      result.disable();
    }

    return result;
  }

  createPublicLinkFormGroup(setting: TestAccess) {
    const details = setting.accessType === TestAccessType.PublicLink ? <PublicLinkType>setting.settings : null;
    const result = this.fb.group({
      requireAccessCode: this.fb.control({ value: details?.requireAccessCode ?? false, disabled: true }),
      attempts: this.fb.control({ value: details?.attempts ?? 1, disabled: this.isReadonly }, { validators: [Validators.required, Validators.min(1), Validators.max(10)] })
    });

    if (details === undefined || details === null) {
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
      attempts: this.fb.control({ value: details?.attempts ?? 1, disabled: this.isReadonly }, { validators: [Validators.required, Validators.min(1), Validators.max(10)] })
    });

    if (details === undefined || details === null) {
      result.disable();
    }

    return result;
  }

  onAccessTypeSelected(accessType: number) {
    this.accessTypeCtrl.setValue(accessType);
    forIn(TestAccessTypeUI, (t) => {
      const action = this.getChangeControlStateMethod(t === accessType.toString());
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
    this.privateAccessCodeConfigsCtrl.clear();
    codes.forEach(code => this.privateAccessCodeConfigsCtrl.push(this.newAccessCodeConfigCtrl(code)));
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

  async removeAccessCode(code: string) {
    this.codeSelections[code] = true;
    this.removeAccessCodes();
  }

  async removeAccessCodes() {
    const codes = this.selectedCodes;
    if (codes.length === 0) {
      return;
    }

    const codeCtrls = this.privateAccessCodeConfigsCtrl;
    await this.testsService.removeAccessCodes(this.testId, codes);
    forEach(codes, code => {
      const ctrl = codeCtrls.controls.findIndex(c => c.controls['code'].value === code);
      codeCtrls.removeAt(ctrl);
    });
    this.resetCodeSelection();
    this.notifyService.success('Access codes are removed');
  }

  async sendAccessCodes() {
    const codes = this.selectedCodes;
    if (codes.length === 0) {
      return;
    }
    await this.testsService.sendAccessCodes(this.testId, codes);
    this.resetCodeSelection();
    this.notifyService.success('Access codes are scheduled to send');
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
    this.testAccessForm.markAsDirty();
  }

  toggleSelectAllCodes(e: Event) {
    const checked = (e.target as HTMLInputElement).checked;
    this.privateAccessCodeConfigsCtrl.controls.forEach(groupCtrl => {
      const code = groupCtrl.controls['code'].value;
      this.codeSelections[code] = checked;
    });
    this.changeRef.markForCheck();
  }

  copiedTestUrl() {
    this.notifyService.success('Copied test url');
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

  private resetCodeSelection() {
    this.allCodesSelection.nativeElement.checked = false;
    this.codeSelections = {};
    this.changeRef.markForCheck();
  }
}
