export type PagedList<T> = {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  items: T[];
};
