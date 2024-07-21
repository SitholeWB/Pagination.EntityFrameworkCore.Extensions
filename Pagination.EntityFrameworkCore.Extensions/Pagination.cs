using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pagination.EntityFrameworkCore.Extensions
{
    public class Pagination<T>
    {
        public long TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int? NextPage { get; set; }
        public int? PreviousPage { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Results { get; set; }

        public Pagination()
        {
            Results = new List<T>();
            CurrentPage = 1;
        }

        public Pagination(Pagination<T> pagination)
        {
            TotalItems = pagination.TotalPages;
            CurrentPage = pagination.CurrentPage;
            NextPage = pagination.NextPage;
            PreviousPage = pagination.PreviousPage;
            TotalPages = pagination.TotalPages;
            Results = pagination.Results;
        }

        public Pagination(IEnumerable<T> results, long totalItems, int page = 1, int limit = 10, bool applyPageAndLimitToResults = false)
        {
            if (page <= 0)
            {
                throw new PaginationException("Page must be greater than 0");
            }

            var startIndex = (page - 1) * limit;
            var endIndex = page * limit;

            TotalItems = totalItems;
            CurrentPage = page;
            Results = results ?? Enumerable.Empty<T>();
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

        public static Pagination<TDestination> GetPagination<TSource, TDestination>(IEnumerable<TSource> results, long totalItems, Func<TSource, TDestination> convertTSourceToTDestinationMethod, int page = 1, int limit = 10, bool applyPageAndLimitToResults = false)
        {
            var destinationResults = results?.Select(x => convertTSourceToTDestinationMethod(x)) ?? new List<TDestination>();
            return new Pagination<TDestination>(destinationResults, totalItems, page, limit, applyPageAndLimitToResults);
        }

        public static async Task<Pagination<TDestination>> GetPaginationAsync<TSource, TDestination>(IEnumerable<TSource> results, long totalItems, Func<TSource, Task<TDestination>> convertTSourceToTDestinationMethod, int page = 1, int limit = 10, bool applyPageAndLimitToResults = false)
        {
            if (results == null)
            {
                return new Pagination<TDestination>(new List<TDestination>(), totalItems, page, limit, applyPageAndLimitToResults);
            }
            var destinationResults = await results.Select(async ev => await convertTSourceToTDestinationMethod(ev)).WhenAll().ConfigureAwait(false);
            return new Pagination<TDestination>(destinationResults, totalItems, page, limit, applyPageAndLimitToResults);
        }
    }
}