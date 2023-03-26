export enum AnswerType {
    SingleChoice = 1,
    MultipleChoice = 2
  }

export interface ExamQuestion {
    id: string;
    description: string;
    answerType: number;
    answers: ExamAnswer[];
}

export interface ExamAnswer {
    id: string;
    description: string;
}

export interface FinishedExam {
    totalPoints: number;
}
