export interface Question {
  id: string;
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
  id: string;
  answerDescription: string;
  answerPoint: number;
  isCorrect: boolean;
}

export function createQuestion(params: Partial<Question>) {
  return params as Question;
}
