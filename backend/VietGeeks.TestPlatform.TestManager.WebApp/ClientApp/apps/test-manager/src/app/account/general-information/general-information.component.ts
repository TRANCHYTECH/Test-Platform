import { Component } from '@angular/core';
import { UserBaseComponent } from '../_base/user-base.component';

@Component({
  selector: 'viet-geeks-general-information',
  templateUrl: './general-information.component.html',
  styleUrls: ['./general-information.component.scss']
})
export class GeneralInformationComponent extends UserBaseComponent {
  override postLoadEntity(): void {
    //
  }
  
  override submit(): Promise<void> {
    throw new Error('Method not implemented.');
  }
  
  override get canSubmit(): boolean {
    throw new Error('Method not implemented.');
  }
}
