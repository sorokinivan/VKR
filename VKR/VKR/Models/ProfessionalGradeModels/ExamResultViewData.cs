namespace VKR.Models.ProfessionalGradeModels
{
    public class ExamResultViewData
    {
        //public ProfessionalGrade(int professionalGradeId, int examinerId, int examId, double grade, DateTime? gradeDate)
        //{
        //    ProfessionalGradeId = professionalGradeId;
        //    ExaminerId = examinerId;
        //    ExamId = examId;
        //    Grade = grade;
        //    GradeDate = gradeDate;
        //}
        //private int? _grade;
        private double _r;
        private double _h;
        private double _d;
        private double _o;

        public int Id { get; set; }
        public string UserId { get; set; }
        public int ExamId { get; set; }
        //public int? Grade {
        //    get
        //    {
        //        return _grade;
        //    }
        //    set
        //    {
        //        if (value > 5 || value < 2)
        //        {
        //            throw new ArgumentOutOfRangeException($"{nameof(value)} must be between 2 and 5.");
        //        }
        //        _grade = value;
        //    }
        //}

        public double R
        {
            get
            {
                return _r;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be more than 0.");
                }
                _r = value;
            }
        }
        public double H
        {
            get
            {
                return _h;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be more than 0.");
                }
                _h = value;
            }
        }
        public double D
        {
            get
            {
                return _d;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be more than 0.");
                }
                _d = value;
            }
        }

        public double O
        {
            get
            {
                return _o;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(value)} must be more than 0.");
                }
                _o = value;
            }
        }

        public List<ProfessionalGradeViewData> Grades { get; set; }
    }
}
