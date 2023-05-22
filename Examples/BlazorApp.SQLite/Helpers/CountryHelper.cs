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

        public static async Task<CountryDto> ToDtoAsync(Country country)
        {
            var seconds = new Random().Next(0, 2);
            await Task.Delay(TimeSpan.FromSeconds(seconds));
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