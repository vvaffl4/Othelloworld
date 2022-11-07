using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Othelloworld.Data.Repos
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected OthelloDbContext _context;
		public Repository(OthelloDbContext context) => _context = context;
		public void Create(T entity) => _context.Set<T>()
			.Add(entity)
			.Context.SaveChanges();

		public void Update(T entity) => _context.Set<T>()
			.Update(entity)
			.Context.SaveChanges();

		public void Delete(T entity) => _context.Set<T>()
				.Remove(entity)
				.Context.SaveChanges();

		public void Delete(IEnumerable<T> entity) {
			_context.Set<T>()
				.RemoveRange(entity);

			_context.SaveChanges();
		}

		public IQueryable<T> FindAll() => _context.Set<T>()
			.AsNoTracking();

		public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => _context.Set<T>()
			.Where(expression)
			.AsNoTracking();
	}
}
