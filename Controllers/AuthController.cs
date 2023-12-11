using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DogInfoAPI.Data;
using DogInfoAPI.Data.Dtos;
using DogInfoAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DogInfoAPI.Controllers
{
    [Authorize]
    public class AuthController : BaseApiController
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if(userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM DogInfoSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";
                
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                
                if(existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128/8];
                    using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password,passwordSalt);

                    string sqlAddAuth = @"INSERT INTO DogInfoSchema.Auth(
                                    [Email],
                                    [PasswordHash],
                                    [PasswordSalt]
                                    ) VALUES ('" + userForRegistration.Email 
                                    + "', @PasswordHash, @PasswordSalt)";  

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    
                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                   
                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordHashParameter);
                    sqlParameters.Add(passwordSaltParameter);

                    if(_dapper.ExecuteSQLWithParameters(sqlAddAuth,sqlParameters))
                    {
                        string sqlAddUser = @"
                            INSERT INTO DogInfoSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender]
                            ) VALUES (" +
                                "'" + userForRegistration.FirstName + 
                                "', '" + userForRegistration.LastName +
                                "', '" + userForRegistration.Email +
                                "', '" + userForRegistration.Gender +
                                "')";

                        if(_dapper.ExecuteSQL(sqlAddUser))
                        {
                        return Ok();    
                        }

                        throw new Exception("Failed to add user");
                    }

                }
                
                throw new Exception("Failed to register user.");
            }
            throw new Exception("Passwords do not match");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT
                [PasswordHash],
                [PasswordSalt] FROM DogInfoSchema.Auth WHERE Email = '" +
                userForLogin.Email + "'";
            
            UserForLoginConfirmationDto userForConfirmation = _dapper.
                LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);
            
            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password,userForConfirmation.PasswordSalt);
            
            for(int i = 0;i < passwordHash.Length;i++)
            {
                if (userForConfirmation.PasswordHash[i] != passwordHash[i])
                    return StatusCode(401,"Wrong Password.");
            }

            string userIdSql = @"SELECT UserId From DogInfoSchema.Users 
                WHERE Email = '" + userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            
            return Ok(new Dictionary<string,string> {
                {"token", _authHelper.CreateToken(userId)}
            });
        }


        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            //check for fake UserId
            string userIdSql = @"SELECT UserId From DogInfoSchema.Users 
                WHERE UserId = '" + User.FindFirst("userId").Value + "'";
        
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }
        
       

    }
}