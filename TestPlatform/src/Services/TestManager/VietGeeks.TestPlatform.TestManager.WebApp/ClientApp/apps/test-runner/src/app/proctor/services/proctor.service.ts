import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { ApiExamService } from '../../api/services';
import { StartExamOutputViewModel, SubmitAnswerOutput } from '../../api/models';
@Injectable({
  providedIn: 'root'
})
export class ProctorService {
  private _examService = inject(ApiExamService);

  verifyTest(input: Partial<{ accessCode: string, testId: string }>) {
    return this._examService.verify({
      body: input
    }).pipe(catchError(error => {
      console.log('error', error);
      return of(null);
    }));
  }

  provideExamineeInfo(examineeInfo: { [key: string]: string }) {
    return this._examService.provideExamineeInfo({ body: examineeInfo });
  }

  startExam(): Observable<StartExamOutputViewModel> {
    return this._examService.startExam();
  }

  submitAnswer(answer: { questionId: string, answerIds: string[] }): Observable<SubmitAnswerOutput | null> {
    return this._examService.submitAnswer({body: answer}).pipe(catchError(error => {
      console.log('submit answer error', error);
      return of(null);
    }));
  }

  finishExam() {
    return this._examService.finishExam().pipe(catchError(error => {
      console.log('finish exam error', error);
      return of(null);
    }));
  }
}
