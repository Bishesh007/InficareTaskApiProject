using InficareTaskProject.Data;
using InficareTaskProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InficareTaskProject.Services
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                await SeedRolesAsync(roleManager);
                await SeedAdminUserAsync(userManager);
            }
        }

        private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
        {
            if (await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new Role { Name = "Admin" });
            }
            if (await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new Role{ Name = "User" });
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<Student> userManager)
        {
            if (userManager.Users.All(u => u.UserName != "admin"))
            {
                var adminUser = new Student
                {
                    UserName = "admin",
                    Email = "admin@gmail.com"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Handle errors if necessary
                    throw new Exception("Failed to create admin user.");
                }
            }
        }
    }
    }
