import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProctorService } from '../../services/proctor.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { FormBuilder } from '@angular/forms';
import { firstValueFrom, interval } from 'rxjs';
import { ToastService } from '@viet-geeks/shared';
import { FingerprintjsProAngularService } from '@fingerprintjs/fingerprintjs-pro-angular';
import { ErrorDetails, VerifyTestOutputViewModel } from '../../../api/models';
import { ExamCurrentStep } from '../../../state/test-session.model';
import { TestSessionService } from '../../services/test-session.service';

@UntilDestroy()
@Component({
  selector: 'viet-geeks-public-test-access',
  templateUrl: './public-test-access.component.html',
  styleUrls: ['./public-test-access.component.scss'],
})
export class PublicTestAccessComponent implements OnInit {

  private _fb = inject(FormBuilder);
  private _route = inject(ActivatedRoute);
  private _router = inject(Router);
  private _proctorService = inject(ProctorService);
  private _notifyService = inject(ToastService);
  private _testSessionService = inject(TestSessionService);
  private _fingerprintjsProAngularService = inject(FingerprintjsProAngularService);

  isLoading = false;
  testId?: string;
  dotCount = 1;
  dots = '';

  ngOnInit(): void {
    this._fingerprintjsProAngularService.getVisitorData().then(c => {
      console.log('visitor info', c);
    });

    this._route.params.pipe(untilDestroyed(this)).subscribe(async p => {
      this.verify(p['test-id']);
    });

    interval(1000)
      .pipe(untilDestroyed(this))
      .subscribe(() => this.updateDots());
  }

  private updateDots() {
    this.dots = '.'.repeat(this.dotCount % 4).padEnd(4, ' ');
    this.dotCount ++;
  }

  public async verify(testId: string) {
    if (!testId) {
      this._router.navigate(['test']);
    }

    const result = await firstValueFrom(this._proctorService.verifyTest({testId: testId}));
    const message = this.tryGetErrorMessageFromResult(result);

    if (message) {
      this._notifyService.error(message);
      this._router.navigate(['test','access']);
    }
    else {
      const verifyOutput = result as VerifyTestOutputViewModel;
      this._testSessionService.setSessionData({
        accessCode: verifyOutput.accessCode,
        consentMessage: verifyOutput.consentMessage,
        instructionMessage: verifyOutput.instructionMessage,
        testDescription: verifyOutput.testName,
        examStep: ExamCurrentStep.VerifyTest
      });

      this._router.navigate(['test','start']);
    }
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
