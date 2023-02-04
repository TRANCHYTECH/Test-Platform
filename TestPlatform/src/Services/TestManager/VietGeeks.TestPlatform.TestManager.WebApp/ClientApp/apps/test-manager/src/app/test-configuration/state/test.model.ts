
export interface Test {
  id: string;
  basicSettings: BasicSettings;
  testSetSettings?: TestSets;
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
