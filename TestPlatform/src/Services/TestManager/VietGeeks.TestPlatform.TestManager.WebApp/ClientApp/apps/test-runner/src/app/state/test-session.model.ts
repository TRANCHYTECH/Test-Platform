import { FinishExamOutput, TestDurationMethodType, TimeSpan, ExamQuestion } from "../api/models";

export type RespondentField = { id: string, fieldValue: string };

export interface TestSession {
  id?: number,
  examStep?: ExamCurrentStep,
  accessCode?: null | string;
  testDescription?: null | string;
  consentMessage?: null | string;
  instructionMessage?: null | string;
  startTime?: Date;
  endTime?: Date;
  timeSettings?: TimeSettings,
  respondentFields?: Array<RespondentField>,
  result?: FinishExamOutput | null,
  questionIndex?: number,
  questionCount?: number,
  activeQuestion?: ExamQuestion,
  activeQuestionStartAt?: string | null
}

export interface TimeSettings {
  duration?: TimeSpan;
  method?: number;
}

export enum TestDurationMethod {
  CompleteTestTime = TestDurationMethodType.$1,
  CompleteQuestionTime = TestDurationMethodType.$2
}

export enum ExamCurrentStep {
    VerifyTest = 1,
    ProvideExamineeInfo = 2,
    Start = 3,
    SubmitAnswer = 4,
    FinishExam = 5
};
