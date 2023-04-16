import { ChangeDetectionStrategy, Component } from '@angular/core';
import { UserBaseComponent } from '../_base/user-base.component';
import { FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'viet-geeks-regional-settings',
  templateUrl: './regional-settings.component.html',
  styleUrls: ['./regional-settings.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegionalSettingsComponent extends UserBaseComponent {
  settingsForm!: FormGroup;

  override AfterGetUserData() {
    this.settingsForm = this.fb.group({
      language: [this.userProfile?.regionalSettings?.language, [Validators.required]],
      timeZone: [this.userProfile?.regionalSettings?.timeZone, [Validators.required]]
    });

    this.maskReadyForUI();
  }

  async submit() {
    console.log('settings form', this.settingsForm.value);
    await this.userService.update(this.userProfile.id, { regionalSettings: this.settingsForm.value });
    this.notifyService.success('Regional settings updated');
  }

  override get canSubmit(): boolean {
    return this.settingsForm.dirty && this.settingsForm.valid;
  }
}
