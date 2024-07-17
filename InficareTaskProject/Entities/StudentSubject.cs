using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InficareTaskProject.Entities
{
    public class StudentSubject
    {
        [Key]
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string StudentId { get; set;}
        public Subject Subject { get; set; }
        public Student Student { get; set; }

    }
}
