export interface ExamContent {
    questions: ExamQuestion[];
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