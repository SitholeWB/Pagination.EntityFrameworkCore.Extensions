using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Pagination.EntityFrameworkCore.Extensions
{
    //Non Async Methods
    public static partial class PaginationExtensions
    {
        public static Pagination<TSource> AsPagination<TSource>(this DbContext dbContext, int page, int limit, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = dbContext.Set<TSource>().Count();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (orderByDescending)
                {
                    var resultsDesc = dbContext.Set<TSource>().OrderByDescending(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit);
                    return new Pagination<TSource>(resultsDesc.ToList(), totalItems, page, limit);
                }
                else
                {
                    var resultsAsc = dbContext.Set<TSource>().OrderBy(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit);
                    return new Pagination<TSource>(resultsAsc.ToList(), totalItems, page, limit);
                }
            }
            var results = dbContext.Set<TSource>().Skip((page - 1) * limit).Take(limit);

            return new Pagination<TSource>(results.ToList(), totalItems, page, limit);
        }

        public static Pagination<TSource> AsPagination<TSource>(this DbContext dbContext, int page, int limit, Expression<Func<TSource, bool>> expression, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = dbContext.Set<TSource>().Where(expression).Count();
            var results = Enumerable.Empty<TSource>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                results = (orderByDescending ? dbContext.Set<TSource>().Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : dbContext.Set<TSource>().Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit);
            }
            else
            {
                results = dbContext.Set<TSource>().Where(expression).Skip((page - 1) * limit).Take(limit);
            }
            return new Pagination<TSource>(results, totalItems, page, limit);
        }

        public static PaginationAuto<TSource, TDestination> AsPagination<TSource, TDestination>(this DbContext dbContext, int page, int limit, Func<TSource, TDestination> convertTsourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = dbContext.Set<TSource>().Count();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (orderByDescending)
                {
                    var resultsDesc = dbContext.Set<TSource>().OrderByDescending(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit);
                    return new PaginationAuto<TSource, TDestination>(resultsDesc, totalItems, convertTsourceToTDestinationMethod, page, limit);
                }
                else
                {
                    var resultsAsc = dbContext.Set<TSource>().OrderBy(p => EF.Property<object>(p, sortColumn)).Skip((page - 1) * limit).Take(limit);
                    return new PaginationAuto<TSource, TDestination>(resultsAsc, totalItems, convertTsourceToTDestinationMethod, page, limit);
                }
            }

            var results = dbContext.Set<TSource>().Skip((page - 1) * limit).Take(limit);

            return new PaginationAuto<TSource, TDestination>(results, totalItems, convertTsourceToTDestinationMethod, page, limit);
        }

        public static PaginationAuto<TSource, TDestination> AsPagination<TSource, TDestination>(this DbContext source, int page, int limit, Expression<Func<TSource, bool>> expression, Func<TSource, TDestination> convertTsourceToTDestinationMethod, string sortColumn = "", bool orderByDescending = false) where TSource : class
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var totalItems = source.Set<TSource>().Where(expression).Count();
            var results = Enumerable.Empty<TSource>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                results = (orderByDescending ? source.Set<TSource>().Where(expression).OrderByDescending(p => EF.Property<object>(p, sortColumn)) : source.Set<TSource>().Where(expression).OrderBy(p => EF.Property<object>(p, sortColumn))).Skip((page - 1) * limit).Take(limit);
            }
            else
            {
                results = source.Set<TSource>().Where(expression).Skip((page - 1) * limit).Take(limit);
            }
            return new PaginationAuto<TSource, TDestination>(results, totalItems, convertTsourceToTDestinationMethod, page, limit);
        }
    }
}