using Microsoft.EntityFrameworkCore.Metadata;

namespace InficareTaskProject.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<StudentSubject> StudentSubjects { get; set; }
    }
}
