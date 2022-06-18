using Microsoft.EntityFrameworkCore;
using VKR.Core;
using VKR.Models;
using VKR.Models.ProfessionalGradeModels;

namespace VKR.Repositories.ProfessionalGrades
{
    public class ProfessionalGradeRepository : IProfessionalGradeRepository
    {
        private readonly DBContext _context;

        public ProfessionalGradeRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<List<ExamResult>> GetProfessionalGrades()
        {
            return await _context.ExamResults.Where(x=>x.Id>0).ToListAsync();
        }

        public async Task<IEnumerable<ExamResultViewData>> GetProfessionalGradeByExamAndUser(string userId, int examId)
        {
            try
            {
                var result = new List<ExamResultViewData>();
                var examResults = await _context.ExamResults.Where(x => x.UserId == userId && x.ExamId == examId).Select(x=> new ExamResultViewData
                {
                    Id = x.Id,
                    ExamId = x.ExamId,
                    UserId = x.UserId,
                    D = x.D,
                    O = x.O,
                    H = x.H,
                    R = x.R,
                    Grades = new List<ProfessionalGradeViewData>()
                }).FirstOrDefaultAsync();
                var examCompetenciesResults = await _context.ProfessionalGrades.Where(x => x.UserId == userId && x.ExamId == examId).ToListAsync();
                var examCompetenciesIds = examCompetenciesResults.Select(x => x.CompetencyId).ToList();
                var examCompetenciesNames = await _context.Competencies.Where(x => examCompetenciesIds.Contains(x.Id)).ToListAsync();
                foreach (var item in examCompetenciesResults)
                {
                    var competencyName = examCompetenciesNames.Where(x => x.Id == item.CompetencyId).Select(x => x.CompetencyName).FirstOrDefault();
                    examResults.Grades.Add(new ProfessionalGradeViewData
                    {
                        Id = item.CompetencyId,
                        CompetencyId = item.CompetencyId,
                        UserId = userId,
                        ExamId = examId,
                        Grade = item.Grade,
                        StringGrade = item.StringGrade,
                        CompetencyName = competencyName,
                    });
                }
                result.Add(examResults);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ExamResultViewData> CalculateProfessionalGrades(ProfessionalGradeInput professionalGradeInputs)
        {
            var result = new ExamResultViewData();

            var competencies = new List<ProfessionalGradeViewData>();
            foreach (var professionalGradeInput in professionalGradeInputs.Competencies)
            {
                var indicators = professionalGradeInput.Indicators.ToList();
                var indicatorScore = CalculateIndicatorScore(indicators);
                var indicatorDichotomousScore = CalculateIndicatorDichotomousScore(indicatorScore, professionalGradeInputs.Threshold);
                result.O = CalculateFormationAssessment(indicatorScore, professionalGradeInput.Id);
                result.D = CalculatePerformanceEvaluation(indicatorDichotomousScore, professionalGradeInput.Id);
                var formationAndEfficiency = CalculateFormationAndEfficiency(indicatorDichotomousScore, professionalGradeInput.Id);
                result.R = formationAndEfficiency.Formation;
                result.H = formationAndEfficiency.Efficiency;
                var grade = CalculateProfessionalGrade(result);
                competencies.Add(new ProfessionalGradeViewData
                {
                    Id = professionalGradeInput.Id,
                    CompetencyId = professionalGradeInput.Id,
                    Grade = grade,
                    ExamId = professionalGradeInputs.ExamId,
                    StringGrade = GetStringGrade(grade)
                });
            }
            result.Grades = competencies;
            return result;
        }

        private string GetStringGrade(double grade)
        {
            switch (grade)
            {
                case (int)GradeEnum.High:
                    return Constants.STRING_GRADE_HIGH;
                    break;
                case (int)GradeEnum.Middle:
                    return Constants.STRING_GRADE_MIDDLE;
                    break;
                case (int)GradeEnum.Low:
                    return Constants.STRING_GRADE_LOW;
                    break;
                default:
                    return Constants.STRING_GRADE_NOT_FORMED;
                    break;
            }
        }

        public async Task SetProfessionalGrades(IEnumerable<ExamResult> examResults)
        {
            _context.ExamResults.AddRange(examResults);
            await _context.SaveChangesAsync();
        }

        #region Calculation of grade
        private List<ProfessionalGradeIndicatorScore> CalculateIndicatorScore(List<ProfessionalGradeInputIndicator> professionalGradeInputIndicators)
        {
            var result = new List<ProfessionalGradeIndicatorScore>();
            
            foreach (var professionalGradeInputIndicator in professionalGradeInputIndicators)
            {
                var tasksSum = professionalGradeInputIndicator.professionalGradeInputTasks.Select(x=>x.TaskGrade).Sum();
                result.Add(new ProfessionalGradeIndicatorScore
                {
                    IndicatorId = professionalGradeInputIndicator.IndicatorId,
                    Score = ((10 / professionalGradeInputIndicator.professionalGradeInputTasks.Count) * tasksSum)
                });
            }
            return result;
        }

        private List<ProfessionalGradeIndicatorDichotomousScore> CalculateIndicatorDichotomousScore(List<ProfessionalGradeIndicatorScore> indicatorScore, int? threshold)
        {
            var result = new List<ProfessionalGradeIndicatorDichotomousScore>();
            foreach (var indicator in indicatorScore)
            {
                if(indicator.Score >= threshold)
                {
                    result.Add(new ProfessionalGradeIndicatorDichotomousScore
                    {
                        IndicatorId = indicator.IndicatorId,
                        Score = Constants.POSITIVE_DICHOTOMOUS_SCORE
                    });
                }
                else
                {
                    result.Add(new ProfessionalGradeIndicatorDichotomousScore
                    {
                        IndicatorId = indicator.IndicatorId,
                        Score = Constants.NEGATIVE_DICHOTOMOUS_SCORE
                    });
                }
            }
            return result;
        }//TODO ВСЕ НАЗВАНИЯ АСИНХРОННЫХ КЛАССОВ ПОМЕНЯТЬ НА НАЗВАНИЕ КЛАССА + АСИНК

        private double CalculateFormationAssessment(List<ProfessionalGradeIndicatorScore> professionalGradeIndicatorScores, int competenceId)
        {
            var gradeSum = professionalGradeIndicatorScores.Select(x => x.Score).Sum();
            var result = Math.Round((1.0 / (double)professionalGradeIndicatorScores.Count) * gradeSum,2);
            return result;
        }

        private double CalculatePerformanceEvaluation(List<ProfessionalGradeIndicatorDichotomousScore> professionalGradeIndicatorDichotomousScores, int competenceId)
        {
            var gradeSum = professionalGradeIndicatorDichotomousScores.Select(x => x.Score).Sum();
            var result = Math.Round((1.0 / (double)professionalGradeIndicatorDichotomousScores.Count) * gradeSum, 2);
            return result;
        }

        private ProfessionalGradeFormationAndEfficiency CalculateFormationAndEfficiency(List<ProfessionalGradeIndicatorDichotomousScore> professionalGradeIndicatorScores, int competencyId)
        {
            var p0 = (double)professionalGradeIndicatorScores.Where(x=>x.Score == Constants.NEGATIVE_DICHOTOMOUS_SCORE).Count() / (double)professionalGradeIndicatorScores.Count;
            var p1 = (double)professionalGradeIndicatorScores.Where(x => x.Score == Constants.POSITIVE_DICHOTOMOUS_SCORE).Count() / (double)professionalGradeIndicatorScores.Count;//Что-то не то перекинул, должны быть дихотомические оценки, а не бальные
            double h;
            if(p0 == 0)
            {
                h = 1;
            }
            else if(p1 == 0)
            {
                h= 0;
            }
            else
            {
                h = (-p0 * Math.Log(p0, Math.E)) - (p1 * Math.Log(p1, Math.E));
            }
            return new ProfessionalGradeFormationAndEfficiency
            {
                CompetencyId = competencyId,
                Formation = h,
                Efficiency = 1 - h
            };
        }

        private int CalculateProfessionalGrade(ExamResultViewData examResult)
        {
            if(examResult.H <= Constants.UNIVERSAL_VALUE_H_HIGH && examResult.D >= Constants.UNIVERSAL_VALUE_D_HIGH && examResult.R >= Constants.UNIVERSAL_VALUE_R_HIGH)
            {
                return (int)GradeEnum.High;
            }
            else if(examResult.H <= Constants.UNIVERSAL_VALUE_H_MIDDLE && examResult.D >= Constants.UNIVERSAL_VALUE_D_MIDDLE && examResult.R >= Constants.UNIVERSAL_VALUE_R_MIDDLE)
            {
                return (int)GradeEnum.Middle;
            }
            else if (examResult.H <= Constants.UNIVERSAL_VALUE_H_LOW && examResult.D >= Constants.UNIVERSAL_VALUE_D_LOW && examResult.R >= Constants.UNIVERSAL_VALUE_R_LOW)
            {
                return (int)GradeEnum.Low;
            }
            else
            {
                return (int)GradeEnum.NotFormed;
            }
        }
        #endregion
    }
}
