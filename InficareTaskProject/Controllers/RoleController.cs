using InficareTaskProject.Core.Permission;
using InficareTaskProject.Data;
using InficareTaskProject.Entities;
using InficareTaskProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InficareTaskProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<Student> _userManager;

        public RoleController(ApplicationDbContext context, RoleManager<Role> roleManager, UserManager<Student> userManger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManger;
        }

        [HttpDelete("/api/role/deleteRole/{id}")]
        //[Authorize(PermissionTypes.DeleteRole)]
        public async Task<ResponseModel> DeleteRole(string id)
        {
            var response = new ResponseModel();
            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role == null)
                {
                    response.errorMessage = "Role not found";
                    response.isSuccess = false;
                    return response;
                }

                // Manually delete related RolePermissions using raw SQL query
                //var sql = $"DELETE FROM RolePermissions WHERE RoleId = {id}";
                await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM RolePermissions WHERE RoleId = {id}");

                // Now, delete the role
                var result = await _roleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    response.errorMessage = result.Errors.Select(x => x.Description).First();
                    response.isSuccess = false;
                }
                else
                {
                    response.errorMessage = "";
                    response.isSuccess = true;
                    response.message = "Role deleted Successfully";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                response.isSuccess = false;
                return response;
            }
        }

        [HttpGet("/api/role/getAllRoles")]
        //[Authorize(PermissionTypes.GetRole)]
        public async Task<List<RoleViewModel>> GetAllRoles()
        {
            var permissionLists = _context.RolePermissions
                             .Select(x => new
                             {
                                 Id = x.PermissionId,
                                 RoleId = x.RoleId

                             }).ToList();

            var rolesList = await _roleManager.Roles
                               .Select(x => new RoleViewModel
                               {
                                   Id = x.Id,
                                   Name = x.Name,
                               }).ToListAsync();

            var joinBoth = permissionLists.Join(
               rolesList,  // Outer sequence
               permission => permission.RoleId,  // Outer key selector
               role => role.Id,  // Inner key selector
               (permission, role) => new  // Result selector
               {
                   RoleId = role.Id,
                   RoleName = role.Name,
                   Permission = new PermissionView { Id = permission.Id } // Create PermissionView object
               })
               .GroupBy(x => new { x.RoleId, x.RoleName }) // Group by RoleId and RoleName
               .Select(g => new RoleViewModel // Select into RoleViewModel
               {
                   Id = g.Key.RoleId,
                   Name = g.Key.RoleName,
                   PermissionList = g.Select(x => x.Permission).ToList() // Convert grouped permissions to list
               })
               .ToList();


            return joinBoth;
        }

        [HttpPost("/api/role/createRolePermission")]
        //[Authorize(PermissionTypes.CreateRole)]
        public async Task<ResponseModel> CreateRoleWithPermissions(RolePermissionViewModel rolePermissionViewModel)
        {
            var response = new ResponseModel();
            try
            {
                var roleCreation = await _roleManager.CreateAsync(new Role { Name = rolePermissionViewModel.RoleName });

                if (!roleCreation.Succeeded)
                {
                    response.errorMessage = roleCreation.Errors.Select(x => x.Description).First();
                    response.isSuccess = false;
                }
                else
                {
                    var newRole = await _roleManager.FindByNameAsync(rolePermissionViewModel.RoleName);
                    if (rolePermissionViewModel.PermissionList != null && rolePermissionViewModel.PermissionList.Any())
                    {
                        var roleId = newRole.Id;

                        foreach (var permission in rolePermissionViewModel.PermissionList)
                        {
                            await _context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO RolePermissions (RoleId, PermissionId) VALUES ({roleId}, {permission.Id})");
                        }
                    }

                    response.errorMessage = "";
                    response.isSuccess = true;
                    response.message = "Role created Successfully";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                response.isSuccess = false;
                return response;
            }
        }

        [HttpPost("/api/role/updateRolePermissions")]
        //[Authorize(PermissionTypes.UpdateRole)]
        public async Task<ResponseModel> UpdateRolePermissions(RolePermissionsViewModel rolePermissionViewModel)
        {
            var response = new ResponseModel();
            try
            {
                // Find the role by name
                var existingRole = await _roleManager.FindByIdAsync(rolePermissionViewModel.Id);

                if (existingRole == null)
                {
                    // Handle the case where the role does not exist
                    response.errorMessage = $"Role '{rolePermissionViewModel.RoleName}' not found.";
                    response.isSuccess = false;
                    return response;
                }

                // Update the permissions for the role
                // Here you may want to delete existing permissions and then add new ones
                // For simplicity, I'm assuming that you have a method to delete existing permissions

                // Delete existing permissions for the role
                await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM RolePermissions WHERE RoleId = {existingRole.Id}");

                // Add new permissions for the role
                foreach (var permission in rolePermissionViewModel.PermissionList)
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO RolePermissions (RoleId, PermissionId) VALUES ({existingRole.Id}, {permission.Id})");
                }

                // Update the role name if necessary
                if (existingRole.Name != rolePermissionViewModel.RoleName)
                {
                    existingRole.Name = rolePermissionViewModel.RoleName;
                    await _roleManager.UpdateAsync(existingRole);
                }

                response.errorMessage = "";
                response.isSuccess = true;
                response.message = "Role permissions updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                response.isSuccess = false;
                return response;
            }
        }
        [HttpGet("/api/role/getRoles")]
        public async Task<List<RoleViewModel>> GetRoles()
        {
            var rolesList = await _roleManager.Roles
                   .Select(x => new RoleViewModel
                   {
                       Id = x.Id,
                       Name = x.Name,
                   }).ToListAsync();

            return rolesList;
        }
    }

}
