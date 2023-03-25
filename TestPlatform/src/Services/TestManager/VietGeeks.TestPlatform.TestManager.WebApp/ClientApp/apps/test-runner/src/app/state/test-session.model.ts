import { TestDuration, TestDurationMethodType, TimeSpan } from "../api/models";

export interface TestSession {
  accessCode?: null | string;
  testDescription?: null | string;
  consentMessage?: null | string;
  instructionMessage?: null | string;
  startTime?: Date;
  timeSettings?: TestDuration
}

export interface TimeSettings {
  duration?: TimeSpan;
  method?: TestDurationMethod;
}

export enum TestDurationMethod {
  CompleteTestTime = TestDurationMethodType.$1,
  CompleteQuestionTime = TestDurationMethodType.$2
}
