using BlazorApp.SQLite.Data;
using BlazorApp.SQLite.DTOs;
using BlazorApp.SQLite.Entities;
using BlazorApp.SQLite.Helpers;
using BlazorApp.SQLite.Models;
using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;

namespace BlazorApp.SQLite.Services
{
    public class CountryService
    {
        private readonly BlazorAppSQLiteContext _context;

        public CountryService(BlazorAppSQLiteContext context)
        {
            _context = context;
        }

        public async Task<Country> AddCountryAsync(AddCountryModel country)
        {
            if (string.IsNullOrWhiteSpace(country?.Name))
            {
                throw new ArgumentException("Country name is required.");
            }

            var entity = _context.Country.Add(new Country
            {
                Name = country.Name,
                DateAdded = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                LastModifiedDate = DateTimeOffset.UtcNow,
            });
            await _context.SaveChangesAsync();
            return entity.Entity;
        }

        public async Task<Country> UpdateCountryAsync(Guid id, UpdateCountryModel country)
        {
            if (string.IsNullOrWhiteSpace(country?.Name))
            {
                throw new ArgumentException("Country name is required.");
            }

            var entity = await _context.Country.FindAsync(id);
            if (entity == null)
            {
                throw new ArgumentException("Id not found.");
            }
            entity.Name = country.Name;
            entity.LastModifiedDate = DateTimeOffset.UtcNow;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteCountryAsync(Guid id)
        {
            var entity = await _context.Country.FindAsync(id);
            if (entity == null)
            {
                throw new ArgumentException("Id not found.");
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Pagination<CountryDto>> GetCountries(int page = 1, int limit = 10)
        {
            return await _context.Country.AsPaginationAsync<Country, CountryDto>(page, limit, CountryHelper.ToDto);
        }

        public async Task<Country?> GetCountry(Guid id)
        {
            return await _context.Country.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}