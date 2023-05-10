export interface ExamSummary {
    id: string;
    finalMark: number;
    firstName: string;
    lastName: string;
    examineeInfo: { [key: string]: string; };
    startedAt: Date;
    finishedAt: Date;
    totalTime: string;
}

export interface TestRunSummary {
    id: string;
    startAt: Date;
    endAt: Date;
}

export interface Respondent {
    examId: string;
    firstName: string;
    lastName: string;
}