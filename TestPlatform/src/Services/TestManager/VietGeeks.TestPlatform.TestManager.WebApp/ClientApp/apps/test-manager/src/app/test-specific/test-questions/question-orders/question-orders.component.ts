import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LexoRank } from 'lexorank';
import { cloneDeep, forEach } from 'lodash-es';
import { firstValueFrom } from 'rxjs';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { Question } from '../../_state/questions/question.model';
import { QuestionService } from '../../_state/questions/question.service';

@Component({
  selector: 'viet-geeks-question-orders',
  templateUrl: './question-orders.component.html',
  styleUrls: ['./question-orders.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QuestionOrdersComponent extends TestSpecificBaseComponent {
  questions: Question[] = [];
  originalOrders: Question[] = [];
  private _changed = false;
  private _questionsService = inject(QuestionService);

  override postLoadEntity(): void | Promise<void> {
    this._changed = false;
    firstValueFrom(this._questionsService.getOrders(this.testId)).then(rs => {
      if (rs.length > 0) {
        this.questions = rs;
        this.originalOrders = cloneDeep(rs);
        this.changeRef.markForCheck();
      }
    });
  }

  override async submit(): Promise<void> {
    // Get changes.
    const changes: { id: string, order: string }[] = [];
    forEach(this.originalOrders, v => {
      const checkedQuestion = this.questions.find(c => c.id === v.id);
      if (checkedQuestion === undefined) {
        return;
      }

      if (v.order !== checkedQuestion.order) {
        changes.push({ id: checkedQuestion.id, order: checkedQuestion.order });
      }
    });

    if (changes.length === 0) {
      return;
    }

    await firstValueFrom(this._questionsService.updateOrders(this.testId, changes));
    this.notifyService.success('Question orders updated');
  }

  override get canSubmit(): boolean {
    return this._changed;
  }

  questionOrder(idx: number) {
    return this.questions[idx].order;
  }

  drop(event: CdkDragDrop<Question[]>) {
    if (event.currentIndex === this.questions.length - 1) {
      const newOrderRank = this.questionOrder(event.currentIndex);
      const compensitionRank = LexoRank.parse(this.questionOrder(event.currentIndex - 1)).between(LexoRank.parse(this.questionOrder(event.currentIndex))).toString();

      moveItemInArray(this.questions, event.previousIndex, event.currentIndex);

      this.questions[event.currentIndex].order = newOrderRank;
      this.questions[event.currentIndex - 1].order = compensitionRank;
    }
    else if (event.currentIndex === 0) {
      const newOrderRank = this.questionOrder(event.currentIndex);
      const compensitionRank = LexoRank.parse(this.questionOrder(event.currentIndex)).between(LexoRank.parse(this.questionOrder(event.currentIndex + 1))).toString();

      moveItemInArray(this.questions, event.previousIndex, event.currentIndex);

      this.questions[event.currentIndex].order = newOrderRank;
      this.questions[event.currentIndex + 1].order = compensitionRank;

    } else {
      moveItemInArray(this.questions, event.previousIndex, event.currentIndex);

      const newOrderRank = LexoRank.parse(this.questionOrder(event.currentIndex - 1)).between(LexoRank.parse(this.questionOrder(event.currentIndex + 1))).toString();
      this.questions[event.currentIndex].order = newOrderRank;
    }

    this._changed = true;
  }
}
