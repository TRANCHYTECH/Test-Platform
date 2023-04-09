/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpContext } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { ExamStatusWithStep } from '../models/exam-status-with-step';
import { FinishExamOutput } from '../models/finish-exam-output';
import { ProvideExamineeInfoOutput } from '../models/provide-examinee-info-output';
import { ProvideExamineeInfoViewModel } from '../models/provide-examinee-info-view-model';
import { StartExamOutputViewModel } from '../models/start-exam-output-view-model';
import { SubmitAnswerOutput } from '../models/submit-answer-output';
import { SubmitAnswerViewModel } from '../models/submit-answer-view-model';
import { VerifyTestInput } from '../models/verify-test-input';
import { VerifyTestOutputViewModel } from '../models/verify-test-output-view-model';

@Injectable({
  providedIn: 'root',
})
export class ApiExamService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation verify
   */
  static readonly VerifyPath = '/Exam/PreStart/Verify';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `verify$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  verify$Plain$Response(params?: {
    body?: VerifyTestInput
  },
  context?: HttpContext

): Observable<StrictHttpResponse<VerifyTestOutputViewModel>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.VerifyPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<VerifyTestOutputViewModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `verify$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  verify$Plain(params?: {
    body?: VerifyTestInput
  },
  context?: HttpContext

): Observable<VerifyTestOutputViewModel> {

    return this.verify$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<VerifyTestOutputViewModel>) => r.body as VerifyTestOutputViewModel)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `verify()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  verify$Response(params?: {
    body?: VerifyTestInput
  },
  context?: HttpContext

): Observable<StrictHttpResponse<VerifyTestOutputViewModel>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.VerifyPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<VerifyTestOutputViewModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `verify$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  verify(params?: {
    body?: VerifyTestInput
  },
  context?: HttpContext

): Observable<VerifyTestOutputViewModel> {

    return this.verify$Response(params,context).pipe(
      map((r: StrictHttpResponse<VerifyTestOutputViewModel>) => r.body as VerifyTestOutputViewModel)
    );
  }

  /**
   * Path part for operation provideExamineeInfo
   */
  static readonly ProvideExamineeInfoPath = '/Exam/PreStart/ProvideExamineeInfo';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `provideExamineeInfo$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  provideExamineeInfo$Plain$Response(params?: {
    body?: ProvideExamineeInfoViewModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<ProvideExamineeInfoOutput>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.ProvideExamineeInfoPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ProvideExamineeInfoOutput>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `provideExamineeInfo$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  provideExamineeInfo$Plain(params?: {
    body?: ProvideExamineeInfoViewModel
  },
  context?: HttpContext

): Observable<ProvideExamineeInfoOutput> {

    return this.provideExamineeInfo$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<ProvideExamineeInfoOutput>) => r.body as ProvideExamineeInfoOutput)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `provideExamineeInfo()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  provideExamineeInfo$Response(params?: {
    body?: ProvideExamineeInfoViewModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<ProvideExamineeInfoOutput>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.ProvideExamineeInfoPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ProvideExamineeInfoOutput>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `provideExamineeInfo$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  provideExamineeInfo(params?: {
    body?: ProvideExamineeInfoViewModel
  },
  context?: HttpContext

): Observable<ProvideExamineeInfoOutput> {

    return this.provideExamineeInfo$Response(params,context).pipe(
      map((r: StrictHttpResponse<ProvideExamineeInfoOutput>) => r.body as ProvideExamineeInfoOutput)
    );
  }

  /**
   * Path part for operation startExam
   */
  static readonly StartExamPath = '/Exam/Start';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `startExam$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  startExam$Plain$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<StartExamOutputViewModel>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.StartExamPath, 'post');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<StartExamOutputViewModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `startExam$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  startExam$Plain(params?: {
  },
  context?: HttpContext

): Observable<StartExamOutputViewModel> {

    return this.startExam$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<StartExamOutputViewModel>) => r.body as StartExamOutputViewModel)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `startExam()` instead.
   *
   * This method doesn't expect any request body.
   */
  startExam$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<StartExamOutputViewModel>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.StartExamPath, 'post');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<StartExamOutputViewModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `startExam$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  startExam(params?: {
  },
  context?: HttpContext

): Observable<StartExamOutputViewModel> {

    return this.startExam$Response(params,context).pipe(
      map((r: StrictHttpResponse<StartExamOutputViewModel>) => r.body as StartExamOutputViewModel)
    );
  }

  /**
   * Path part for operation submitAnswer
   */
  static readonly SubmitAnswerPath = '/Exam/SubmitAnswer';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `submitAnswer$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  submitAnswer$Plain$Response(params?: {
    body?: SubmitAnswerViewModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<SubmitAnswerOutput>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.SubmitAnswerPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<SubmitAnswerOutput>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `submitAnswer$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  submitAnswer$Plain(params?: {
    body?: SubmitAnswerViewModel
  },
  context?: HttpContext

): Observable<SubmitAnswerOutput> {

    return this.submitAnswer$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<SubmitAnswerOutput>) => r.body as SubmitAnswerOutput)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `submitAnswer()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  submitAnswer$Response(params?: {
    body?: SubmitAnswerViewModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<SubmitAnswerOutput>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.SubmitAnswerPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<SubmitAnswerOutput>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `submitAnswer$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  submitAnswer(params?: {
    body?: SubmitAnswerViewModel
  },
  context?: HttpContext

): Observable<SubmitAnswerOutput> {

    return this.submitAnswer$Response(params,context).pipe(
      map((r: StrictHttpResponse<SubmitAnswerOutput>) => r.body as SubmitAnswerOutput)
    );
  }

  /**
   * Path part for operation finishExam
   */
  static readonly FinishExamPath = '/Exam/Finish';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `finishExam$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  finishExam$Plain$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<FinishExamOutput>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.FinishExamPath, 'post');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<FinishExamOutput>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `finishExam$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  finishExam$Plain(params?: {
  },
  context?: HttpContext

): Observable<FinishExamOutput> {

    return this.finishExam$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<FinishExamOutput>) => r.body as FinishExamOutput)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `finishExam()` instead.
   *
   * This method doesn't expect any request body.
   */
  finishExam$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<FinishExamOutput>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.FinishExamPath, 'post');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<FinishExamOutput>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `finishExam$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  finishExam(params?: {
  },
  context?: HttpContext

): Observable<FinishExamOutput> {

    return this.finishExam$Response(params,context).pipe(
      map((r: StrictHttpResponse<FinishExamOutput>) => r.body as FinishExamOutput)
    );
  }

  /**
   * Path part for operation getExamStatus
   */
  static readonly GetExamStatusPath = '/Exam/Status';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getExamStatus$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getExamStatus$Plain$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<ExamStatusWithStep>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.GetExamStatusPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ExamStatusWithStep>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getExamStatus$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getExamStatus$Plain(params?: {
  },
  context?: HttpContext

): Observable<ExamStatusWithStep> {

    return this.getExamStatus$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<ExamStatusWithStep>) => r.body as ExamStatusWithStep)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getExamStatus()` instead.
   *
   * This method doesn't expect any request body.
   */
  getExamStatus$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<ExamStatusWithStep>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.GetExamStatusPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ExamStatusWithStep>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getExamStatus$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getExamStatus(params?: {
  },
  context?: HttpContext

): Observable<ExamStatusWithStep> {

    return this.getExamStatus$Response(params,context).pipe(
      map((r: StrictHttpResponse<ExamStatusWithStep>) => r.body as ExamStatusWithStep)
    );
  }

}
