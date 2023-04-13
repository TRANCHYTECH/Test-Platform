import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { TestRunSummary } from '../../state/exam-summary.model';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { forIn, orderBy } from 'lodash-es';

@Component({
  selector: 'viet-geeks-test-run-selector',
  templateUrl: './test-run-selector.component.html',
  styleUrls: ['./test-run-selector.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestRunSelectorComponent {
  //todo: display with utc offset.
  sortedTestRuns: TestRunSummary[] = [];
  uiSelected: { [key: string]: boolean } = {};
  selectionStatus = 'All dates';

  @Input()
  set data({ value, selectAll }: { value: TestRunSummary[], selectAll: boolean }) {
    this.sortedTestRuns = orderBy(value ?? [], v => v.startAt, 'desc');
    if (selectAll === true) {
      this.sortedTestRuns.forEach(run => this.uiSelected[run.id] = true);
    }
  }

  @Output()
  runsSelected = new EventEmitter<string[]>();

  @ViewChild('dropdown')
  dropdown!: NgbDropdown;

  getSelectedDates() {
    const selected: string[] = [];
    forIn(this.uiSelected, (v, k) => {
      if (v === true) {
        selected.push(k);
      }
    });
    if (selected.length === this.sortedTestRuns.length) {
      return { text: 'All dates', selected: selected };
    }

    return { text: `${selected.length} Dates`, selected: selected };
  }

  select() {
    const selected = this.getSelectedDates();
    this.selectionStatus = selected.text;
    this.runsSelected.emit(selected.selected);
    this.dropdown.close();
  }
}
