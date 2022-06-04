using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Pagination.EntityFrameworkCore.Extensions
{
	//Non Async Methods
	public static partial class PaginationExtensions
	{
		public static Pagination<TSource> AsPagination<TSource>(this IQueryable<TSource> source, int page, int limit, string sortColumn = "", bool orderByDescending = false)
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = source.Count();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = source.Skip((page - 1) * limit).Take(limit);

			return new Pagination<TSource>(results.ToList(), totalItems, page, limit);
		}

		public static Pagination<TSource> AsPagination<TSource>(this IQueryable<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, string sortColumn = "", bool orderByDescending = false)
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = source.Where(expression).Count();
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = (orderByDescending ? source.Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit);
			}
			else
			{
				results = source.Where(expression).Skip((page - 1) * limit).Take(limit);
			}
			return new Pagination<TSource>(results, totalItems, page, limit);
		}

		public static PaginationAuto<TSource, Tdestination> AsPagination<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Func<TSource, Tdestination> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = source.Count();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}

			var results = source.Skip((page - 1) * limit).Take(limit);

			return new PaginationAuto<TSource, Tdestination>(results, totalItems, convertTsourceToTdestinationMethod, page, limit);
		}

		public static PaginationAuto<TSource, Tdestination> AsPagination<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, Tdestination> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = source.Where(expression).Count();
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = (orderByDescending ? source.Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit);
			}
			else
			{
				results = source.Where(expression).Skip((page - 1) * limit).Take(limit);
			}
			return new PaginationAuto<TSource, Tdestination>(results, totalItems, convertTsourceToTdestinationMethod, page, limit);
		}
	}
}