export interface Test {
  id: string;
  basicSettings: BasicSettings;
  testSetSettings?: TestSets;
  testAccessSettings?: TestAccess;
  createdOn: Date;
}

export interface BasicSettings {
  name: string;
  category: string;
  description: string;
}

export interface TestSets {
  generatorType: number;
  generator?: {
    $type: number;
    configs: { id: string, draw: number }[]
  }
}

export class TestAccess {
  accessType!: number;
  settings!: IAccessType;
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
    setId: string
  }[];
  attempts: number;
}

export interface TestQuestion {
  id: number;
  questionNo: number;
  questionDefinition: string;
  categoryId: string;
  categoryName: string;
  categoryColor: string;
  answerType: number;
  answerTypeName: string;
  answers?: Answer[];
  scoreSettings: ScoreSettings;
  isMandatory: boolean;
  createdDate: string;
  lastModifiedDate: string;
}

interface ScoreSettings {
  correctPoint: number;
  incorrectPoint: number;
  isPartialAnswersEnabled: boolean;
  maxPoints: number;
  maxWords: number;
}

interface Answer {
  answerDescription: string;
  answerPoint: number;
}

export function createTest(params: Partial<Test>) {
  return params as Test;
}
