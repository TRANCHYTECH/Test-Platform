import { AggregatedGrading, Question } from "@viet-geeks/shared";

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

export type ScoresPerQuestionCatalog = {
    categoryId: string;
    categoryName: string;
    totalPoints: number;
    actualPoints: number;
    numberOfQuestions: number;
};

export interface ReadonlyQuestionScore {
    totalPoints: number;
    actualPoints: number;
    answerTime: string;
}

export interface ExamReview {
    firstName: string;
    lastName: string;
    questions: (Question & ReadonlyQuestionScore)[];
    answers: { [questionId: string]: string[] },
    scores: ScoresPerQuestionCatalog[],
    grading: AggregatedGrading[]
}
