import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { untilDestroyed } from '@ngneat/until-destroy';
import { firstValueFrom, Observable } from 'rxjs';
import { Question } from '../../../state/questions/question.model';
import { QuestionsQuery } from '../../../state/questions/question.query';
import { QuestionService } from '../../../state/questions/question.service';
@Component({
  selector: 'viet-geeks-manage-questions',
  templateUrl: './manage-questions.component.html',
  styleUrls: ['./manage-questions.component.scss'],
})
export class ManageQuestionsComponent implements OnInit {
  questions$!: Observable<Question[]>;
  router = inject(Router);
  route = inject(ActivatedRoute);

  testId: string;

  constructor(private _questionsQuery: QuestionsQuery, private _questionsService: QuestionService) {
    this.testId = 'new';
  }

  ngOnInit() {
    this.questions$ = this._questionsQuery.selectAll();
    this.route.params.pipe(untilDestroyed(this)).subscribe(async p => {
      this.testId = p['id'];
      if (!this.isNewTest) {
        await firstValueFrom(this._questionsService.get(this.testId), { defaultValue: null });
        const testDef = this._questionsQuery.getEntity(this.testId);
        if (testDef === undefined) {
          await this.router.navigate(['tests']);
          return;
        }
      }
    });
  }

  get isNewTest() {
    return this.testId === 'new';
  }
}
