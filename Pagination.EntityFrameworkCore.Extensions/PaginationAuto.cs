using System;
using System.Collections.Generic;
using System.Linq;

namespace Pagination.EntityFrameworkCore.Extensions
{
    [Obsolete("Use static methods Pagination.GetPagination(...) or Pagination.GetPaginationAsync(...)")]
    public class PaginationAuto<TSource, TDestination> : Pagination<TDestination>
    {
        [Obsolete("Use static methods Pagination.GetPagination(...) or Pagination.GetPaginationAsync(...)")]
        public PaginationAuto(Pagination<TSource> pagination, Func<TSource, TDestination> convertTSourceToTDestinationMethod)
        : base(pagination.Results?.Select(x => convertTSourceToTDestinationMethod(x)) ?? new List<TDestination>(), pagination.TotalItems, pagination.CurrentPage, (int)pagination.TotalItems, false)
        {
        }

        [Obsolete("Use static methods Pagination.GetPagination(...) or Pagination.GetPaginationAsync(...)")]
        public PaginationAuto(IEnumerable<TSource> results, long totalItems, Func<TSource, TDestination> convertTSourceToTDestinationMethod, int page = 1, int limit = 10, bool applyPageAndLimitToResults = false)
       : base(results?.Select(x => convertTSourceToTDestinationMethod(x)) ?? new List<TDestination>(), totalItems, page, limit, applyPageAndLimitToResults)
        {
        }
    }
}