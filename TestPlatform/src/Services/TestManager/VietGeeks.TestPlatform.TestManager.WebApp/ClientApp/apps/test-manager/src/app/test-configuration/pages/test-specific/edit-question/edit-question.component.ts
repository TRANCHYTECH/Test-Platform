import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { Observable } from 'rxjs';
import { QuestionCategory } from '../../../state/question-categories/question-categories.model';
import { QuestionCategoriesQuery } from '../../../state/question-categories/question-categories.query';
import { QuestionCategoriesService } from '../../../state/question-categories/question-categories.service';
import { AnswerTypes } from './data';

interface QuestionForm {
  questionDescription: FormControl<string | null>;
  category: FormControl<string | null>;
  answerType: FormControl<string | null>;
}

@Component({
  selector: 'viet-geeks-edit-question',
  templateUrl: './edit-question.component.html',
  styleUrls: ['./edit-question.component.scss'],
})
export class EditQuestionComponent implements OnInit {
  Editor = ClassicEditor;
  questionCategories$!: Observable<QuestionCategory[]>;
  questionForm: FormGroup<QuestionForm>;
  answerTypes = AnswerTypes;

  constructor (
    private _fb: FormBuilder,
    private _questionCategoriesQuery: QuestionCategoriesQuery,
     private _questionCategoriesService: QuestionCategoriesService
  ) {
    this.questionCategories$ = this._questionCategoriesQuery.selectAll();
    this.questionForm = this._fb.group({
      questionDescription: '',
      category: '',
      answerType: ''
    });
  }
  ngOnInit(): void { }

  addCategory() {
    this._questionCategoriesService.add({ id: 'Geography', text: 'Geometry' });
  }
}
