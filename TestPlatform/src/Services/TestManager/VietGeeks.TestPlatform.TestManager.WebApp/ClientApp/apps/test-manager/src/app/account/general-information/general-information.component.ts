import { Component } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { UserBaseComponent } from '../_base/user-base.component';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-general-information',
  templateUrl: './general-information.component.html',
  styleUrls: ['./general-information.component.scss']
})
export class GeneralInformationComponent extends UserBaseComponent {

  override AfterGetUserData() {
    this.maskReadyForUI();
  }

  override submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }
  
  override get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }
}
