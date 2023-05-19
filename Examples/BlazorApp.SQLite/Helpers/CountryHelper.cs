using BlazorApp.SQLite.DTOs;
using BlazorApp.SQLite.Entities;

namespace BlazorApp.SQLite.Helpers
{
	public class CountryHelper
	{
		public static CountryDto ToDto(Country country)
		{
			return new CountryDto
			{
				DateAdded = country.DateAdded,
				Id = country.Id,
				LastModifiedDate = country.LastModifiedDate,
				Name = country.Name,
			};
		}
	}
}