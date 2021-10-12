using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pagination.EntityFrameworkCore.Extensions
{
	public static class PaginationExtensions
	{
		public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this IQueryable<TSource> source, int page, int limit, string sortColumn = "", bool orderByDescending = false)
		{
			ValidateInputs(page, limit);

			var totalItems = await source.CountAsync();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = source.Skip((page - 1) * limit).Take(limit);

			return new Pagination<TSource>(await results.ToListAsync(), totalItems, page, limit);
		}

		public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this DbSet<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, string sortColumn = "", bool orderByDescending = false) where TSource : class
		{
			ValidateInputs(page, limit);

			var totalItems = await source.Where(expression).CountAsync();
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn))).ToListAsync();
			}
			else
			{
				results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync();
			}
			return new Pagination<TSource>(results, totalItems, page, limit);
		}

		// PaginationAuto Mapping
		public static async Task<PaginationAuto<TSource, Tdestination>> AsPaginationAsync<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Func<TSource, Tdestination> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
		{
			ValidateInputs(page, limit);

			var totalItems = await source.CountAsync();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = source.Skip((page - 1) * limit).Take(limit);

			return new PaginationAuto<TSource, Tdestination>(await results.ToListAsync(), totalItems, convertTsourceToTdestinationMethod, page, limit);
		}

		public static async Task<PaginationAuto<TSource, Tdestination>> AsPaginationAsync<TSource, Tdestination>(this DbSet<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, Tdestination> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
		{
			ValidateInputs(page, limit);

			var totalItems = await source.Where(expression).CountAsync();
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn))).ToListAsync();
			}
			else
			{
				results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync();
			}
			return new PaginationAuto<TSource, Tdestination>(results, totalItems, convertTsourceToTdestinationMethod, page, limit);
		}


		// PaginationAuto Async Mapping
		public static async Task<PaginationAuto<TSource, Tdestination>> AsPaginationAsync<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Task<Func<TSource, Tdestination>> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
		{
			ValidateInputs(page, limit);

			var totalItems = await source.CountAsync();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = source.Skip((page - 1) * limit).Take(limit);

			return new PaginationAuto<TSource, Tdestination>(await results.ToListAsync(), totalItems, await convertTsourceToTdestinationMethod, page, limit);
		}

		public static async Task<PaginationAuto<TSource, Tdestination>> AsPaginationAsync<TSource, Tdestination>(this DbSet<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, Task<Func<TSource, Tdestination>> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
		{
			ValidateInputs(page, limit);

			var totalItems = await source.Where(expression).CountAsync();
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn))).ToListAsync();
			}
			else
			{
				results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync();
			}
			return new PaginationAuto<TSource, Tdestination>(results, totalItems, await convertTsourceToTdestinationMethod, page, limit);
		}

		// Non Async Methods

		public static Pagination<TSource> AsPagination<TSource>(this IQueryable<TSource> source, int page, int limit, string sortColumn = "", bool orderByDescending = false)
		{
			ValidateInputs(page, limit);

			var totalItems = source.Count();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = source.Skip((page - 1) * limit).Take(limit);

			return new Pagination<TSource>(results.ToList(), totalItems, page, limit);
		}

		public static Pagination<TSource> AsPagination<TSource>(this DbSet<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, string sortColumn = "", bool orderByDescending = false) where TSource : class
		{
			ValidateInputs(page, limit);

			var totalItems = source.Where(expression).Count();
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = (orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn)));
			}
			else
			{
				results = source.Where(expression).Skip((page - 1) * limit).Take(limit);
			}
			return new Pagination<TSource>(results, totalItems, page, limit);
		}

		public static PaginationAuto<TSource, Tdestination> AsPagination<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Func<TSource, Tdestination> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
		{
			ValidateInputs(page, limit);

			var totalItems = source.Count();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = source.Skip((page - 1) * limit).Take(limit);

			return new PaginationAuto<TSource, Tdestination>(results, totalItems, convertTsourceToTdestinationMethod, page, limit);
		}

		public static PaginationAuto<TSource, Tdestination> AsPagination<TSource, Tdestination>(this DbSet<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, Tdestination> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
		{
			ValidateInputs(page, limit);

			var totalItems = source.Where(expression).Count();
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = (orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn)));
			}
			else
			{
				results = source.Where(expression).Skip((page - 1) * limit).Take(limit);
			}
			return new PaginationAuto<TSource, Tdestination>(results, totalItems, convertTsourceToTdestinationMethod, page, limit);
		}

		private static void ValidateInputs(int page, int limit)
		{
			if (limit <= 0)
			{
				throw new PaginationException("Limit must be greater than 0");
			}
			if (page <= 0)
			{
				throw new PaginationException("Page must be greater than 0");
			}
		}
	}
}