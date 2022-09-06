using System;
using System.Collections.Generic;
using System.Linq;

namespace Othelloworld.Util
{
	public class PagedList<T> : List<T>
	{
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public bool HasPrev =>
			CurrentPage > 1;
		public bool HasNext => 
			CurrentPage < TotalPages;
		public PagedList(List<T> items, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			PageSize = pageSize;
			CurrentPage = pageNumber;
			TotalPages = (int)Math.Floor(count / (double)pageSize + 0.5);
			AddRange(items);
		}

		public static PagedList<T> GetPagedList(IQueryable<T> source, int pageNumber, int pageSize) =>
			new PagedList<T>(
				source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
				source.Count(),
				pageNumber,
				pageSize
			);
	}
}
