using DotNetWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using DotNetWebAPI.Dtos;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using DotNetWebAPI.Helpers;

namespace DotNetWebAPI.Controllers
{
   [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
    
        private object? symetricSecutiryKey;

        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper (config);

            _authHelper = new AuthHelper(config);
        }

       [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if(userForRegistration.Password== userForRegistration.PasswordConfirm) 
            {
                string sqlCheckUserExists = "SELECT Email FROM CountryDataSchema.Auth WHERE Email = '" + 
                    userForRegistration.Email + "'";

                    IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                    if (existingUsers.Count()==0)
                    {
                        byte[] passwordSalt = new byte[128/8];
                        using (RandomNumberGenerator rng= RandomNumberGenerator.Create())
                        {
                            rng.GetNonZeroBytes(passwordSalt);
                        }


                        byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                        string sqlAddAuth = @"
                            INSERT INTO CountryDataSchema.Auth (Email,
                            PasswordHash,
                            PasswordSalt) VALUES ('" + userForRegistration.Email + 
                            "', @PasswordHash, @PasswordSalt)";

                            Console.WriteLine(sqlAddAuth);

                        List<SqlParameter> sqlParameters = new List<SqlParameter>();

                        SqlParameter passwordSaltParameter= new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                        passwordSaltParameter.Value = passwordSalt;

                        SqlParameter passwordHashParameter= new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                        passwordHashParameter.Value = passwordHash;

                        sqlParameters.Add(passwordSaltParameter);
                        sqlParameters.Add(passwordHashParameter);

                        if(_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))

                    {
                        string sqlAddUser = @"INSERT INTO CountryDataSchema.Dator (
	                        [FirstName],
	                        [LastName],
	                        [Email],
                            [Gender] 
                            ) VALUES (" +
                            "'" + userForRegistration.FirstName +
                            "'," + "'" + userForRegistration.LastName +
                            "'," + "'" + userForRegistration.Email +
                            "'," + "'" + userForRegistration.Gender +
                            "')";

                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                        return Ok();
                        }
                    throw new Exception("Failed to add user");
                    }
                    throw new Exception("Failed to register user");

                    } 
                    throw new Exception("User with this email already exists");
            } 
            throw new Exception ("Password do not match!");
        } 

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT
                PasswordHash, 
                PasswordSalt FROM CountryDataSchema.Auth WHERE Email = '" + 
                userForLogin.Email +"'";
                UserForLoginConfirmationDto userForConfirmation = _dapper
                    .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

                byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

                //if (passwordHash == userForConfirmation.PasswordHash)

                for(int index = 0; index<passwordHash.Length; index++)
                {
                    if (passwordHash[index]!= userForConfirmation.PasswordHash[index])
                    {
                        return StatusCode(401, "Incorrect Password!");
                    }
                }


            string datorIdSql = @"SELECT DatorId FROM CountryDataSchema.Dator WHERE Email = '" +
                userForLogin.Email + "'";

            int datorId = _dapper.LoadDataSingle<int>(datorIdSql);

            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(datorId)}
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string datorIdSql = @"SELECT DatorId FROM CountryDataSchema.Dator WHERE DatorId = '" +
                User.FindFirst("datorId")?.Value + "'";

        int datorId = _dapper.LoadDataSingle<int>(datorIdSql);

        return _authHelper.CreateToken(datorId);
        }
        
    }
}