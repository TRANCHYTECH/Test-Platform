import { Component, OnInit } from '@angular/core';
import { firstValueFrom, Observable } from 'rxjs';
import { Test } from '../../state/test.model';
import { TestsQuery } from '../../state/tests.query';
import { TestsService } from '../../state/tests.service';

@Component({
  selector: 'viet-geeks-test-list',
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.scss']
})
export class TestListComponent implements OnInit {
  tests$!: Observable<Test[]>;
  
  constructor(private _testsQuery: TestsQuery, private _testsService: TestsService) {
  }

  ngOnInit() {
    this.tests$ = this._testsQuery.selectAll();
    firstValueFrom(this._testsService.get());
  }
}
