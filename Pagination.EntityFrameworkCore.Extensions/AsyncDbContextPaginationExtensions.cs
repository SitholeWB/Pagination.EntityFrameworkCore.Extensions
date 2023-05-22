using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pagination.EntityFrameworkCore.Extensions
{
    public static partial class PaginationExtensions
    {
        //*** DbContext source
        public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this DbContext source, int page, int limit, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await source.Set<TSource>().CountAsync().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (orderByDescending)
                {
                    var resultsDesc = source.Set<TSource>().OrderByDescending(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit);
                    return new Pagination<TSource>(await resultsDesc.ToListAsync().ConfigureAwait(false), totalItems, page, limit);
                }
                else
                {
                    var resultsAsc = source.Set<TSource>().OrderBy(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit);
                    return new Pagination<TSource>(await resultsAsc.ToListAsync().ConfigureAwait(false), totalItems, page, limit);
                }
            }
            var results = source.Set<TSource>().Skip((page - 1) * limit).Take(limit);

            return new Pagination<TSource>(await results.ToListAsync().ConfigureAwait(false), totalItems, page, limit);
        }

        public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this DbContext dbContext, int page, int limit, Expression<Func<TSource, bool>> expression, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await dbContext.Set<TSource>().Where(expression).CountAsync().ConfigureAwait(false);
            var results = Enumerable.Empty<TSource>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                results = await (orderByDescending ? dbContext.Set<TSource>().Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : dbContext.Set<TSource>().Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                results = await dbContext.Set<TSource>().Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            return new Pagination<TSource>(results, totalItems, page, limit);
        }

        // PaginationAuto Mapping
        public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this DbContext dbContext, int page, int limit, Func<TSource, TDestination> convertTsourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await dbContext.Set<TSource>().CountAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (orderByDescending)
                {
                    var resultsDesc = await dbContext.Set<TSource>().OrderByDescending(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
                    return new Pagination<TDestination>(resultsDesc?.Select(a => convertTsourceToTDestinationMethod(a)) ?? Enumerable.Empty<TDestination>(), totalItems, page, limit);
                }
                else
                {
                    var resultsAsc = await dbContext.Set<TSource>().OrderBy(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
                    return new Pagination<TDestination>(resultsAsc?.Select(a => convertTsourceToTDestinationMethod(a)) ?? Enumerable.Empty<TDestination>(), totalItems, page, limit);
                }
            }
            var results = await dbContext.Set<TSource>().Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            return new Pagination<TDestination>(results?.Select(a => convertTsourceToTDestinationMethod(a)) ?? Enumerable.Empty<TDestination>(), totalItems, page, limit);
        }

        public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this DbContext dbContext, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, TDestination> convertTsourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await dbContext.Set<TSource>().Where(expression).CountAsync().ConfigureAwait(false);
            var results = Enumerable.Empty<TSource>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                results = await (orderByDescending ? dbContext.Set<TSource>().Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : dbContext.Set<TSource>().Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                results = await dbContext.Set<TSource>().Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            return new Pagination<TDestination>(results?.Select(a => convertTsourceToTDestinationMethod(a)) ?? Enumerable.Empty<TDestination>(), totalItems, page, limit);
        }

        // PaginationAuto Async Mapping
        public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this DbContext dbContext, int page, int limit, Func<TSource, Task<TDestination>> convertTsourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);
            var totalItems = await dbContext.Set<TSource>().CountAsync().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (orderByDescending)
                {
                    var resultsDesc = await dbContext.Set<TSource>().OrderByDescending(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
                    var destinationDesc = await resultsDesc.Select(async ev => await convertTsourceToTDestinationMethod(ev)).WhenAll().ConfigureAwait(false);
                    return new Pagination<TDestination>(destinationDesc, totalItems, page, limit);
                }
                else
                {
                    var resultsAsc = await dbContext.Set<TSource>().OrderBy(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
                    var destinationAsc = await resultsAsc.Select(async ev => await convertTsourceToTDestinationMethod(ev)).WhenAll().ConfigureAwait(false);
                    return new Pagination<TDestination>(destinationAsc, totalItems, page, limit);
                }
            }
            var results = await dbContext.Set<TSource>().Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);

            var destination = await results.Select(async ev => await convertTsourceToTDestinationMethod(ev)).WhenAll().ConfigureAwait(false);
            return new Pagination<TDestination>(destination, totalItems, page, limit);
        }

        public static async Task<Pagination<TDestination>> AsPaginationAsync<TSource, TDestination>(this DbContext dbContext, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, Task<TDestination>> convertTsourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await dbContext.Set<TSource>().Where(expression).CountAsync().ConfigureAwait(false);
            var results = Enumerable.Empty<TSource>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                results = await (orderByDescending ? dbContext.Set<TSource>().Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : dbContext.Set<TSource>().Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                results = await dbContext.Set<TSource>().Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            var destination = await results.Select(async ev => await convertTsourceToTDestinationMethod(ev)).WhenAll().ConfigureAwait(false);
            return new Pagination<TDestination>(destination, totalItems, page, limit);
        }
    }
}