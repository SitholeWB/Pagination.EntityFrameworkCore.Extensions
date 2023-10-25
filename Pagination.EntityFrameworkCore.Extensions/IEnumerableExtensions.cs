using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pagination.EntityFrameworkCore.Extensions
{
	public static class IEnumerableExtensions
	{
		public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
		{
			return await Task.WhenAll(tasks);
		}
	}
}