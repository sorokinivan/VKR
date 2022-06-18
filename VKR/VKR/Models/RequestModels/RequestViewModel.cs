namespace VKR.Models.RequestModels
{
    public class RequestViewModel
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Status { get; set; }
        public string RequestText { get; set; }
    }
}
