import { Component, OnInit } from '@angular/core';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { Observable } from 'rxjs';
import { QuestionCategory } from '../../../state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../../state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../../state/question-categories/question-categories.service';
import { TestQuestion } from '../../../state/test.model';
import { QuestionData, AnswerTypes } from './data';

@Component({
  selector: 'viet-geeks-edit-question',
  templateUrl: './edit-question.component.html',
  styleUrls: ['./edit-question.component.scss'],
})
export class EditQuestionComponent implements OnInit {
  Editor = ClassicEditor;
  questionCategories$!: Observable<QuestionCategory[]>;
  question!: TestQuestion;
  answerTypes = AnswerTypes;

  constructor (
    private _questionCategoriesQuery: QuestionCategoriesQuery,
     private _questionCategoriesService: QuestionCategoriesService
  ) {
    this.questionCategories$ = this._questionCategoriesQuery.selectAll();
  }
  ngOnInit(): void {
    this.question = QuestionData;
  }

  addCategory() {
    this._questionCategoriesService.add({ id: 'Geography', text: 'Geometry' });
  }
}
