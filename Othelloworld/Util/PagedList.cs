using System;
using System.Collections.Generic;
using System.Linq;

namespace Othelloworld.Util
{
	public class PagedList<T>
	{
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }

		public ICollection<T> Items { get; set; }
		public bool HasPrev =>
			CurrentPage > 1;
		public bool HasNext => 
			CurrentPage < TotalPages;
		public PagedList(List<T> items, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			PageSize = pageSize;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			CurrentPage = Math.Min(TotalPages, pageNumber);
			Items = items;
		}

		public static PagedList<T> GetPagedList(IQueryable<T> source, int pageNumber, int pageSize) =>
			new PagedList<T>(
				source.Skip(Math.Min(pageNumber - 1, source.Count() / pageSize) * pageSize)
					.Take(pageSize)
					.ToList(),
				source.Count(),
				pageNumber,
				pageSize
			);
	}
}
