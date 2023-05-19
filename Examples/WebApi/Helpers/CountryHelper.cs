using WebApi.DTOs;
using WebApi.Entities;

namespace WebApi.Helpers
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