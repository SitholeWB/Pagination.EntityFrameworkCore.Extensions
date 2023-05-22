using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pagination.EntityFrameworkCore.Extensions
{
    public static partial class PaginationExtensions
    {
        public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this IQueryable<TSource> source, int page, int limit, string sortColumn = "", bool orderByDescending = false)
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await source.CountAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(sortColumn))
            {
                source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
            }
            var results = source.Skip((page - 1) * limit).Take(limit);

            return new Pagination<TSource>(await results.ToListAsync().ConfigureAwait(false), totalItems, page, limit);
        }

        public static async Task<Pagination<TSource>> AsPaginationAsync<TSource>(this IQueryable<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, string sortColumn = "", bool orderByDescending = false)
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await source.Where(expression).CountAsync().ConfigureAwait(false);
            var results = Enumerable.Empty<TSource>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                results = await (orderByDescending ? source.Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            return new Pagination<TSource>(results, totalItems, page, limit);
        }

        // PaginationAuto Mapping
        public static async Task<PaginationAuto<TSource, Tdestination>> AsPaginationAsync<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Func<TSource, Tdestination> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await source.CountAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(sortColumn))
            {
                source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
            }
            var results = source.Skip((page - 1) * limit).Take(limit);

            return new PaginationAuto<TSource, Tdestination>(await results.ToListAsync().ConfigureAwait(false), totalItems, convertTsourceToTdestinationMethod, page, limit);
        }

        public static async Task<PaginationAuto<TSource, Tdestination>> AsPaginationAsync<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, Tdestination> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await source.Where(expression).CountAsync().ConfigureAwait(false);
            var results = Enumerable.Empty<TSource>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                results = await (orderByDescending ? source.Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            return new PaginationAuto<TSource, Tdestination>(results, totalItems, convertTsourceToTdestinationMethod, page, limit);
        }

        // PaginationAuto Async Mapping
        public static async Task<Pagination<Tdestination>> AsPaginationAsync<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Func<TSource, Task<Tdestination>> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await source.CountAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(sortColumn))
            {
                source = orderByDescending ? source.OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.OrderBy(p => EF.Property<object>(p, sortColumn));
            }
            var results = await source.Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            var destination = await results.Select(async ev => await convertTsourceToTdestinationMethod(ev)).WhenAll();
            return new Pagination<Tdestination>(destination, totalItems, page, limit);
        }

        public static async Task<Pagination<Tdestination>> AsPaginationAsync<TSource, Tdestination>(this IQueryable<TSource> source, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, Task<Tdestination>> convertTsourceToTdestinationMethod, string sortColumn = "", bool orderByDescending = false)
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = await source.Where(expression).CountAsync().ConfigureAwait(false);
            var results = Enumerable.Empty<TSource>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                results = await (orderByDescending ? source.Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            else
            {
                results = await source.Where(expression).Skip((page - 1) * limit).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            var destination = await results.Select(async ev => await convertTsourceToTdestinationMethod(ev)).WhenAll();
            return new Pagination<Tdestination>(destination, totalItems, page, limit);
        }
    }
}