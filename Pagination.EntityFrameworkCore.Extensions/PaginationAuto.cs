using System;
using System.Collections.Generic;
using System.Linq;

namespace Pagination.EntityFrameworkCore.Extensions
{
	public class PaginationAuto<Tsource, Tdestination>
	{
		public PaginationAuto(Pagination<Tsource> pagination, Func<Tsource, Tdestination> convertTsourceToTdestinationMethod)
		{
			Results = pagination?.Results?.Select(a => convertTsourceToTdestinationMethod(a)) ?? Enumerable.Empty<Tdestination>();
			TotalItems = pagination?.TotalItems ?? 0;
			CurrentPage = pagination?.CurrentPage ?? 1;
			NextPage = pagination?.NextPage;
			Pages = pagination?.Pages ?? Enumerable.Empty<int>();
		}

		public PaginationAuto(IEnumerable<Tsource> results, long totalItems, Func<Tsource, Tdestination> convertTsourceToTdestinationMethod, int page = 1, int limit = 10)
		{
			ValidateInputs(page, limit);

			var startIndex = (page - 1) * limit;
			var endIndex = page * limit;
			var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)limit);

			TotalItems = totalItems;
			CurrentPage = page;
			Results = results?.Select(a => convertTsourceToTdestinationMethod(a)) ?? Enumerable.Empty<Tdestination>();

			if (startIndex > 0)
			{
				PreviousPage = page - 1;
			}
			if (endIndex < totalItems)
			{
				NextPage = page + 1;
			}

			Pages = totalPages > 0 ? Enumerable.Range(1, totalPages) : Enumerable.Empty<int>();
		}

		private static void ValidateInputs(int page, int limit)
		{
			if (limit <= 0)
			{
				throw new PaginationException("Limit must be greater than 0");
			}
			if (page <= 0)
			{
				throw new PaginationException("Page must be greater than 0");
			}
		}

		public long TotalItems { get; private set; }
		public int CurrentPage { get; private set; }
		public int? NextPage { get; private set; }
		public int? PreviousPage { get; private set; }
		public IEnumerable<int> Pages { get; private set; }
		public IEnumerable<Tdestination> Results { get; private set; }
	}
}
