import { ChangeDetectionStrategy, Component, DestroyRef, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { filter } from 'lodash-es';
import { BehaviorSubject, debounceTime, distinctUntilChanged } from 'rxjs';
import { Respondent } from '../../_state/exam-summary.model';

@Component({
  selector: 'viet-geeks-respondent-selector',
  templateUrl: './respondent-selector.component.html',
  styleUrls: ['./respondent-selector.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RespondentSelectorComponent implements OnInit {
  selectedRespondent?: Respondent;
  selectedRespondentIdx = 0;

  respondents: Respondent[] = [];
  filteredRespondents$ = new BehaviorSubject<Respondent[]>([]);

  private _destroyRef = inject(DestroyRef);
  private _searchText$ = new BehaviorSubject<string>('');

  @Input()
  set data(value: Respondent[]) {
    this.respondents = value;
    if (this.respondents.length > 0) {
      this.selectRespondent({ data: this.respondents[0] });
      this.filteredRespondents$.next(this.respondents);
    }
  }

  @Output()
  respondentSelected = new EventEmitter<Respondent>();

  get respondentName() {
    return this.selectedRespondent != undefined ? `${this.selectedRespondent.firstName} ${this.selectedRespondent.lastName}` : this._translator.instant('labels.noUser');
  }

  private _translator = inject(TranslateService);

  ngOnInit(): void {
    this._searchText$.pipe(
      takeUntilDestroyed(this._destroyRef),
      debounceTime(50),
      distinctUntilChanged()).subscribe(text => {
        const searchTerm = text.toLowerCase();
        let searchResult: Respondent[];
        if (searchTerm === '') {
          searchResult = this.respondents;
        } else {
          searchResult = filter(this.respondents, (r: Respondent) => {
            return r.firstName.toLowerCase().includes(searchTerm) || r.lastName.toLowerCase().includes(searchTerm);
          });
        }
        this.filteredRespondents$.next(searchResult);
      });
  }

  selectRespondent({ data, target }: { data: Respondent, target?: NgbDropdown }) {
    this.selectedRespondent = data;
    this.selectedRespondentIdx = this.respondents.findIndex(c => c.examId == this.selectedRespondent?.examId)
    this.respondentSelected.next(data);
    target?.close();
  }

  getValue(event: Event) {
    return (event.target as HTMLInputElement).value;
  }

  search(text: string) {
    this._searchText$.next(text);
  }

  canSelectNextRespondent() {
    return this.selectedRespondentIdx < this.respondents.length - 1;
  }

  canSelectPreviousRespondent() {
    return this.selectedRespondentIdx > 0;
  }


  nextRespondent() {
    this.selectRespondent({ data: this.respondents[this.selectedRespondentIdx + 1] });
  }

  previousRespondent() {
    this.selectRespondent({ data: this.respondents[this.selectedRespondentIdx - 1] });
  }
}
