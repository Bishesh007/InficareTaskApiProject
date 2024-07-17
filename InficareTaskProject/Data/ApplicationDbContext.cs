using InficareTaskProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace InficareTaskProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<Student, Role, string>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RolePermissions>().HasNoKey();
            builder.Entity<RolePermissions>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Entity<RolePermissions>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            builder.Entity<RolePermissions>()
                .HasOne(rp => rp.Permission)
                .WithMany() // Assuming Permission does not have a navigation property back to RolePermissions
                .HasForeignKey(rp => rp.PermissionId);
            // Define RolePermissions as keyless entity type

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<Role> Role { get; set; }

    }
}
