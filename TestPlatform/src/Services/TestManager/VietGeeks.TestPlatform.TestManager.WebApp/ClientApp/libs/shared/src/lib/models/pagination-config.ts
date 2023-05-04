
export type PaginationConfig = {
  page: number;
  pageSize: number;
  totalCount: number;
  pageCount: number;
  pageSizes: number[];
};

export function defaultPaginationConfig(): PaginationConfig {
  return {
    page: 1,
    pageSize: 5,
    totalCount: 0,
    pageCount: 0,
    pageSizes: [2, 5, 10, 20, 50, 100]
  }
}

export type PagedSearchResponse<T> = {
  results: T[];
  totalCount: number;
  pageCount: number;
}

export type PagedSearchRequest = {
  page: number;
  pageSize: number;
}
