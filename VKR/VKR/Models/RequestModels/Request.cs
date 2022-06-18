namespace VKR.Models.RequestModels
{
    public class Request
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public string? RequestText { get; set; }
    }
}
