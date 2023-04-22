import { TestStatus } from "@viet-geeks/test-manager/state";

export interface Test {
  id: string;
  basicSettings: BasicSettings;
  testSetSettings: TestSets;
  testAccessSettings: TestAccess;
  gradingSettings: GradingSettings;
  timeSettings: TimeSettings;
  testStartSettings: TestStartSettings;
  currentTestRun: CurrentTestRun;
  createdOn: Date;
  isEnabled: boolean;
  status: TestStatus;
}

export interface TimeSettings {
  testDurationMethod: TestDurationMethod;
  testActivationMethod: TestActivationMethod;
  answerQuestionConfig: AnswerQuestionConfig;
}

export interface CurrentTestRun {
  id: string;
  activatedOrScheduledAtUtc: Date;
}

export interface TestDurationMethod {
  $type: number;
}

export interface CompleteTestDuration extends TestDurationMethod {
  duration: string;
}

export interface CompleteQuestionDuration extends TestDurationMethod {
  duration: string;
}

export interface TestActivationMethod {
  $type: number;
}

export interface ManualTestActivation extends TestActivationMethod {
  activeUntil: string;
  // Readonly
  activeUntilDate: Date;
}

export interface TimePeriodActivation extends TestActivationMethod {
  activeFromDate: Date;
  activeUntilDate: Date;
}

export interface BasicSettings {
  name: string;
  category: string;
  description: string;
}

export interface GradingSettings {
  testEndConfig: TestEndConfig;
  gradingCriterias: { [key: string]: GradingCriteriaConfig };
  informRespondentConfig?: InformRespondentConfig;
}

export interface InformRespondentConfig {
  informViaEmail: boolean;
  passedMessage: string;
  failedMessage: string;
  informFactors: { [key: string]: string };
}

export interface TestEndConfig {
  message?: string;
  redirectTo: boolean;
  toAddress?: string;
}

export interface GradingCriteriaConfig {
  $type: number;
}

export interface PassMaskCriteria extends GradingCriteriaConfig {
  value: number;
  unit: number;
}

export interface GradeRangeCriteria extends GradingCriteriaConfig {
  gradeType: number;
  unit: number;
  details: GradeRangeCriteriaDetail[]
}

export interface GradeRangeCriteriaDetail {
  id?: string;
  to: number,
  grades: { [key: string]: string }
}

export interface TestSets {
  generatorType: number;
  generator?: {
    $type: number;
    configs: { id: string, draw: number }[] | undefined
  }
}

export class TestAccess {
  accessType!: number;
  settings!: IAccessType;
}

export const TestAccessType = {
  PublicLink: 1,
  PrivateAccessCode: 2,
  GroupPassword: 3,
  Training: 4
}

export const TestAccessTypeUI = {
  PublicLink: TestAccessType.PublicLink.toString(),
  PrivateAccessCode: TestAccessType.PrivateAccessCode.toString(),
  GroupPassword: TestAccessType.GroupPassword.toString(),
  Training: TestAccessType.Training.toString()
}

export interface IAccessType {
  $type: number;
}

export interface GroupPasswordType extends IAccessType {
  password: boolean;
  attempts: number;
}

export interface PublicLinkType extends IAccessType {
  requireAccessCode: boolean;
  attempts: number;
}

export interface PrivateAccessCodeType extends IAccessType {
  configs: {
    code: string,
    email: string,
    sendCode: boolean,
    setId: string,
    uiStatuses: any[]
  }[];
  attempts: number;
}

export interface AnswerQuestionConfig {
  skipQuestion: boolean
}

export interface TestStartSettings {
  instruction?: string;
  consent?: string;
  respondentIdentifyConfig: RespondentIdentifyConfig[];
}

export interface RespondentIdentifyConfig {
  fieldId: string;
  isRequired: boolean;
}

export interface TestInvitationStats {
  accessCode: string;
  events: { event: string }[];
}

export enum TestActivationMethodType {
  ManualTest = 1,
  TimePeriod = 2
}

//todo: rename it
export const GeneratorTypes = {
  Default: 1,
  RandomFromCategories: 2
};

export function createTest(params: Partial<Test>) {
  return params as Test;
}


