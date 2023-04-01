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

import { FinishExamOutput } from '../models/finish-exam-output';
import { ProvideExamineeInfoViewModel } from '../models/provide-examinee-info-view-model';
import { StartExamOutput } from '../models/start-exam-output';
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
   * To access only the response body, use `provideExamineeInfo()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  provideExamineeInfo$Response(params?: {
    body?: ProvideExamineeInfoViewModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.ProvideExamineeInfoPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
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

): Observable<void> {

    return this.provideExamineeInfo$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
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

): Observable<StrictHttpResponse<StartExamOutput>> {

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
        return r as StrictHttpResponse<StartExamOutput>;
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

): Observable<StartExamOutput> {

    return this.startExam$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<StartExamOutput>) => r.body as StartExamOutput)
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

): Observable<StrictHttpResponse<StartExamOutput>> {

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
        return r as StrictHttpResponse<StartExamOutput>;
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

): Observable<StartExamOutput> {

    return this.startExam$Response(params,context).pipe(
      map((r: StrictHttpResponse<StartExamOutput>) => r.body as StartExamOutput)
    );
  }

  /**
   * Path part for operation submitAnswer
   */
  static readonly SubmitAnswerPath = '/Exam/SubmitAnswer';

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

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.SubmitAnswerPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
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

): Observable<void> {

    return this.submitAnswer$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
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

}
