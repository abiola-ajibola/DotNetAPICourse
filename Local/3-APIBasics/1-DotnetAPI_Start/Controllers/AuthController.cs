using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using HelloWorld.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize] // Controller level authorization
    [ApiController]
    [Route("/auth")]
    public class AuthController(IConfiguration config) : ControllerBase
    {
        private readonly DataContextDapper _context = new(config);
        private readonly AuthHelper _authHelper = new(config);
        private readonly Mapper _mapper = new(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserForRegistrationDto, UserForLoginDto>();
        }));

        [HttpPost("login"), AllowAnonymous]
        public IActionResult Login(UserForLoginDto loginData)
        {
            // 1. Get the user by email from Auth table
            // string getAuthQuery = @"
            // SELECT
            //     [PasswordHash],
            //     [PasswordSalt]
            // FROM TutorialAppSchema.Auth
            // WHERE Email = @Email
            // ";
            string getAuthQuery2 = @"
            SELECT
                [Email],
                [PasswordHash]
            FROM TutorialAppSchema.Auth2
            WHERE Email = @Email
            ";
            var authDetails = _context.LoadSingle<UserForLoginConfirmationDto2>(getAuthQuery2, new { loginData.Email });
            // 2. Verify password
            /* byte[] incommingHash = GetPasswordHash(loginData.Password, authDetails.PasswordSalt);
            for (int i = 0; i < incommingHash.Length; i++)
            {
                if (incommingHash[i] != authDetails.PasswordHash[i])
                {
                    return BadRequest(new { message = "Incorrect email or password" });
                }
            } */
            // OR using PasswordHasher
            /////////////////////////////
            if (!_authHelper.VerifyPassword(loginData, loginData.Password, authDetails.PasswordHash))
            {
                return BadRequest(new { message = "Incorrect email or password" });
            }
            //////////////////////////////
            // 3. Respond with user information if password is correct

            string getUserIdQuery = @"
            SELECT [userId] FROM TutorialAppSchema.Users
            WHERE Email = @Email
            ";
            int userId = _context.LoadSingle<int>(getUserIdQuery, new { loginData.Email });
            Console.WriteLine("UserId => " + userId);
            return StatusCode(200, new { token = _authHelper.CreateToken(userId) });
        }

        [HttpPost("register", Name = "register"), AllowAnonymous]
        public IActionResult Register(UserForRegistrationDto userData)
        {
            Console.WriteLine(userData);
            // Check passord accuracy
            if (userData.Password != userData.PasswordConfirm)
            {
                throw new ValidationException("Passwords do not match!");
            }
            // Check if email is already in use
            string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = @Email";
            if (_context.ExecuteSql(sqlCheckUserExists, new { Email = userData.Email }))
            {
                throw new Exception("Email already in use");
            }

            ///////////////////////////////////////////////////////////////////
            /* 
            ////// Create an Auth record in the Auth table, then create user ////////
            // 1. Generate salt
            byte[] passwordSalt = new byte[128 / 8]; // 16 bytes. This size is somewhat standard for password salts.

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            // 2. Generate password hash
            byte[] passwordHash = GetPasswordHash(userData.Password, passwordSalt);
            */
            /////////////////////////////////////////////////////////////////////

            /// OR
            UserForLoginDto user = _mapper.Map<UserForLoginDto>(userData);
            string passwordHash = _authHelper.GetPasswordHash2(user, userData.Password);

            // 3. Insert into Auth table
            /* 
            string insertIntoAuthQuery = @"
            INSERT INTO TutorialAppSchema.Auth 
            (
                [Email],
                [PasswordHash],
                [PasswordSalt]
            )
            VALUES
            (
                @Email,
                @PasswordHash,
                @PasswordSalt
            )";
            var parameters = new { Email = userData.Email, PasswordHash = passwordHash, PasswordSalt = passwordSalt }; 
            */

            ////// ALTERNATIVELY
            /////////////////////

            string insertIntoAuthQuery = @"
            INSERT INTO TutorialAppSchema.Auth2 
            (
                [Email],
                [PasswordHash]
            )
            VALUES
            (
                @Email,
                @PasswordHash
            )";
            var parameters = new { Email = userData.Email, PasswordHash = passwordHash };
            bool isQuerySuccessful = _context.ExecuteSql(insertIntoAuthQuery, parameters);
            //////////////////////
            // 4. Insert into Users table if Auth insert is successful
            if (!isQuerySuccessful)
            {
                throw new Exception("Could not register User");
            }

            string insertIntoUsersQuery = @"
                INSERT INTO TutorialAppSchema.Users
                (
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active]
                )
                VALUES
                (
                    @FirstName,
                    @LastName,
                    @Email,
                    @Gender,
                    1
                )";
            if (!_context.ExecuteSql(insertIntoUsersQuery, userData))
            {
                throw new Exception("Could not register User");
            }
            return StatusCode(201, new { message = "User created successfully" });

        }

        [HttpGet("refreshToken")]
        public IActionResult RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                // This user is inheritted from the base contoller. It represents the user from the httpContext
                // not the User model
                this.User.FindFirst("userId")?.Value + "'";

            int userId = _context.LoadSingle<int>(userIdSql);

            return StatusCode(200, new { token = _authHelper.CreateToken(userId) });
        }
    }
}