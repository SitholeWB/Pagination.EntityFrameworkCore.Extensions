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
		//*** DbContext source
		public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this DbContext source, int page, int limit, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await source.Set<TSource>().CountAsync(cancellationToken).ConfigureAwait(false);

			if (!string.IsNullOrEmpty(sortColumn))
			{
				if (orderByDescending)
				{
					var resultsDesc = source.Set<TSource>().OrderByDescending(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit);
					return new Pagination<TSource>(await resultsDesc.ToListAsync(cancellationToken).ConfigureAwait(false), totalItems, page, limit);
				}
				else
				{
					var resultsAsc = source.Set<TSource>().OrderBy(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit);
					return new Pagination<TSource>(await resultsAsc.ToListAsync(cancellationToken).ConfigureAwait(false), totalItems, page, limit);
				}
			}
			var results = source.Set<TSource>().Skip((page - 1) * limit).Take(limit);

			return new Pagination<TSource>(await results.ToListAsync(cancellationToken).ConfigureAwait(false), totalItems, page, limit);
		}

		public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this DbContext dbContext, int page, int limit, Expression<Func<TSource, bool>> expression, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await dbContext.Set<TSource>().Where(expression).CountAsync(cancellationToken).ConfigureAwait(false);
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? dbContext.Set<TSource>().Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : dbContext.Set<TSource>().Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				results = await dbContext.Set<TSource>().Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			return new Pagination<TSource>(results, totalItems, page, limit);
		}

		// PaginationAuto Mapping
		public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this DbContext dbContext, int page, int limit, Func<TSource, TDestination> convertTSourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await dbContext.Set<TSource>().CountAsync(cancellationToken).ConfigureAwait(false);
			if (!string.IsNullOrEmpty(sortColumn))
			{
				if (orderByDescending)
				{
					var resultsDesc = await dbContext.Set<TSource>().OrderByDescending(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
					return Pagination<TSource>.GetPagination(resultsDesc, totalItems, convertTSourceToTDestinationMethod, page, limit);
				}
				else
				{
					var resultsAsc = await dbContext.Set<TSource>().OrderBy(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
					return Pagination<TSource>.GetPagination<TSource, TDestination>(resultsAsc, totalItems, convertTSourceToTDestinationMethod, page, limit);
				}
			}
			var results = await dbContext.Set<TSource>().Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			return Pagination<TSource>.GetPagination(results, totalItems, convertTSourceToTDestinationMethod, page, limit);
		}

		public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this DbContext dbContext, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, TDestination> convertTSourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await dbContext.Set<TSource>().Where(expression).CountAsync(cancellationToken).ConfigureAwait(false);
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? dbContext.Set<TSource>().Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : dbContext.Set<TSource>().Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				results = await dbContext.Set<TSource>().Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			return Pagination<TSource>.GetPagination(results, totalItems, convertTSourceToTDestinationMethod, page, limit);
		}

		// PaginationAuto Async Mapping
		public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this DbContext dbContext, int page, int limit, Func<TSource, Task<TDestination>> convertTSourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);
			var totalItems = await dbContext.Set<TSource>().CountAsync(cancellationToken).ConfigureAwait(false);

			if (!string.IsNullOrEmpty(sortColumn))
			{
				if (orderByDescending)
				{
					var resultsDesc = await dbContext.Set<TSource>().OrderByDescending(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
					return await Pagination<TSource>.GetPaginationAsync(resultsDesc, totalItems, convertTSourceToTDestinationMethod, page, limit);
				}
				else
				{
					var resultsAsc = await dbContext.Set<TSource>().OrderBy(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
					return await Pagination<TSource>.GetPaginationAsync(resultsAsc, totalItems, convertTSourceToTDestinationMethod, page, limit);
				}
			}
			var results = await dbContext.Set<TSource>().Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);

			return await Pagination<TSource>.GetPaginationAsync(results, totalItems, convertTSourceToTDestinationMethod, page, limit);
		}

		public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this DbContext dbContext, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, Task<TDestination>> convertTSourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var totalItems = await dbContext.Set<TSource>().Where(expression).CountAsync(cancellationToken).ConfigureAwait(false);
			var results = Enumerable.Empty<TSource>();
			if (!string.IsNullOrEmpty(sortColumn))
			{
				results = await (orderByDescending ? dbContext.Set<TSource>().Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : dbContext.Set<TSource>().Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				results = await dbContext.Set<TSource>().Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			return await Pagination<TSource>.GetPaginationAsync(results, totalItems, convertTSourceToTDestinationMethod, page, limit);
		}
	}
}