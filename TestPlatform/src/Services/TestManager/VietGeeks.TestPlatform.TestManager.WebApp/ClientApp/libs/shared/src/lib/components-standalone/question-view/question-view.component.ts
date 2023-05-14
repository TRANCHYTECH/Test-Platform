import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnswerType, Question } from '../../models/question.model';
import { MultipleChoiceAnswerComponent } from '../answer-multiple-choice-view/answer-multiple-choice-view.component';
import { AnswerSingleChoiceViewComponent } from '../answer-single-choice-view/answer-single-choice-view.component';

@Component({
  selector: 'viet-geeks-question-view',
  standalone: true,
  imports: [CommonModule, AnswerSingleChoiceViewComponent, MultipleChoiceAnswerComponent],
  templateUrl: './question-view.component.html',
  styleUrls: ['./question-view.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QuestionViewComponent {
  AnswerType = AnswerType;

  @Input()
  question!: Question;

  @Input()
  answers: string[] = [];
}
