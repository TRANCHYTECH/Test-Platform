export enum AnswerType {
  SingleChoice = 1,
  MultipleChoice = 2
}
export interface Question {
  id: string;
  questionNo: number;
  description: string;
  categoryId: string;
  categoryName?: string;
  categoryColor?: string;
  answerType: number;
  answerTypeName?: string;
  answers?: Answer[];
  scoreSettings: ScoreSettings;
  createdDate: string;
  lastModifiedDate: string;
}

export interface ScoreSettings {
  correctPoint: number;
  incorrectPoint: number;
  isPartialAnswersEnabled?: boolean;
  maxPoints?: number;
  maxWords?: number;
  totalPoints?: number;
  partialIncorrectPoint?: number;
  isDisplayMaximumScore: boolean;
  mustAnswerToContinue: boolean;
  isMandatory: boolean;
}

export interface Answer {
  id: string;
  answerDescription: string;
  answerPoint: number;
  isCorrect: boolean;
}

export function createQuestion(params: Partial<Question>) {
  return params as Question;
}
