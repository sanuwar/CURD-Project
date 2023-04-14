using DotNetWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using DotNetWebAPI.Dtos;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DotNetWebAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper (config);
            _config = config;
        }

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


                        byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

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
                        return Ok();
                        }
                        throw new Exception("Failed to register user");

                    } 
                    throw new Exception("User with this email already exists");
            } 
            throw new Exception ("Password do not match!");
        } 

        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT
                PasswordHash, 
                PasswordSalt FROM CountryDataSchema.Auth WHERE Email = '" + 
                userForLogin.Email +"'";
                UserForLoginConfirmationDto userForConfirmation = _dapper
                    .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

                byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

                //if (passwordHash == userForConfirmation.PasswordHash)

                for(int index = 0; index<passwordHash.Length; index++)
                {
                    if (passwordHash[index]!= userForConfirmation.PasswordHash[index])
                    {
                        return StatusCode(401, "Incorrect Password!");
                    }
                }

            return Ok();
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            String passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + 
                            Convert.ToBase64String(passwordSalt); 

                        return KeyDerivation.Pbkdf2(
                            password: password,
                            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                            prf: KeyDerivationPrf.HMACSHA256,
                            iterationCount: 100000,
                            numBytesRequested: 256 / 8
                        );
        }
    }
}