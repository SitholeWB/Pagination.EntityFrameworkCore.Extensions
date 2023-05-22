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
            Results = pagination?.Results?.Select(a => convertTsourceToTDestinationMethod(a)) ?? Enumerable.Empty<TDestination>();
            TotalItems = pagination?.TotalItems ?? 0;
            CurrentPage = pagination?.CurrentPage ?? 1;
            NextPage = pagination?.NextPage;
            TotalPages = pagination?.TotalPages ?? 0;
        }

        [Obsolete("Simplify your code by using Pagination")]
        public PaginationAuto(IEnumerable<Tsource> results, long totalItems, Func<Tsource, TDestination> convertTsourceToTDestinationMethod, int page = 1, int limit = 10, bool applyPageAndLimitToResults = false)
       : base(results?.Select(x => convertTsourceToTDestinationMethod(x)) ?? new List<TDestination>(), totalItems, page, limit, applyPageAndLimitToResults)
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var startIndex = (page - 1) * limit;
            var endIndex = page * limit;

            TotalItems = totalItems;
            CurrentPage = page;
            Results = results?.Select(a => convertTsourceToTDestinationMethod(a)) ?? Enumerable.Empty<TDestination>();
            if (applyPageAndLimitToResults)
            {
                Results = Results.Skip(startIndex).Take(limit);
            }
            if (startIndex > 0)
            {
                PreviousPage = page - 1;
            }
            if (endIndex < totalItems)
            {
                NextPage = page + 1;
            }

            TotalPages = limit > 0 ? (int)Math.Ceiling((decimal)totalItems / (decimal)limit) : 0;
        }

        public long TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int? NextPage { get; private set; }
        public int? PreviousPage { get; private set; }
        public int TotalPages { get; private set; }
        public IEnumerable<TDestination> Results { get; private set; }
    }
}