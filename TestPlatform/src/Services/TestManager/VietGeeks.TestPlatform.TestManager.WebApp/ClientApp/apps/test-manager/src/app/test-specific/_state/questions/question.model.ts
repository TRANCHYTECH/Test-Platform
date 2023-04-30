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
  answers?: Answer[];
  scoreSettings: ScoreSettings;
  createdDate: string;
  lastModifiedDate: string;
}

export interface ScoreSettings {
  $type?: number;
  correctPoint?: number;
  incorrectPoint?: number;
  isPartialAnswersEnabled?: boolean;
  maxPoints?: number;
  maxWords?: number;
  bonusPoints?: number;
  partialIncorrectPoint?: number;
  isDisplayMaximumScore: boolean;
  mustAnswerToContinue: boolean;
  isMandatory: boolean;
  totalPoints?: number;
}

export interface Answer {
  id: string;
  answerDescription: string;
  answerPoint: number;
  isCorrect: boolean;
}
export interface QuestionSummary {
  categoryId: string;
  numberOfQuestions: number;
  totalPoints: number;
}

export function createQuestion(params: Partial<Question>) {
  return params as Question;
}
