# Pagination.EntityFrameworkCore.Extensions


This is a library for List Pagination on Dotnet.
Works well with **Entity Framework** as an extension.

# Get Started

```nuget
Install-Package Pagination.EntityFrameworkCore.Extensions
```

### Sample output on Web API:

```JSON
{
  "totalItems": 5,
  "currentPage": 2,
  "nextPage": 3,
  "previousPage": 1,
  "totalPages": 3,
  "results": [
    {
      "name": "Mali",
      "id": "0ae45b80-9269-4fa7-86b3-e6b294cba322",
      "dateAdded": "2021-09-26T15:31:14.936849+02:00",
      "lastModifiedDate": "2021-09-26T15:31:14.936849+02:00"
    },
    {
      "name": "South Africa",
      "id": "0ae45b80-9269-4fa7-86b3-e6b494cba13e",
      "dateAdded": "2021-03-15T21:02:03.8421801+00:00",
      "lastModifiedDate": "2021-03-15T21:02:03.8422383+00:00"
    }
  ]
}
```

### How to use:
#### Support both asynchronous vs synchronous methods, only "Async" at the end of method name will differentiate.
```C#

// Use only Pagination Model
public async Task<Pagination<Country>> GetCountriesAsync(int page, int limit)
{
	var list  = await _dbContext.Countries.Skip((page - 1) * limit).Take(limit).ToListAsync();
	var totalItems  = await _dbContext.Countries.CountAsync();
	return new Pagination<Country>(list, totalItems, page, limit);

	//OR, there is optional boolean param applyPageAndLimitToResults if you want to apply page and limit to results
	/*
	var list  = await _dbContext.Countries.ToListAsync();
	var totalItems  = await _dbContext.Countries.CountAsync();

	return new Pagination<Country>(list, totalItems, page, limit, applyPageAndLimitToResults: true);
	
	*/
}

// OR use as Entity Framework extension
private async Task<Pagination<Country>> GetAllCountriesAsync(int page, int limit)
{
	using (var context = new CollegeDbContext())
	{
		return await _dbContext.Countries.AsPaginationAsync<Country>(page, limit, sortColumn: "Name", orderByDescending: true);
		// OR return await _dbContext.AsPaginationAsync<Country>(page, limit, sortColumn: "Name", orderByDescending: true);
	}
}

//OR add filter before pagination
private async Task<Pagination<Country>> GetAllCountriesAsync(int page, int limit)
{
	using (var context = new CollegeDbContext())
	{
		return await _dbContext.Countries.Where(x => x.DateAdded > DateTimeOffset.UtcNow.AddDays(-30)).AsPaginationAsync<Country>(page, limit, sortColumn: "Name", orderByDescending: true);
	}
}

//OR with supported filter
private async Task<Pagination<Country>> GetAllCountriesAsync(int page, int limit, string searchText)
{
	using (var context = new CollegeDbContext())
	{
		return await _dbContext.Countries.AsPaginationAsync<Country>(page, limit, x => x.Name.Contains(searchText), sortColumn: "Name", orderByDescending: true);
		// OR return await _dbContext.AsPaginationAsync<Country>(page, limit, x => x.Name.Contains(searchText), sortColumn: "Name", orderByDescending: true);
	}
}
    
    
```

## Auto Mapping or Converting Models:

```C#
//Create a method that will map/convert source model to destination model.
//This method can also be Async.
private CountryViewModel ConverCountryToCountryViewModel(Country country)
{
	return new CountryViewModel
	{
		name = country.Name,
		Id = country.Id
	};
}

// Use only Pagination Model
public async Task<Pagination<CountryViewModel>> GetCountriesAsync(int page, int limit)
{
	var list  = await _dbContext.Countries.Skip((page - 1) * limit).Take(limit).ToListAsync();
	var totalItems  = await _dbContext.Countries.CountAsync();
	//Pass 'ConverCountryToCountryViewModel' method that will map/convert source model to destination model 
	return new Pagination<CountryViewModel>(list, totalItems, ConverCountryToCountryViewModel, page, limit);

	//OR, there is optional boolean param applyPageAndLimitToResults if you want to apply page and limit to results
	/*
	var list  = await _dbContext.Countries.ToListAsync();
	var totalItems  = await _dbContext.Countries.CountAsync();
	return Pagination<Country>.GetPagination<Country, CountryViewModel>(results, totalItems, x => ConverCountryToCountryViewModel(x), page, limit, applyPageAndLimitToResults: true);
	*/
}

// OR use as Entity Framework extension
private async Task<Pagination<CountryViewModel>> GetAllCountriesAsync(int page, int limit)
{
	using (var context = new CollegeDbContext())
	{
		//Pass 'ConverCountryToCountryViewModel' method that will map/convert source model to destination model 
		return await _dbContext.Countries.AsPaginationAsync<Country, CountryViewModel>(page, limit, x => ConverCountryToCountryViewModel(x), sortColumn: "Name", orderByDescending: true);
		//OR return await _dbContext.AsPaginationAsync<Country, CountryViewModel>(page, limit, x => ConverCountryToCountryViewModel(x), sortColumn: "Name", orderByDescending: true);
	}
}

//OR add filter before pagination
private async Task<Pagination<CountryViewModel>> GetAllCountriesAsync(int page, int limit)
{
	using (var context = new CollegeDbContext())
	{
		//Pass 'ConverCountryToCountryViewModel' method that will map/convert source model to destination model 
		return await _dbContext.Countries.Where(x => x.DateAdded > DateTimeOffset.UtcNow.AddDays(-30)).AsPaginationAsync<Country, CountryViewModel>(page, limit, x => ConverCountryToCountryViewModel(x), sortColumn: "Name", orderByDescending: true);
		
	}
}

//OR with supported filter
private async Task<Pagination<CountryViewModel>> GetAllCountriesAsync(int page, int limit, string searchText)
{
	using (var context = new CollegeDbContext())
	{
		//Pass 'ConverCountryToCountryViewModel' method that will map/convert source model to destination model 
		return await _dbContext.Countries.AsPaginationAsync<Country, CountryViewModel>(page, limit, x => x.Name.Contains(searchText), x => ConverCountryToCountryViewModel(x), sortColumn: "Name", orderByDescending: true);
		//OR return await _dbContext.AsPaginationAsync<Country, CountryViewModel>(page, limit, x => x.Name.Contains(searchText), x => ConverCountryToCountryViewModel(x), sortColumn: "Name", orderByDescending: true);
	}
}

// With extra parameter/s
private async Task<CountryViewModel> ConverCountryToCountryViewModelAsync(Country country, Guid userId)
{
//Some async processing like sending audit log somewhere
/*
	await _someLoggingService.LogAsync(userId, country);
*/
	return new CountryViewModel
	{
		name = country.Name,
		Id = country.Id
	};
}
//Then auto map like this:
private async Task<Pagination<CountryViewModel>> GetAllCountriesAsync(int page, int limit, string searchText)
{
	using (var context = new CollegeDbContext())
	{
		//Pass 'ConverCountryToCountryViewModelAsync' method that will map/convert source model to destination model 
		return await _dbContext.Countries.AsPaginationAsync<Country, CountryViewModel>(page, limit, x => x.Name.Contains(searchText), x => ConverCountryToCountryViewModelAsync(x, Guid.Empty), sortColumn: "Name", orderByDescending: true);
		//OR return await _dbContext.AsPaginationAsync<Country, CountryViewModel>(page, limit, x => x.Name.Contains(searchText),  x => ConverCountryToCountryViewModelAsync(x, Guid.Empty), sortColumn: "Name", orderByDescending: true);
	}
}
		
		
```

# NuGet
https://www.nuget.org/packages/Pagination.EntityFrameworkCore.Extensions/
