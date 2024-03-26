
using InficareTaskProject.Entities;
using InficareTaskProject.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InficareTaskProject.Classes
{
    public class JwtTokenManager : IJwtTokenManager
    {
        private readonly IConfiguration _configuration;
        public JwtTokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(Student identityUser)
        {
            var userClaims = GetUserClaims(identityUser);

            return GenerateJwtToken(userClaims);
        }

        private List<Claim> GetUserClaims(Student identityUser)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identityUser.UserName),
                new Claim(ClaimTypes.Email, identityUser.Email),
            };

            return userClaims;
        }

        private string GenerateJwtToken(List<Claim> userClaims)
        {
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:JwtKey"]));

            var signingCred = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                SigningCredentials = signingCred,
                Expires = DateTime.UtcNow.AddHours(1),
                Audience = _configuration["JWT:JwtAudience"],
                Issuer = _configuration["JWT:JwtAudience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}





