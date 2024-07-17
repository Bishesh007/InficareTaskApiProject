using Microsoft.AspNetCore.Identity;

namespace InficareTaskProject.Entities
{
    public class Role : IdentityRole
    {
        public virtual ICollection<RolePermissions> RolePermissions { get; set; }
    }
}
