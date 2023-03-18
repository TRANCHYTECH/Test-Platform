import { Injectable } from '@angular/core';
import { ApiExamService } from '../api/services';
import { TestSession } from '../state/test-session.model';

@Injectable({ providedIn: 'root' })
export class TestSessionService {
  constructor(private _examService: ApiExamService) {}
  private _sessionData: Partial<TestSession> = {};

  public setSessionData(sessionData: Partial<TestSession>) {
    this._sessionData = {...this._sessionData, ...sessionData};
  }

  public getSessionData(): Partial<TestSession> {
    return {...this._sessionData};
  }
}
