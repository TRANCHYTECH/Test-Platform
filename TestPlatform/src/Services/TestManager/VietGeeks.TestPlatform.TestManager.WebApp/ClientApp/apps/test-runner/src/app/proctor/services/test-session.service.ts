import { Injectable } from '@angular/core';
import { ExamQuestion } from '../../api/models';
import { TestSession } from '../../state/test-session.model';

@Injectable({ providedIn: 'root' })
export class TestSessionService {
  private _sessionData: Partial<TestSession> = {
    questionIndex: 0
  };
  private _questions: ExamQuestion[] = [];

  public hasSessionData() {
    return !!this._sessionData.accessCode;
  }

  public setSessionData(sessionData: Partial<TestSession>) {
    this._sessionData = {...this._sessionData, ...sessionData};
  }

  public getSessionData(): Partial<TestSession> {
    return {...this._sessionData};
  }

  public setQuestions(questions: ExamQuestion[]) {
    this._questions = questions;
  }

  public getQuestions = () => this._questions;
  public getQuestionsCount = () => (this._questions ?? []).length;
}
