import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, OnInit, inject } from '@angular/core';
import { Observable, firstValueFrom, tap } from 'rxjs';
import { PagedSearchResponse, PaginationConfig, defaultPaginationConfig } from '../../models/pagination-config';

@Component({
  selector: 'viet-geeks-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaginationComponent implements OnInit {
  @Input()
  label!: string;

  @Input()
  config: PaginationConfig = defaultPaginationConfig();

  @Input()
  pagedSearchFn!: (page: number, pageSize: number) => Observable<PagedSearchResponse<object>>;

  private _changeRef = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.pageChange(1);
  }

  refresh() {
    this.pageChange(1);
  }

  pageChange(page: number) {
    this.config.page = page;
    firstValueFrom(this.pagedSearchFn(page, this.config.pageSize).pipe(this.updatePaginationConfig())).then(() => this._changeRef.markForCheck());
  }

  private updatePaginationConfig<T>() {
    return tap((rs: PagedSearchResponse<T>) => {
      this.config = Object.assign(this.config, { totalCount: rs.totalCount, pageCount: rs.pageCount });
    });
  }
}
