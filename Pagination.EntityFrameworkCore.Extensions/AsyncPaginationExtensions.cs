using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pagination.EntityFrameworkCore.Extensions
{
	public static partial class PaginationExtensions
	{
		public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this IQueryable<TSource> source, int page, int limit, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await source.CountAsync(cancellationToken).ConfigureAwait(false);
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = source.Skip((page - 1) * limit).Take(limit);

			return new Pagination<TSource>(await results.ToListAsync(cancellationToken).ConfigureAwait(false), totalItems, page, limit);
		}

		public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this IQueryable<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await source.Where(expression).CountAsync(cancellationToken).ConfigureAwait(false);
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? source.Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			return new Pagination<TSource>(results, totalItems, page, limit);
		}

		// PaginationAuto Mapping
		public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this IQueryable<TSource> source, int page, int limit, Func<TSource, TDestination> convertTSourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await source.CountAsync(cancellationToken).ConfigureAwait(false);
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = await source.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);

			return Pagination<TSource>.GetPagination(results, totalItems, convertTSourceToTDestinationMethod, page, limit);
		}

		public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this IQueryable<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, TDestination> convertTSourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await source.Where(expression).CountAsync(cancellationToken).ConfigureAwait(false);
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? source.Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			return Pagination<TSource>.GetPagination(results, totalItems, convertTSourceToTDestinationMethod, page, limit);
		}

		// PaginationAuto Async Mapping
		public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this IQueryable<TSource> source, int page, int limit, Func<TSource, Task<TDestination>> convertTSourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await source.CountAsync(cancellationToken).ConfigureAwait(false);
			if (!string.IsNullOrEmpty(sortColumn))
			{
				source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
			}
			var results = await source.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			return await Pagination<TSource>.GetPaginationAsync(results, totalItems, convertTSourceToTDestinationMethod, page, limit);
		}

		public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this IQueryable<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, Task<TDestination>> convertTSourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await source.Where(expression).CountAsync(cancellationToken).ConfigureAwait(false);
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? source.Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			return await Pagination<TSource>.GetPaginationAsync(results, totalItems, convertTSourceToTDestinationMethod, page, limit);
		}
	}
}