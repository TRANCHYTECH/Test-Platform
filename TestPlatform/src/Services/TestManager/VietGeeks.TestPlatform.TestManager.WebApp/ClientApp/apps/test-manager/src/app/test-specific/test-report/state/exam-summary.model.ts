export interface ExamSummary {
    id: string;
    finalMark: number;
    firstName: string;
    lastName: string;
    examineeInfo: { [key: string]: string; };
    startedAt: Date;
    finishedAt: Date;
}

export interface TestRunSummary {
    id: string;
    startAt: Date;
    endAt: Date;
}