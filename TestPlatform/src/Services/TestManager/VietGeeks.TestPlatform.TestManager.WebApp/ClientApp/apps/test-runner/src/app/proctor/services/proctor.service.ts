import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { ApiExamService } from '../../api/services';
import { ActivateQuestionOutput, ErrorDetails, FinishExamOutput, StartExamOutputViewModel, SubmitAnswerOutput } from '../../api/models';
import { RespondentField } from '../../state/test-session.model';
@Injectable({
  providedIn: 'root'
})
export class ProctorService {
  private _examService = inject(ApiExamService);

  verifyTest(input: Partial<{ accessCode: string, testId: string }>) {
    return this._examService.verify({
      body: input
    }).pipe(catchError(error => {
      if (error.status == 400) {
        return of(error.error as ErrorDetails);
      }

      return of(null);
    }));
  }

  provideExamineeInfo(fields: RespondentField[]) {
    const examineeInfo = fields.reduce((r,f) => ({
      ...r,
      [f.id]: f.fieldValue
    }), {});

    return this._examService.provideExamineeInfo({
      body: {
        examineeInfo: examineeInfo
      }
    });
  }

  startExam(): Observable<StartExamOutputViewModel> {
    return this._examService.startExam();
  }

  submitAnswer(answer: { questionId: string, answerIds: string[] }): Observable<SubmitAnswerOutput | null> {
    return this._examService.submitAnswer({ body: answer }).pipe(catchError((error) => {
      console.log("submit answer error: ", error);
      return of(null);
    }));
  }

  finishExam(): Observable<FinishExamOutput | null> {
    return this._examService.finishExam().pipe(catchError(error => {
      console.log('finish exam error', error);
      return of(null);
    }));
  }

  getAfterTestConfig() {
    return this._examService.getAfterTestConfigs()
      .pipe(catchError(error => {
        console.log('get after test config error', error);
        return of(null);
      }));
  }

  activeNextQuestion(): Observable<ActivateQuestionOutput | null> {
    return this._examService.nextQuestion().pipe(catchError(error => {
      console.log('activate question error', error);
      return of(null);
    }));
  }

  activePreviousQuestion(): Observable<ActivateQuestionOutput | null> {
    return this._examService.previousQuestion().pipe(catchError(error => {
      console.log('activate question error', error);
      return of(null);
    }));
  }

  activeQuestion(questionIndex: number): Observable<ActivateQuestionOutput | null> {
    return this._examService.goToQuestion({
      questionIndex
    }).pipe(catchError(error => {
      console.log('activate question error', error);
      return of(null);
    }));
  }
}
