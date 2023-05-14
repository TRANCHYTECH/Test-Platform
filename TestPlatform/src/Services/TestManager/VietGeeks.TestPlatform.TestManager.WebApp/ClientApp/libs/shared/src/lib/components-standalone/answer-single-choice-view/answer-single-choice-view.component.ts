import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnswerBaseComponent } from '../answer/answer-base.component';

@Component({
  selector: 'viet-geeks-answer-single-choice-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './answer-single-choice-view.component.html',
  styleUrls: ['./answer-single-choice-view.component.scss', '../answer/_shared-answer-styles.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AnswerSingleChoiceViewComponent extends AnswerBaseComponent {
}
