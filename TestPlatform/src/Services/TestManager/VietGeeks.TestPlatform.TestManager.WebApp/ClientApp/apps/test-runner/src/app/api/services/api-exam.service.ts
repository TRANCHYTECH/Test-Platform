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

import { ProvideExamineeInfoViewModel } from '../models/provide-examinee-info-view-model';
import { SubmitAnswerViewModel } from '../models/submit-answer-view-model';
import { VerifyTestInput } from '../models/verify-test-input';
import { VerifyTestOutput } from '../models/verify-test-output';

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

): Observable<StrictHttpResponse<VerifyTestOutput>> {

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
        return r as StrictHttpResponse<VerifyTestOutput>;
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

): Observable<VerifyTestOutput> {

    return this.verify$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<VerifyTestOutput>) => r.body as VerifyTestOutput)
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

): Observable<StrictHttpResponse<VerifyTestOutput>> {

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
        return r as StrictHttpResponse<VerifyTestOutput>;
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

): Observable<VerifyTestOutput> {

    return this.verify$Response(params,context).pipe(
      map((r: StrictHttpResponse<VerifyTestOutput>) => r.body as VerifyTestOutput)
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
   * Path part for operation startTest
   */
  static readonly StartTestPath = '/Exam/Start';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `startTest()` instead.
   *
   * This method doesn't expect any request body.
   */
  startTest$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ApiExamService.StartTestPath, 'post');
    if (params) {
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
   * To access the full response (for headers, for example), `startTest$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  startTest(params?: {
  },
  context?: HttpContext

): Observable<void> {

    return this.startTest$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
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

}
