using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VKR.Core;
using VKR.Models;
using VKR.Models.ExamModels;
using VKR.Models.ProfessionalGradeModels;
using VKR.Repositories.ProfessionalGrades;

namespace VKR.Repositories.Exams
{
    public class ExamRepository : IExamRepository
    {
        private readonly DBContext _context;
        private readonly IProfessionalGradeRepository _professionalGradeRepository;
        private readonly UserManager<IdentityUser> _userManager;
        public ExamRepository(DBContext context, IProfessionalGradeRepository professionalGradeRepository, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _professionalGradeRepository = professionalGradeRepository;
            _userManager = userManager;
        }

        public async Task<List<Exam>> GetExams()
        {
            return await _context.Exams.ToListAsync();
        }
        public async Task<IEnumerable<Exam>> GetAvailableExamsPerCurrentUser(string userId)
        {
            var examIds = await _context.UsersAvailableExams.Where(x => x.UserId == userId).Select(x => x.ExamId).ToListAsync();
            var result = await _context.Exams.Where(x => examIds.Contains(x.Id) && x.Status == (int)ExamStatusEnum.Open).ToListAsync();
            return result;
        }
        public async Task OpenExam(int examId)
        {
            var exam = await _context.Exams.Where(x => x.Id == examId).FirstOrDefaultAsync();
            exam.Status = (int)ExamStatusEnum.Open;
            await _context.SaveChangesAsync();
        }
        public async Task CloseExam(int examId)
        {
            var exam = await _context.Exams.Where(x => x.Id == examId).FirstOrDefaultAsync();
            exam.Status = (int)ExamStatusEnum.Close;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Exam>> GetFinishedExamsPerCurrentUser(string userId)
        {
            var examIds = await _context.UsersFinishedExams.Where(x => x.UserId == userId).Select(x => x.ExamId).ToListAsync();
            var result = await _context.Exams.Where(x => examIds.Contains(x.Id)).ToListAsync();
            return result;
        }

        public async Task SaveExamResults(ExamResultsDataModel model, string userId)
        {
            var questions = await _context.Questions.Where(x => x.ExamId == model.ExamId).Select(x => x.Id).ToListAsync();
            var resultList = new List<ExamDichotomousGrades>();
            var correctAnswers = await _context.Answers.Where(x => questions.Contains(x.QuestionId) && x.IsCorrect == true).ToListAsync();
            foreach (var question in model.Questions)
            {
                var result = new ExamDichotomousGrades
                {
                    ExamId = model.ExamId,
                    QuestionId = question.QuestionId,
                    UserId = userId
                };
                var questionAnswers = correctAnswers.Where(x => x.QuestionId == question.QuestionId).Select(x => x.Id).ToList();
                if (questionAnswers.SequenceEqual(question.Answers))
                    result.Grade = 1;
                else
                    result.Grade = 0;
                resultList.Add(result);
                _context.ExamDichotomousGrades.Add(result);

            }

            var dataForGradeCalculation = await GetDataForGradeCalculation(resultList);
            var calculatedGrade = await _professionalGradeRepository.CalculateProfessionalGrades(dataForGradeCalculation);
            try
            {
                if (calculatedGrade != null)
                {
                    _context.ExamResults.Add(new ExamResult
                    {
                        D = calculatedGrade.D,
                        H = calculatedGrade.H,
                        O = calculatedGrade.O,
                        R = calculatedGrade.R,
                        ExamId = model.ExamId,
                        UserId = userId
                    });
                    foreach (var result in calculatedGrade.Grades)
                    {
                        _context.ProfessionalGrades.Add(new ProfessionalGrade
                        {
                            CompetencyId = result.CompetencyId,
                            ExamId = model.ExamId,
                            Grade = result.Grade,
                            StringGrade = result.StringGrade,
                            UserId = userId
                        });
                    }
                }
            
            var finishedExam = new UsersFinishedExams { ExamId = model.ExamId, UserId = userId };
                _context.UsersFinishedExams.Add(finishedExam);
            await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var t = ex.Message;
                var tt = t;
            }

        }

        public async Task<List<AllResultsModel>> GetAllResults()
        {
            var result = new List<AllResultsModel>();
            var examResults = await _context.ProfessionalGrades.ToListAsync();
            var usersIds = examResults.Select(x=>x.UserId).ToList();
            var examsIds = examResults.Select(x => x.ExamId).ToList();
            var users = await _userManager.Users.Where(x => usersIds.Contains(x.Id)).Select(x => new { x.Id, x.UserName }).ToListAsync();
            var exams = await _context.Exams.Where(x => examsIds.Contains(x.Id)).ToListAsync();
            foreach (var item in examResults)
            {
                result.Add(new AllResultsModel
                {
                    UserId = item.UserId,
                    ExamId = item.ExamId,
                    UserName = users.Where(x => x.Id == item.UserId).Select(x => x.UserName).FirstOrDefault(),
                    ExamName = exams.Where(x => x.Id == item.ExamId).Select(x => x.ExamName).FirstOrDefault(),
                    ExamResult = item.StringGrade
                });
            }
            return result;
        }

        private async Task<ProfessionalGradeInput> GetDataForGradeCalculation(List<ExamDichotomousGrades> model)
        {
            var result = new ProfessionalGradeInput();
            var examId = model.FirstOrDefault().ExamId;
            result.ExamId = examId;
            result.Threshold = await _context.Exams.Where(x => x.Id == examId).Select(x => x.Threshold).FirstOrDefaultAsync();
            var resultCompetencies = new List<ProfessionalGradeInputCompetency>();
            var examCompetencies = await _context.ExamsCompetencies.Where(x => x.ExamId == examId).Select(x => x.CompetencyId).ToListAsync();
            var competencies = await _context.Competencies.Where(x => examCompetencies.Contains(x.Id)).ToListAsync();
            var questions = await _context.Questions.Where(x => x.ExamId == examId).ToListAsync();
            foreach (var competency in competencies)
            {
                var competencyIndicators = await _context.CompetenciesIndicators.Where(x => x.CompetencyId == competency.Id).Select(x => x.IndicatorId).ToListAsync();
                var indicators = await _context.Indicators.Where(x => competencyIndicators.Contains(x.Id)).ToListAsync();
                var resultIndicators = new List<ProfessionalGradeInputIndicator>();
                foreach (var indicator in indicators)
                {
                    var resultTasks = new List<ProfessionalGradeInputTask>();
                    var indicatorQuestions = questions.Where(x => x.IndicatorId == indicator.Id).ToList();
                    foreach (var question in indicatorQuestions)
                    {
                        var taskGrade = model.Where(x => x.QuestionId == question.Id).Select(x => x.Grade).FirstOrDefault();

                        resultTasks.Add(new ProfessionalGradeInputTask
                        {
                            ExamId = examId,
                            TaskGrade = taskGrade,
                            TaskId = question.Id
                        });
                    }
                    resultIndicators.Add(new ProfessionalGradeInputIndicator
                    {
                        IndicatorId = indicator.Id,
                        professionalGradeInputTasks = resultTasks
                    });
                }
                resultCompetencies.Add(new ProfessionalGradeInputCompetency
                {
                    CompetencyName = competency.CompetencyName,
                    Id = competency.Id,
                    Indicators = resultIndicators
                });
            }
            result.Competencies = resultCompetencies;
            return result;
        }

        public async Task<List<Indicator>> GetIndicatorsByCompetency(int CompetencyId)
        {
            var indicatorsIds = await _context.CompetenciesIndicators.Where(x => x.CompetencyId == CompetencyId).Select(x => x.IndicatorId).ToListAsync();
            var result = await _context.Indicators.Where(x => indicatorsIds.Contains(x.Id)).ToListAsync();
            return result;
        }

        public async Task<List<Competency>> GetCompetencies()
        {
            return await _context.Competencies.ToListAsync();
        }
        public async Task<List<Indicator>> GetIndicators()
        {
            return await _context.Indicators.ToListAsync();
        }

        public async Task<ExamViewModel> GetExamById(int examId)
        {
            var exam = await _context.Exams.Where(x => x.Id == examId).Select(x => new ExamViewModel
            {
                ExamId = x.Id,
                ExamName = x.ExamName
            }).FirstOrDefaultAsync();

            if (exam == null)
                throw new Exception("Такого экзамена не существует");

            var questions = await _context.Questions.Where(x => x.ExamId == exam.ExamId).Select(x => new QuestionsViewModel
            {
                QuestionId = x.Id,
                QuestionText = x.QuestionText
            }).ToListAsync();

            for (int i = 0; i < questions.Count; i++)
            {
                var questionAnswers = await _context.Answers.Where(x => x.QuestionId == questions[i].QuestionId).Select(x => new AnswersViewModel
                {
                    AnswerId = x.Id,
                    AnswerText = x.AnswerText,
                    AnswerIsCorrect = x.IsCorrect
                }).ToListAsync();
                questions[i].Answers = questionAnswers;
            }
            exam.Questions = questions;
            return exam;
        }
        public async Task CreateNewIndicator(string indicatorName)
        {
            var check = await _context.Indicators.Where(x => x.IndicatorName == indicatorName).FirstOrDefaultAsync();
            if (check != null)
            {
                throw new Exception();
            }
            var newIndicator = new Indicator
            {
                IndicatorName = indicatorName
            };
            _context.Indicators.Add(newIndicator);
            await _context.SaveChangesAsync();
        }

        public async Task CreateCompetencyIndicators(CompetencyIndicatorsModel model)
        {
            var check = await _context.CompetenciesIndicators.Where(x => x.CompetencyId == model.CompetencyId).ToListAsync();
            if (check.Count > 0)
            {
                var itemsToRemove = new List<CompetenciesIndicators>();
                foreach (var indicator in check)
                {
                    itemsToRemove.Add(new CompetenciesIndicators
                    {
                        CompetencyId = model.CompetencyId,
                        IndicatorId = indicator.IndicatorId
                    });
                }
                _context.CompetenciesIndicators.RemoveRange(itemsToRemove);
            }
            foreach (var indicator in model.IndicatorIds)
            {
                _context.CompetenciesIndicators.Add(new CompetenciesIndicators
                {
                    CompetencyId = model.CompetencyId,
                    IndicatorId = indicator
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task CreateNewCompetency(string competencyName)
        {
            var check = await _context.Competencies.Where(x => x.CompetencyName == competencyName).FirstOrDefaultAsync();
            if (check != null)
            {
                throw new Exception();
            }
            var newCompetency = new Competency
            {
                CompetencyName = competencyName
            };
            _context.Competencies.Add(newCompetency);
            await _context.SaveChangesAsync();
        }

        public async Task SaveExam(ExamCreationDataModel model)
        {
            var check = await _context.Exams.Where(x => x.ExamName == model.ExamName).FirstOrDefaultAsync();
            if (check != null)
            {
                throw new Exception("Экзамен с таким названием уже существует");
            }
            var newExam = new Exam { ExamName = model.ExamName, Threshold = model.Threshold, Status = (int)ExamStatusEnum.Close };
            _context.Exams.Add(newExam);
            _context.SaveChanges();
            var examCompetencies = new ExamsCompetencies { CompetencyId = model.CompetencyId, ExamId = newExam.Id };
            _context.ExamsCompetencies.Add(examCompetencies);
            _context.SaveChanges();
            foreach (var question in model.Questions)
            {
                var newQuestion = new Question
                {
                    ExamId = newExam.Id,
                    IndicatorId = question.QuestionIndicator,
                    QuestionText = question.QuestionText,
                };
                _context.Questions.Add(newQuestion);
                _context.SaveChanges();
                foreach (var answer in question.Answers)
                {
                    _context.Answers.Add(new Answer
                    {
                        QuestionId = newQuestion.Id,
                        IsCorrect = answer.AnswerIsCorrect,
                        AnswerText = answer.AnswerText
                    });
                    _context.SaveChanges();
                }
            }
        }
    }
}
