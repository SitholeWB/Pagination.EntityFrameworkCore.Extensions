using System;
using System.Collections.Generic;

namespace Pagination.EntityFrameworkCore.Extensions
{
    [Obsolete("Use static methods Pagination.GetPagination(...) or Pagination.GetPaginationAsync(...)")]
    public class PaginationAuto<TSource, TDestination> : Pagination<TDestination>
    {
        [Obsolete("Use static methods Pagination.GetPagination(...) or Pagination.GetPaginationAsync(...)")]
        public PaginationAuto(Pagination<TSource> pagination, Func<TSource, TDestination> convertTSourceToTDestinationMethod)
        : base(GetPagination(pagination.Results, pagination.TotalItems, convertTSourceToTDestinationMethod, pagination.CurrentPage, (int)pagination.TotalItems, false))
        {
        }

        [Obsolete("Use static methods Pagination.GetPagination(...) or Pagination.GetPaginationAsync(...)")]
        public PaginationAuto(IEnumerable<TSource> results, long totalItems, Func<TSource, TDestination> convertTSourceToTDestinationMethod, int page = 1, int limit = 10, bool applyPageAndLimitToResults = false)
       : base(GetPagination(results, totalItems, convertTSourceToTDestinationMethod, page, limit, applyPageAndLimitToResults))
        {
        }
    }
}