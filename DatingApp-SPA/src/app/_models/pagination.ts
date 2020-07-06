export interface Pagination {
    // info from pagination header
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T> {
    // we will use it also for messages
    result: T;
    pagination: Pagination;
}
