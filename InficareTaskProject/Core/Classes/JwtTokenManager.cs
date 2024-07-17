
using InficareTaskProject.Data;
using InficareTaskProject.Entities;
using InficareTaskProject.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        private readonly UserManager<Student> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Role> _roleManager;




        public JwtTokenManager(IConfiguration configuration, UserManager<Student> userManager, ApplicationDbContext context, RoleManager<Role> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        public string GenerateToken(Student identityUser)
        {

            var userClaims = GetUserClaims(identityUser);

            return GenerateJwtToken(userClaims);
        }

        private List<Claim> GetUserClaims(Student identityUser) 
        {
            //var r = _userManager.GetRolesAsync(identityUser);
            //var role = r.Result[0];


            var roleId = _context.UserRoles.Where(x => x.UserId == identityUser.Id).Select(x => x.RoleId).FirstOrDefault();
            var role = _context.Roles.Where(x => x.Id == roleId).FirstOrDefault();
            var roles = _context.Role.Where(x => x.Id == roleId).FirstOrDefault();

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identityUser.UserName),
                new Claim(ClaimTypes.Email, identityUser.Email),
                new Claim(ClaimTypes.Role, role.Name),
            };
            //var list = GetPermissionsForRole(role);
            //var roleId = _roleManager.Roles.Where(x => x.Name == "Admin").Select(x => x.Id).FirstOrDefault();
            var permissionList = _context.RolePermissions.Include(x => x.Permission).Where(x =>x.RoleId ==roleId)
                                         .Select(x => new
                                         {
                                             Permission = x.Permission.Name,

                                         }).ToList();

            foreach(var per in permissionList)
            {
                userClaims.Add(new Claim("permissions", per.Permission));
            }


            return userClaims;
        }
        private string GenerateJwtToken(List<Claim> userClaims)
        {
            try
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
            catch (Exception ex) 
            { 
                return ex.Message;
            }
        }

        private List<object> GetPermissionsForRole(string role)
        {
            var roleId = _roleManager.Roles.Where(x => x.Name == role).Select(x => x.Id).FirstOrDefault();
            var permissionList = _context.RolePermissions
                                        .Include(x => x.Permission)
                                        .Where(x => x.RoleId == roleId)
                                        .Select(x => new
                                        {
                                            Permission = x.Permission.Name
                                        })
                                        .ToList();


            var resultList = permissionList.Select(x => (object)x.Permission).ToList();

            return resultList;
        }
    }
}





