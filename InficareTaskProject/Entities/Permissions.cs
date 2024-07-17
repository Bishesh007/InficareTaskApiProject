using System.ComponentModel.DataAnnotations;

namespace InficareTaskProject.Entities
{
    public class Permissions
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }


}
