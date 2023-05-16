import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProctorService } from '../../services/proctor.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ToastService } from '@viet-geeks/shared';
import { FingerprintjsProAngularService } from '@fingerprintjs/fingerprintjs-pro-angular';
import { ErrorDetails, VerifyTestOutputViewModel } from '../../../api/models';
import { ExamCurrentStep } from '../../../state/test-session.model';
import { TestSessionService } from '../../services/test-session.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

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
  private _destroyRef = inject(DestroyRef);

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

    this._route.params.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(p => {
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
    const message = this.tryGetErrorMessageFromResult(result);

    if (message) {
      this._notifyService.error(message);
    }
    else {
      const verifyOutput = result as VerifyTestOutputViewModel;
      this._testSessionService.setSessionData({
        accessCode: accessCode,
        consentMessage: verifyOutput.consentMessage,
        instructionMessage: verifyOutput.instructionMessage,
        testDescription: verifyOutput.testName,
        examStep: ExamCurrentStep.VerifyTest
      });
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

  private tryGetErrorMessageFromResult(result: VerifyTestOutputViewModel | ErrorDetails | null) {
    let message = '';
    if (result == null || (result as ErrorDetails).error) {
      message = 'Access code is not valid';
      if ((result as ErrorDetails).error) {
        message = (result as ErrorDetails).error as string;
      }
    }

    return message;
  }
}
