namespace InficareTaskProject.ViewModels
{
    public class StudentsViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Class { get; set; }
        public string RollNo { get; set; }
        public string Section { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
    }
    public class AuthenticateUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
