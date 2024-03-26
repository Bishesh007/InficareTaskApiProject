namespace InficareTaskProject.ViewModels
{
    public class ResponseModel
    {
        public bool? isSuccess { get; set; }
        public string? message { get; set; }
        public string? errorMessage { get; set; }

        public string? token { get; set; }
    }
}
