using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security;

namespace InficareTaskProject.Entities
{
    public class RolePermissions
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(450)]
        public string RoleId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int PermissionId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permissions Permission { get; set; }
    }
}
