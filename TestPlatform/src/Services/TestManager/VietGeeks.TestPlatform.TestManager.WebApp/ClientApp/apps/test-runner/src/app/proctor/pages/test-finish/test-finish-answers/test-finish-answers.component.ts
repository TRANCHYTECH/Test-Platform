import { Component, Input, OnInit } from '@angular/core';
import { AfterTestConfigOutput, QuestionOutput } from 'apps/test-runner/src/app/api/models';
import { Dictionary, TestSession } from 'apps/test-runner/src/app/state/test-session.model';

type AnswerDictionary = {
  [key: string]: boolean;
};

type QuestionToAnswerDictionary = {
  [key: string]: AnswerDictionary;
};

@Component({
  selector: 'viet-geeks-test-finish-answers',
  templateUrl: './test-finish-answers.component.html',
  styleUrls: ['./test-finish-answers.component.css']
})
export class TestFinishAnswersComponent implements OnInit {
  @Input() sessionData: Partial<TestSession> = {};
  @Input() afterTestConfig?: AfterTestConfigOutput | null;

  questions: QuestionOutput[] = [];
  answers: Dictionary<Array<string>> = {};
  answersDictionary: QuestionToAnswerDictionary = {};
  questionScores: { [key: string]: number } = {};

  ngOnInit(): void {
    this.questions = this.sessionData.questions ?? [];
    this.answers = this.sessionData.answers ?? {};
    this.answersDictionary = this.mapAnswersToDictionary();
    this.questionScores = this.sessionData.questionScores ?? {};
  }

  private mapAnswersToDictionary(): QuestionToAnswerDictionary {
    return Object.entries(this.answers).reduce((result, [questionId, answerIds]) => {
      result[questionId] = answerIds.reduce((r, id) => ({ ...r, [id]: true }), {} as AnswerDictionary);
      return result;
    }, {} as QuestionToAnswerDictionary);
  }
}
