import { Injectable, inject } from '@angular/core';
import { ExamQuestion } from '../../api/models';
import { ExamCurrentStep as ExamStepValue, TestSession } from '../../state/test-session.model';
import { Router, RouterStateSnapshot } from '@angular/router';
import { TestSessionQuery } from '../../state/test-session.query';
import { ApiExamService } from '../../api/services';
import { catchError, firstValueFrom, of } from 'rxjs';
import { TestSessionStore } from '../../state/test-session.store';
import { TestDurationService } from './test-duration.service';

const SESSION_KEY = 'TestSession';
export const SESSION_ID = 1;

@Injectable({ providedIn: 'root' })
export class TestSessionService {
  private _apiExamService = inject(ApiExamService);
  private _sessionData: Partial<TestSession> = {
    questionIndex: 0
  };
  private _questions: ExamQuestion[] = [];
  private _testSessionQuery = inject(TestSessionQuery);
  private _testSessionStore = inject(TestSessionStore);
  private _testDurationService = inject(TestDurationService);
  private _router = inject(Router);

  public saveSessionKey(sessionKey: string) {
    sessionStorage.setItem(SESSION_KEY, sessionKey);
  }

  public getSessionKey() {
    return sessionStorage.getItem(SESSION_KEY) ?? '';
  }

  public async checkSessionStatus(state: RouterStateSnapshot) {
    if (this._testSessionQuery.hasEntity()) {
      const testSession = this._testSessionQuery.getEntity(SESSION_ID);
        const url = this.getRouteFromExamStep(testSession?.examStep as number);
        this.navigateToUrl(state, url);
        return true;
      }

    this._testSessionStore.add({
      id: SESSION_ID
    });

    const status = await firstValueFrom(this.getExamStatus());

    if (status) {
      this._testSessionStore.update((e: TestSession) => e.id == SESSION_ID, {
        activeQuestion: status?.activeQuestion,
        questionIndex: status?.activeQuestionIndex ?? undefined,
        questionCount: status?.questionCount ?? undefined,
        respondentFields: this.mapExamineeInfo(status?.examineeInfo) ?? undefined,
        timeSettings: this._testDurationService.mapToTimeSettings(status.testDuration),
        examStep: status?.step as number
      });
    }

    const url = this.getRouteFromExamStep(status?.step as number);
    this.navigateToUrl(state, url);

    return true;
  }

  public hasSessionData() {
    return !!this._sessionData.accessCode;
  }

  public setSessionData(sessionData: Partial<TestSession>) {
    this._testSessionStore.update(SESSION_ID, sessionData);
  }

  public getSessionData(): Partial<TestSession> {
    return this._testSessionQuery.getEntity(SESSION_ID) ?? {
      id: SESSION_ID
    };
  }

  public setQuestions(questions: ExamQuestion[]) {
    this._questions = questions;
  }

  public getQuestions = () => this._questions;
  public getQuestionsCount = () => (this._questions ?? []).length;

  private getExamStatus() {
    return this._apiExamService.getExamStatus().pipe(catchError(() => of(null)))
  }

  private mapExamineeInfo(examineeInfo: null | undefined | {[key: string]: string}) {
    if (!examineeInfo) {
      return [];
    }

    return Object.entries(examineeInfo).map(([key, value]) => ({
      id: key,
      fieldValue: value
    }));
  }

  private navigateToUrl(state: RouterStateSnapshot, url: string) {
    if (!state.url.includes(url)) {
      this._router.navigate([url]);
    }
  }

  private getRouteFromExamStep(examStep: number): string {
    switch (examStep) {
        case ExamStepValue.VerifyTest:
        case ExamStepValue.ProvideExamineeInfo:
          return 'test/start';
        case ExamStepValue.Start:
        case ExamStepValue.SubmitAnswer:
          return 'test/question';
        case ExamStepValue.FinishExam:
          return 'test/finish';
        default:
          return 'test/access';
    }
  }
}
