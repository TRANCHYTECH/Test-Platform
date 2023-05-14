import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnswerBaseComponent } from '../answer/answer-base.component';

@Component({
  selector: 'viet-geeks-answer-multiple-choice-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './answer-multiple-choice-view.component.html',
  styleUrls: ['./answer-multiple-choice-view.component.scss', '../answer/_shared-answer-styles.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MultipleChoiceAnswerComponent extends AnswerBaseComponent {
}
