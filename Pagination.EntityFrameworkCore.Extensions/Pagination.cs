﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Pagination.EntityFrameworkCore.Extensions
{
	public class Pagination<T>
	{
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

		public long TotalItems { get; private set; }
		public int CurrentPage { get; private set; }
		public int? NextPage { get; private set; }
		public int? PreviousPage { get; private set; }
		public int TotalPages { get; private set; }
		public IEnumerable<T> Results { get; private set; }
	}
}