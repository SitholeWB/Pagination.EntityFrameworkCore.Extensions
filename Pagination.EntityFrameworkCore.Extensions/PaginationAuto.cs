using System;
using System.Collections.Generic;
using System.Linq;

namespace Pagination.EntityFrameworkCore.Extensions
{
    public class PaginationAuto<Tsource, Tdestination> : Pagination<Tdestination>
    {
        public PaginationAuto(Pagination<Tsource> pagination, Func<Tsource, Tdestination> convertTsourceToTdestinationMethod)
        : base(pagination.Results?.Select(x => convertTsourceToTdestinationMethod(x)) ?? new List<Tdestination>(), pagination.TotalItems, pagination.CurrentPage, (int)pagination.TotalItems, false)
        {
            Results = pagination?.Results?.Select(a => convertTsourceToTdestinationMethod(a)) ?? Enumerable.Empty<Tdestination>();
            TotalItems = pagination?.TotalItems ?? 0;
            CurrentPage = pagination?.CurrentPage ?? 1;
            NextPage = pagination?.NextPage;
            TotalPages = pagination?.TotalPages ?? 0;
        }

        public PaginationAuto(IEnumerable<Tsource> results, long totalItems, Func<Tsource, Tdestination> convertTsourceToTdestinationMethod, int page = 1, int limit = 10, bool applyPageAndLimitToResults = false)
       : base(results?.Select(x => convertTsourceToTdestinationMethod(x)) ?? new List<Tdestination>(), totalItems, page, limit, applyPageAndLimitToResults)
        {
            PaginationExtensionsHelper.ValidateInputs(page, limit);

            var startIndex = (page - 1) * limit;
            var endIndex = page * limit;

            TotalItems = totalItems;
            CurrentPage = page;
            Results = results?.Select(a => convertTsourceToTdestinationMethod(a)) ?? Enumerable.Empty<Tdestination>();
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
        public IEnumerable<Tdestination> Results { get; private set; }
    }
}