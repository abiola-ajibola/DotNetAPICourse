using System.ComponentModel.DataAnnotations;
using System.Text;
using AutoMapper;
using DotnetAPI.Dtos;
using HelloWorld.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController(IConfiguration config) : ControllerBase
    {
        private readonly DataContextDapper _context = new(config);
        private readonly Mapper _mapper = new(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserForRegistrationDto, UserForLoginDto>();
        }));

        [HttpPost("login")]
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
            Console.WriteLine("AUTH:\t" + authDetails.PasswordHash);
            if (!VerifyPassword(loginData, loginData.Password, authDetails.PasswordHash))
            {
                return BadRequest(new { message = "Incorrect email or password" });
            }
            //////////////////////////////
            // 3. Respond with user information if password is correct
            Console.WriteLine("Login");
            Console.WriteLine(loginData);
            return Ok(new { loginData.Email });
        }

        [HttpPost("register", Name = "register")]
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
            string passwordHash = GetPasswordHash2(user, userData.Password);

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
            return CreatedAtRoute("register", new { message = "User created successfully" });

        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            // Why not use passwordHasher, which is recommended?
            // see: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1?view=aspnetcore-9.0
            string passwordSaltPlusString = config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
            Console.WriteLine("Hash => " + hash);
            return hash;
        }

        private static string GetPasswordHash2(UserForLoginDto user, string password)
        {
            var hasher = new PasswordHasher<UserForLoginDto>();
            return hasher.HashPassword(user, password);
        }

        private static bool VerifyPassword(UserForLoginDto user, string password, string hash)
        {
            var hasher = new PasswordHasher<UserForLoginDto>();
            return hasher.VerifyHashedPassword(user, hash, password) == PasswordVerificationResult.Success;
        }
    }
}