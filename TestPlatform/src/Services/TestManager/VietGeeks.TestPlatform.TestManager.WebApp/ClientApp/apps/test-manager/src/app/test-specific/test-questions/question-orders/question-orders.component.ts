import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { QuestionService } from '../../_state/questions/question.service';
import { TestSpecificBaseComponent } from '../../_base/test-specific-base.component';
import { firstValueFrom } from 'rxjs';
import { Question } from '../../_state/questions/question.model';
import { LexoRank } from 'lexorank';
import { cloneDeep, forEach } from 'lodash-es';

@Component({
  selector: 'viet-geeks-question-orders',
  templateUrl: './question-orders.component.html',
  styleUrls: ['./question-orders.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QuestionOrdersComponent extends TestSpecificBaseComponent implements OnInit {
  questions: Question[] = [];
  originalOrders: Question[] = [];
  private _questionsService = inject(QuestionService);

  override postLoadEntity(): void | Promise<void> {
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
      if (checkedQuestion === undefined)
        throw Error('Wrong');

      if (v.order !== checkedQuestion.order) {
        changes.push({ id: checkedQuestion.id, order: checkedQuestion.order });
      }
    });

    if(changes.length === 0) {
      return;
    }
    
    await firstValueFrom(this._questionsService.updateOrders(this.testId, changes));
    this.notifyService.success('Questioin orders updated');
  }

  override get canSubmit(): boolean {
    return true;
  }

  questionOrder(idx: number) {
    return this.questions[idx].order;
  }

  drop(event: CdkDragDrop<Question[]>) {
    let newOrderRank: string;
    if (event.currentIndex === this.questions.length - 1) {
      newOrderRank = this.questionOrder(event.currentIndex);
      const compensitionRank = LexoRank.parse(this.questionOrder(event.currentIndex - 1)).between(LexoRank.parse(this.questionOrder(event.currentIndex))).toString();
      moveItemInArray(this.questions, event.previousIndex, event.currentIndex);
      this.questions[event.currentIndex].order = newOrderRank;
      this.questions[event.currentIndex - 1].order = compensitionRank;
    }
    else if (event.currentIndex === 0) {
      newOrderRank = this.questionOrder(event.currentIndex);
      const compensitionRank = LexoRank.parse(this.questionOrder(event.currentIndex)).between(LexoRank.parse(this.questionOrder(event.currentIndex + 1))).toString();

      moveItemInArray(this.questions, event.previousIndex, event.currentIndex);
      this.questions[event.currentIndex].order = newOrderRank;
      this.questions[event.currentIndex + 1].order = compensitionRank;

    } else {
      moveItemInArray(this.questions, event.previousIndex, event.currentIndex);
      newOrderRank = LexoRank.parse(this.questionOrder(event.currentIndex - 1)).between(LexoRank.parse(this.questionOrder(event.currentIndex + 1))).toString();
      this.questions[event.currentIndex].order = newOrderRank;
    }

    const original = this.questions.map(d => d.order);
    const sorted = this.questions.map(d => d.order).sort();
    console.log('Ranges 0', original);
    console.log('Ranges 1', sorted);
    for (let index = 0; index < original.length; index++) {
      if (original[index] !== sorted[index]) {
        throw Error('wrong');
      }
    }
  }
}
