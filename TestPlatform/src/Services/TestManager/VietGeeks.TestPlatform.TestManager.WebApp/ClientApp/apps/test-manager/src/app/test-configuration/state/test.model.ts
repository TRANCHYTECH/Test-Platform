
export interface Test {
  id: string;
  basicSetting: TestBasicSetting;
}

export interface TestBasicSetting {
  name: string;
  category: string;
  description: string;
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
  return {
    id: params.id,
    basicSetting: params.basicSetting
  } as Test;
}
