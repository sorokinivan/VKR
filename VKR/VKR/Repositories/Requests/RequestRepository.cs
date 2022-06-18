using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using VKR.Controllers;
using VKR.Core;
using VKR.Models;
using VKR.Models.ExamModels;
using VKR.Models.RequestModels;

namespace VKR.Repositories.Requests
{
    public class RequestRepository : IRequestRepository
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RequestRepository(DBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task SaveRequest(Request model)
        {
            var check = await _context.Requests.Where(x => x.UserId == model.UserId && x.ExamId == model.ExamId).FirstOrDefaultAsync();

            if (check != null)
                throw new Exception("Такая заявка уже создана");

            _context.Requests.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task SaveResultRequest(RequestResultModel model)
        {
            var request = await _context.Requests.Where(x => x.UserId == model.UserId && x.ExamId == model.ExamId).FirstOrDefaultAsync();
            if(request != null && request.Status == (int)RequestStatusEnum.NotChecked)
            {
                request.Status = model.Status;
                if (!String.IsNullOrWhiteSpace(model.Text))
                {
                    request.RequestText = model.Text;
                }
                if(request.Status == (int)RequestStatusEnum.Approved)
                {
                    _context.UsersAvailableExams.Add(new UsersAvailableExams
                    {
                        UserId = model.UserId,
                        ExamId = model.ExamId,
                    });
                }
                //SendRequestMail(model);//ПОЧИНИТЬ ЭТО
                await _context.SaveChangesAsync();
            }
        }

        private async void SendRequestMail(RequestResultModel model)
        {
            //var user = await _context.AspNetUsers.Where(x=>x.Id == model.UserId).FirstOrDefaultAsync();
            //var user = await _userManager.FindByIdAsync(model.UserId);
            SmtpClient client = new SmtpClient("smtp.yandex.ru", 587);
            //If you need to authenticate
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("Nextop670@yandex.ru", "rbgcbdnbpfiyfncw");
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("Nextop670@yandex.ru");
            mailMessage.To.Add(model.UserName);
            mailMessage.Subject = "Hello There";
            mailMessage.Body = "Hello my friend!";
            client.Send(mailMessage);
        }

        public async Task<List<RequestViewModel>> GetNotCheckedRequests()
        {
            var requests = await _context.Requests.Where(x=>x.Status == (int)RequestStatusEnum.NotChecked).ToListAsync();
            var examsIds = requests.Select(x => x.ExamId).ToList();
            var exams = await _context.Exams.Where(x => examsIds.Contains(x.Id)).Select(x => new {x.Id, x.ExamName}).ToListAsync();
            var usersIds = requests.Select(x => x.UserId).ToList();
            var users = await _userManager.Users.Where(x=> usersIds.Contains(x.Id)).Select(x => new {x.Id, x.UserName}).ToListAsync();
            var result = new List<RequestViewModel>();
            foreach(var request in requests)
            {
                result.Add(new RequestViewModel
                {
                    Id = request.Id,
                    ExamId = request.ExamId,
                    ExamName = exams.Where(x => x.Id == request.ExamId).Select(x=>x.ExamName).FirstOrDefault(),
                    UserId = request.UserId,
                    UserName = users.Where(x => x.Id == request.UserId).Select(x=>x.UserName).FirstOrDefault(),
                    Status = request.Status,
                    RequestText = request.RequestText
                });
            }
            return result;
        }

        public async Task<List<RequestViewModel>> GetRequestsByUser(string user)
        {
            try
            {
                var requests = await _context.Requests.Where(x => x.UserId == user).ToListAsync();
                var examsIds = requests.Select(x => x.ExamId).ToList();
                var exams = await _context.Exams.Where(x => examsIds.Contains(x.Id)).Select(x => new { x.Id, x.ExamName }).ToListAsync();
                var userName = await _userManager.Users.Where(x => x.Id == user).Select(x => x.UserName).FirstOrDefaultAsync();
                var result = new List<RequestViewModel>();
                foreach (var request in requests)
                {
                    var t = exams.Where(x => x.Id == request.ExamId).Select(x => x.ExamName).FirstOrDefault();
                    result.Add(new RequestViewModel
                    {
                        Id = request.Id,
                        ExamId = request.ExamId,
                        ExamName = exams.Where(x => x.Id == request.ExamId).Select(x => x.ExamName).FirstOrDefault(),
                        UserName = userName,
                        Status = request.Status,
                        RequestText = request.RequestText
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                return new List<RequestViewModel>();
            }
        }

        public async Task<List<ExamWithCompetenciesViewModel>> GetExamsForRequest(string user)
        {
            var excludeExams = await _context.Requests.Where(x => x.UserId == user && (x.Status == (int)RequestStatusEnum.Approved || x.Status == (int)RequestStatusEnum.NotChecked)).Select(x=>x.ExamId).ToListAsync();
            var exams = await _context.Exams.Where(x=>!excludeExams.Contains(x.Id)).ToListAsync();
            var examsIds = exams.Select(x=>x.Id).ToList();
            var competencies = await _context.ExamsCompetencies.Where(x => examsIds.Contains(x.ExamId)).ToListAsync();
            var competenciesIds = competencies.Select(x=>x.CompetencyId).ToList();
            var competenciesNames = await _context.Competencies.Where(x => competenciesIds.Contains(x.Id)).ToListAsync();
            var result = new List<ExamWithCompetenciesViewModel>();
            foreach(var exam in exams)
            {
                var examCompetencies = competencies.Where(x => x.ExamId == exam.Id).Select(x=>x.CompetencyId).ToList();
                result.Add(new ExamWithCompetenciesViewModel
                {
                    ExamId = exam.Id,
                    ExamName = exam.ExamName,
                    Competencies = competenciesNames.Where(x => examCompetencies.Contains(x.Id)).Select(x => x.CompetencyName).ToList()
                });
            }
            return result;
        }
    }
}
