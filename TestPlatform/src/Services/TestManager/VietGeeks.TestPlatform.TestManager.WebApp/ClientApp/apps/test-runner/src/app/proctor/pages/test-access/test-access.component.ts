import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProctorService } from '../../services/proctor.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { FormBuilder, FormGroup } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ToastService } from '@viet-geeks/shared';
import { TestSessionService } from '../../services/test-session.service';
import { FingerprintjsProAngularService } from '@fingerprintjs/fingerprintjs-pro-angular';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-test-access',
  templateUrl: './test-access.component.html',
  styleUrls: ['./test-access.component.scss'],
})
export class TestAccessComponent implements OnInit {

  private _fb = inject(FormBuilder);
  private _route = inject(ActivatedRoute);
  private _router = inject(Router);
  private _proctorService = inject(ProctorService);
  private _notifyService = inject(ToastService);
  private _testSessionService = inject(TestSessionService);
  private _fingerprintjsProAngularService = inject(FingerprintjsProAngularService);

  verifyTestForm: FormGroup;
  isLoading = false;

  constructor() {
    this.verifyTestForm = this._fb.group({
      accessCode: ''
    });
  }

  ngOnInit(): void {
    this._fingerprintjsProAngularService.getVisitorData().then(c => {
      console.log('visitor info', c);
    });

    this._route.params.pipe(untilDestroyed(this)).subscribe(p => {
      const accessCode = p['access-code'];
      if (accessCode) {
        this.verifyTestForm.setValue({
          accessCode: accessCode
        });
      }
    });

  }

  public async verify() {
    const accessCode = this.verifyTestForm.get('accessCode')?.value;
    this.enableLoading();
    const result = await firstValueFrom(this._proctorService.verifyTest({accessCode: accessCode}));
    if (result == null) {
      this._notifyService.error('Access code is not valid');
    }
    else {
      this._testSessionService.setSessionData({
        accessCode: accessCode,
        consentMessage: result.consentMessage,
        instructionMessage: result.instructionMessage,
        testDescription: result.testName
      })
      this._router.navigate(['test','start']);
    }
    this.disableLoading();
  }

  private enableLoading() {
    this.isLoading = true;
    this.verifyTestForm.get('accessCode')?.disable();
  }

  private disableLoading() {
    this.isLoading = false;
    this.verifyTestForm.get('accessCode')?.enable();
  }
}
