using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        public AuthHelper(IConfiguration config)
        {
            _config = config;
        }
        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            // Why not use passwordHasher, which is recommended?
            // see: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1?view=aspnetcore-9.0
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
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

        public string GetPasswordHash2(UserForLoginDto user, string password)
        {
            var hasher = new PasswordHasher<UserForLoginDto>();
            return hasher.HashPassword(user, password);
        }

        public bool VerifyPassword(UserForLoginDto user, string password, string hash)
        {
            var hasher = new PasswordHasher<UserForLoginDto>();
            return hasher.VerifyHashedPassword(user, hash, password) == PasswordVerificationResult.Success;
        }

        public string CreateToken(int userId)
        {
            // see: https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claim?view=net-9.0
            // Claims are for creating JWT subjects
            IList<Claim> claims = [
                new Claim("userId", userId.ToString())
            ];

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString ?? ""
                    )
                );
            // see: https://learn.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.signingcredentials?view=netframework-4.8.1
            SigningCredentials credentials = new(
                    tokenKey,
                    SecurityAlgorithms.HmacSha512Signature
                );

            // see: https://learn.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.securitytokendescriptor?view=netframework-4.8.1
            SecurityTokenDescriptor descriptor = new()
            {
                // see: https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsidentity.-ctor?view=net-9.0#system-security-claims-claimsidentity-ctor(system-collections-generic-ienumerable((system-security-claims-claim)))
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            // see: https://learn.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt.jwtsecuritytokenhandler?view=msal-web-dotnet-latest
            // used to create JWTs
            JwtSecurityTokenHandler tokenHandler = new();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            // A more concise verison
            // No need to declare many things like descriptor, credentials, and claims

            // SecurityToken token = tokenHandler.CreateToken(new()
            // {
            //     Subject = new ClaimsIdentity([new Claim("type", "value")]),
            //     SigningCredentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature),// or new (tokenKey, SecurityAlgorithms.HmacSha512Signature)
            //     Expires = DateTime.Now.AddDays(1)
            // });

            return tokenHandler.WriteToken(token);
        }
    }
}