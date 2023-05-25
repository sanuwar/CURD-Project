using DotNetWebAPI.Data;
using DotNetWebAPI.Dtos;
using DotNetWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class PostCountryInfoController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostCountryInfoController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("PostCountryInfo")]

        public IEnumerable<PostCountryInfo> GetPostCountryInfo()
        {
            string sql = @"SELECT 
                [CountryId],
                [DatorId],
		        [CountryName],
		        [IsDemocracy],
		        [PopulationInMillion],
                [DataCreated],
                [DataUpdated]
	        FROM CountryDataSchema.PostCountryInfo";

            IEnumerable<PostCountryInfo>getPostCountryInfo = _dapper.LoadData<PostCountryInfo>(sql);
            return getPostCountryInfo;
        }

        [HttpGet("PostCountryInfoSingle/{countryId}")]

        public PostCountryInfo GetPostCountryInfoSingle(int countryId)
        {
            string sql = @"SELECT 
                [CountryId],
                [DatorId],
		        [CountryName],
		        [IsDemocracy],
		        [PopulationInMillion],
                [DataCreated],
                [DataUpdated]
	        FROM CountryDataSchema.PostCountryInfo
            WHERE CountryId=" + countryId.ToString();

            PostCountryInfo getPostCountryInfoSingle = _dapper.LoadDataSingle<PostCountryInfo>(sql);
            return getPostCountryInfoSingle;
        }

        [HttpGet("PostCountryInfoByDator/{datorId}")]

        public IEnumerable<PostCountryInfo> GetPostCountryInfoByDator(int datorId)
        {
            string sql = @"SELECT 
                [CountryId],
                [DatorId],
		        [CountryName],
		        [IsDemocracy],
		        [PopulationInMillion],
                [DataCreated],
                [DataUpdated]
	        FROM CountryDataSchema.PostCountryInfo
            WHERE DatorId=" + datorId.ToString();

            IEnumerable<PostCountryInfo>getPostCountryInfoByUser = _dapper.LoadData<PostCountryInfo>(sql);
            return getPostCountryInfoByUser;
        }

        [HttpGet("MyPostCountryInfo")]

         public IEnumerable<PostCountryInfo> GetMyPostCountryInfo()
        {
            string sql = @"SELECT 
                [CountryId],
                [DatorId],
		        [CountryName],
		        [IsDemocracy],
		        [PopulationInMillion],
                [DataCreated],
                [DataUpdated]
	        FROM CountryDataSchema.PostCountryInfo
            WHERE DatorId=" + this.User.FindFirst("datorId")?.Value;

            return _dapper.LoadData<PostCountryInfo>(sql);
            
        }

        [HttpPost("PostCountryInfo")]
        public IActionResult AddPostCountryInfo(PostCountryInfoToAdd postCountryInfoToAdd)
        {
            string sql = @"INSERT INTO CountryDataSchema.PostCountryInfo (
            [DatorId],
	        [CountryName],
	        [IsDemocracy],
	        [PopulationInMillion],
            [DataCreated],
            [DataUpdated]
            ) VALUES (" + this.User.FindFirst("datorId")?.Value
            + ",'" + postCountryInfoToAdd.CountryName +
	        "'," + "'" + postCountryInfoToAdd.IsDemocracy +
	        "'," + postCountryInfoToAdd.PopulationInMillion +
            ", GETDATE(), GETDATE())";

            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            } throw new Exception ("Failed to add new post");
        }


        [HttpPut("EditCountryInfo")]
        public IActionResult EditCountryInfo(PostCountryInfoToEdit postCountryInfoToEdit)
        {
            string sql = @"
            UPDATE CountryDataSchema.PostCountryInfo
		    SET 
		        [CountryName] =" + "'" + postCountryInfoToEdit.CountryName + "',"+
                "[IsDemocracy]="  + "'"+ postCountryInfoToEdit.IsDemocracy + "'," +
                "[PopulationInMillion]= " + postCountryInfoToEdit.PopulationInMillion + ","+
                @" DataUpdated = GETDATE()
                    WHERE CountryId = " +  postCountryInfoToEdit.CountryId.ToString() + 
                    "AND DatorId = "  + this.User.FindFirst("datorId")?.Value;

             Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Edit post");
        }

        [HttpDelete("DeletePostCountryInfo/{countryId}")]

        public IActionResult DeletePostCountyInfo(int countryId)
        {
            string sql = @"DELETE FROM CountryDataSchema.PostCountryInfo WHERE CountryId ="
                + countryId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Edit post");
        }
    }
}
