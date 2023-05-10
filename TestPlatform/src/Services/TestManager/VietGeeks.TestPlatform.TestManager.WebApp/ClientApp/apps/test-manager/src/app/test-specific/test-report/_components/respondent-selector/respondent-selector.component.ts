import { Component, Input } from '@angular/core';
import { Respondent } from '../../_state/exam-summary.model';

@Component({
  selector: 'viet-geeks-respondent-selector',
  templateUrl: './respondent-selector.component.html',
  styleUrls: ['./respondent-selector.component.scss']
})
export class RespondentSelectorComponent {

  selectedRespondent?: Respondent;

  respondents: Respondent[] = [];

  @Input()
  set data({ value }: { value: Respondent[] }) {
    this.respondents = value;
  }

  selectRespondent(item: Respondent) {
    this.selectedRespondent = item;
  }
}
