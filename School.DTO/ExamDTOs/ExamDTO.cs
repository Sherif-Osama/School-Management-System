namespace School.DTO.ExamDTOs
{
    public class ExamDTO
    {
        public int ExamID { get; set; }

        public int ClassSubjectID { get; set; }

        public int ExamTypeID { get; set; }

        public DateOnly ExamDate { get; set; }

        public decimal TotalMarks { get; set; }
    }
}