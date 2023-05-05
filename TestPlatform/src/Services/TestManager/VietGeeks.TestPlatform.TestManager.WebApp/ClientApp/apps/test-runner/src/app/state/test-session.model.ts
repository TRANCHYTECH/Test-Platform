import { TestDurationMethodType, TimeSpan, ExamQuestion, AggregatedGradingOuput, QuestionOutput } from "../api/models";

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
  questionIndex?: number,
  questionCount?: number,
  activeQuestion?: ExamQuestion,
  activeQuestionStartAt?: string | null,
  grading?: null | Array<AggregatedGradingOuput>;
  questions?: QuestionOutput[] | null,
  answers?: Dictionary<Array<string>> | null;
  questionScores?: Dictionary<number> | null;
  canSkipQuestion?: boolean;
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

export enum GradingCriteriaConfigType
{
    PassMask = 1,
    GradeRanges = 2
}

export enum RangeUnit
{
    Percent = 1,
    Point = 2
}

export enum InformFactor
{
    PercentageScore = 1,
    PointsScore = 2,
    Grade = 3,
    DescriptiveGrade = 4,
    CorrectAnwsers = 5,
    PassOrFailMessage = 6
}

export type Dictionary<T> = {
  [key: string]: T
}
