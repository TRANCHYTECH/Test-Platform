import { Component, Input, OnInit } from '@angular/core';
import { PagedSearchResponse, PaginationConfig, defaultPaginationConfig } from '../../models/pagination-config';
import { Observable, firstValueFrom, tap } from 'rxjs';

@Component({
  selector: 'viet-geeks-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export default class PaginationComponent implements OnInit {
  @Input()
  label!: string;

  @Input()
  config: PaginationConfig = defaultPaginationConfig();

  @Input()
  pagedSearchFn!: (page: number, pageSize: number) => Observable<PagedSearchResponse<object>>;


  ngOnInit(): void {
    this.pageChange(1);
  }

  pageChange(page: number) {
    this.config.page = page;
    firstValueFrom(this.pagedSearchFn(page, this.config.pageSize).pipe(this.updatePaginationConfig()));
  }

  private updatePaginationConfig<T>() {
    return tap((rs: PagedSearchResponse<T>) => {
      this.config = Object.assign(this.config, { totalCount: rs.totalCount, pageCount: rs.pageCount });
    });
  }
}
