using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly WebApiContext _context;

        public CountriesController(WebApiContext context)
        {
            _context = context;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<Pagination<CountryDto>>> GetCountries(int page = 1, int limit = 10)
        {
            if (_context.Country == null)
            {
                return NotFound();
            }
            return await _context.Country.AsPaginationAsync<Country, CountryDto>(page, limit, CountryHelper.ToDto);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(Guid id)
        {
            if (_context.Country == null)
            {
                return NotFound();
            }
            var country = await _context.Country.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        // PUT: api/Countries/5 To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(Guid id, UpdateCountryModel country)
        {
            if (string.IsNullOrWhiteSpace(country?.Name))
            {
                return BadRequest("Country name is required.");
            }
            var entity = await _context.Country.FindAsync(id);
            if (entity == null)
            {
                return NotFound("Id not found.");
            }
            entity.Name = country.Name;
            entity.LastModifiedDate = DateTimeOffset.UtcNow;
            _context.Update(entity);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Countries To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(AddCountryModel country)
        {
            if (string.IsNullOrWhiteSpace(country?.Name))
            {
                return BadRequest("Country name is required.");
            }

            if (_context.Country == null)
            {
                return Problem("Entity set 'WebApiContext.Country'  is null.");
            }
            var entity = _context.Country.Add(new Country
            {
                Name = country.Name,
                DateAdded = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                LastModifiedDate = DateTimeOffset.UtcNow,
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = entity.Entity.Id }, entity.Entity);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            if (_context.Country == null)
            {
                return NotFound();
            }
            var country = await _context.Country.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Country.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}