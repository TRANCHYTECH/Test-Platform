import { Component, OnInit } from '@angular/core';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { Observable } from 'rxjs';
import { QuestionCategory } from '../../../state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../../state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../../state/question-categories/question-categories.service';
import { Question } from '../../../state/questions/question.model';
import { AnswerTypes } from './data';

@Component({
  selector: 'viet-geeks-edit-question',
  templateUrl: './edit-question.component.html',
  styleUrls: ['./edit-question.component.scss'],
})
export class EditQuestionComponent implements OnInit {
  Editor = ClassicEditor;
  questionCategories$!: Observable<QuestionCategory[]>;
  question!: Question;
  answerTypes = AnswerTypes;

  constructor (
    private _questionCategoriesQuery: QuestionCategoriesQuery,
     private _questionCategoriesService: QuestionCategoriesService
  ) {
    this.questionCategories$ = this._questionCategoriesQuery.selectAll();
  }
  ngOnInit(): void {
  }

  addCategory() {
    this._questionCategoriesService.add({ id: 'Geography', text: 'Geometry' });
  }
}
