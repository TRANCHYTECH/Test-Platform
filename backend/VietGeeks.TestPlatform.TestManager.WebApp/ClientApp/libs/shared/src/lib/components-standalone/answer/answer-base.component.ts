import { Component, Input } from '@angular/core';
import { Answer, Question } from '../../models/question.model';

export enum AnswerState {
    NoAnswer = 'no-answer',
    Correct = 'correct',
    Incorrect = 'incorrect'
}

@Component({
    template: ''
})
export class AnswerBaseComponent {
    AnswerState = AnswerState;

    @Input()
    question!: Question;

    @Input()
    answers: string[] = [];

    checkAnswer(input: Answer) {
        // 3 states: correct, incorrect, no-answer.
        if (this.haveUserAnswer(input)) {
            return input.isCorrect ? AnswerState.Correct : AnswerState.Incorrect;
        }

        return AnswerState.NoAnswer;
    }

    haveUserAnswer(input: Answer) {
        return this.answers.includes(input.id);
    }

   userAnswerMark(input: Answer) {
    return this.haveUserAnswer(input) ? 'checked' : null;
   }
}
