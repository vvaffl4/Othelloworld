
interface PagedList<T> {
	currentPage: number;
	totalPages: number;
	pageSize: number;
	totalCount: number;
	items: T[];
}

export default PagedList;