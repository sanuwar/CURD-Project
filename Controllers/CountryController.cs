using DotNetWebAPI.Data;
using DotNetWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CountryController: ControllerBase
{
    DataContextDapper _dapper;

    public CountryController (IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetCountries")]

    public IEnumerable<Country>GetCountries()
    {
        string sql = @"SELECT 
        [CountryId],
		[CountryName],
		[IsDemocracy],
		[PopulationInMillion]
	FROM CountryDataSchema.CountryList";

        IEnumerable<Country>countries= _dapper.LoadData<Country>(sql);
        return countries;
    }
     
    [HttpGet("GetSingleCountry/{countryId}")]

    public Country GetSingleCountry(int countryId)
    {
        string sql = @"SELECT 
            [CountryId],
		    [CountryName],
		    [IsDemocracy],
		    [PopulationInMillion]
        FROM CountryDataSchema.CountryList
        WHERE CountryId=" + countryId.ToString();
        
        Country country = _dapper.LoadDataSingle<Country>(sql);
        return country;
    }

    [HttpPut("EditCounty")]
    public IActionResult EditCountry(Country country)
    {
        string sql = @"
        UPDATE CountryDataSchema.CountryList
		    SET 
		        [CountryName] = '" + country.CountryName +
		        "',  [IsDemocracy]= '" + country.IsDemocracy +
		        "',  [PopulationInMillion]="+ country.PopulationInMillion +
		    " WHERE CountryId = " + country.CountryId;

            

            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();

            } throw new Exception("Failed to update CountryList");
        
    }

    [HttpPost("AddCountry")]
    public IActionResult AddCountry(Country country)
    {
        string sql = @"INSERT INTO CountryDataSchema.CountryList (
	        [CountryName],
	        [IsDemocracy],
	        [PopulationInMillion] 
            ) VALUES (" +
            "'" + country.CountryName +
	        "'," + "'" + country.IsDemocracy +
	        "'," + country.PopulationInMillion +
            ")";
        
        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
            {
                return Ok();

            } throw new Exception("Failed to update CountryList");
    }
    
}

