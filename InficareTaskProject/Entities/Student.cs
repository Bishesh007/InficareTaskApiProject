using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InficareTaskProject.Entities
{
    public class Student : IdentityUser
    {
        public string Class { get; set; }
        public string RollNo { get; set; }
        public string Section { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
}
