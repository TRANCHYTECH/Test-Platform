import { Component } from '@angular/core';
import { questions } from './data';
@Component({
  selector: 'viet-geeks-manage-questions',
  templateUrl: './manage-questions.component.html',
  styleUrls: ['./manage-questions.component.scss'],
})
export class ManageQuestionsComponent {
  questions: any[];

  constructor() {
    this.questions = questions;
  }
}
