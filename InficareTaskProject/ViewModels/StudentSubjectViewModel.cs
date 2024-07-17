namespace InficareTaskProject.ViewModels
{
    public class StudentSubjectViewModel
    {
        public string StudentId { get; set; }
        public List<SubjectViewModel> SubjectList { get; set; }

    }

    public class SubjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

}
