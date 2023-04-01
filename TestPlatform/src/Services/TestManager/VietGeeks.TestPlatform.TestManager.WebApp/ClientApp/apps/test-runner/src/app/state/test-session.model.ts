import { FinishExamOutput, TestDurationMethodType, TimeSpan } from "../api/models";

export type RespondentField = { id: string, fieldValue: string };

export interface TestSession {
  accessCode?: null | string;
  testDescription?: null | string;
  consentMessage?: null | string;
  instructionMessage?: null | string;
  startTime?: Date;
  endTime?: Date;
  timeSettings?: TimeSettings,
  respondentFields?: Array<RespondentField>,
  result?: FinishExamOutput | null,
  questionIndex?: number
}

export interface TimeSettings {
  duration?: TimeSpan;
  method?: number;
}

export enum TestDurationMethod {
  CompleteTestTime = TestDurationMethodType.$1,
  CompleteQuestionTime = TestDurationMethodType.$2
}
