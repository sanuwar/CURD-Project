using DotNetWebAPI.Data;
using DotNetWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatorController:ControllerBase
    {
        DataContextDapper _dapper;
        public DatorController (IConfiguration config)
        {
            _dapper = new DataContextDapper (config);
        }

        [HttpGet("GetDators")]
        public IEnumerable<Dator>GetDators()
        {
            string sql = @"SELECT 
                [DatorId],
		        [FirstName],
		        [LastName],
		        [Email],
                [Gender]
	        FROM CountryDataSchema.Dator";
          
          IEnumerable<Dator>dators = _dapper.LoadData<Dator>(sql);

          return dators;
        }

        [HttpGet("GetSingleDator/{datorId}")]

        public Dator GetSingleDator(int datorId)
        {
            string sql = @"SELECT 
            [DatorId],
		    [FirstName],
		    [LastName],
		    [Email],
            [Gender]
        FROM CountryDataSchema.Dator
        WHERE DatorId=" + datorId.ToString();

        Dator dator = _dapper.LoadDataSingle<Dator>(sql);
        return dator;
        }

        [HttpPut("EditDator")]
        public IActionResult EditDator(Dator dator)
        {
            string sql = @"
                UPDATE CountryDataSchema.Dator
		            SET 
		            [FirstName] = '" + dator.FirstName +
		            "',  [LastName]= '" + dator.LastName +
		            "',  [Email]= '"+ dator.Email +
                    "',  [Gender]= '" + dator.Gender + "'"+
		        " WHERE DatorId = " + dator.DatorId.ToString();

            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();

            } throw new Exception("Failed to update Dator");
        } 

        [HttpPost("AddDator")]

        public IActionResult AddDator(Dator dator)
        {
            string sql = @"INSERT INTO CountryDataSchema.Dator (
	        [FirstName],
	        [LastName],
	        [Email],
            [Gender] 
            ) VALUES (" +
            "'" + dator.FirstName +
	        "'," + "'" + dator.LastName +
	        "','" + dator.Email +
            "','" + dator.Gender +
            "')";
        
        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
            {
                return Ok();

            } throw new Exception("Failed to update CountryList");
        }       
    }
}