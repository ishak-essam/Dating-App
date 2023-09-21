export interface Pagination {
    currentPage: number,
    itemsPerPage: number,
    totalItems: number,
    totalPage: number
}
export class PaginationResulat<T> {
    result?: T;
    pagination?:Pagination
}