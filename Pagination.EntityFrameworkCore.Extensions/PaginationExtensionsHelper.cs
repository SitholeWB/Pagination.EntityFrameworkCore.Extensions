using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Pagination.EntityFrameworkCore.Extensions
{
	public static class PaginationExtensionsHelper
	{
		public static void ValidateInputs(int page, int limit)
		{
			if (page <= 0)
			{
				throw new PaginationException("Page must be greater than 0");
			}
		}

		public static string GetAnySortField(Type type)
		{
			var allProperties = type.GetProperties();

			var keyProperty = allProperties.SingleOrDefault(p => p.IsDefined(typeof(KeyAttribute), true));
			if (keyProperty == null)
			{
				keyProperty = allProperties.SingleOrDefault(p => p.PropertyType.IsVisible && (p.PropertyType.IsValueType || p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTimeOffset)) && !p.IsDefined(typeof(NotMappedAttribute), true));
			}

			return keyProperty?.Name ?? string.Empty;
		}
	}
}