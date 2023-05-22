using System;
using System.Collections.Generic;
using System.Linq;

namespace Pagination.EntityFrameworkCore.Extensions
{
    [Obsolete("Simplify your code by using Pagination")]
    public class PaginationAuto<Tsource, TDestination> : Pagination<TDestination>
    {
        [Obsolete("Simplify your code by using Pagination")]
        public PaginationAuto(Pagination<Tsource> pagination, Func<Tsource, TDestination> convertTsourceToTDestinationMethod)
        : base(pagination.Results?.Select(x => convertTsourceToTDestinationMethod(x)) ?? new List<TDestination>(), pagination.TotalItems, pagination.CurrentPage, (int)pagination.TotalItems, false)
        {
        }

        [Obsolete("Simplify your code by using Pagination")]
        public PaginationAuto(IEnumerable<Tsource> results, long totalItems, Func<Tsource, TDestination> convertTsourceToTDestinationMethod, int page = 1, int limit = 10, bool applyPageAndLimitToResults = false)
       : base(results?.Select(x => convertTsourceToTDestinationMethod(x)) ?? new List<TDestination>(), totalItems, page, limit, applyPageAndLimitToResults)
        {
        }
    }
}