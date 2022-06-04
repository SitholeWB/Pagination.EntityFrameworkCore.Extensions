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
			TotalPages = pagination?.TotalPages ?? 0;
		}

		public PaginationAuto(IEnumerable<Tsource> results, long totalItems, Func<Tsource, Tdestination> convertTsourceToTdestinationMethod, int page = 1, int limit = 10)
		{
			PaginationExtensionsHelper.ValidateInputs(page, limit);

			var startIndex = (page - 1) * limit;
			var endIndex = page * limit;

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

			TotalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)limit);
		}

		public long TotalItems { get; private set; }
		public int CurrentPage { get; private set; }
		public int? NextPage { get; private set; }
		public int? PreviousPage { get; private set; }
		public int TotalPages { get; private set; }
		public IEnumerable<Tdestination> Results { get; private set; }
	}
}