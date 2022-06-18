using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VKR.Models.AuthorizationModels;
using VKR.Models.ExamModels;
using VKR.Models.NewsModels;
using VKR.Models.ProfessionalGradeModels;
using VKR.Models.RequestModels;

namespace VKR.Models
{
    public class DBContext : IdentityDbContext<IdentityUser>
    {
        public DBContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CompetenciesIndicators>()
            .HasKey(p => new { p.CompetencyId, p.IndicatorId });
            modelBuilder.Entity<ExamsCompetencies>()
            .HasKey(p => new { p.ExamId, p.CompetencyId });
            modelBuilder.Entity<UsersAvailableExams>()
            .HasKey(p => new { p.UserId, p.ExamId });
            modelBuilder.Entity<UsersFinishedExams>()
            .HasKey(p => new { p.UserId, p.ExamId });
        }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<ProfessionalGrade> ProfessionalGrades { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Userdf> Users { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Competency> Competencies { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<ExamsCompetencies> ExamsCompetencies { get; set; }
        public DbSet<CompetenciesIndicators> CompetenciesIndicators { get; set; }
        public DbSet<UsersAvailableExams> UsersAvailableExams { get; set; }
        public DbSet<UsersFinishedExams> UsersFinishedExams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<ExamDichotomousGrades> ExamDichotomousGrades { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<News> News { get; set; }
        //public DbSet<AspNetUser> AspNetUsers { get; set; }
        //public DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        //public DbSet<AspNetRole> AspNetRoles { get; set; }
    }
}
