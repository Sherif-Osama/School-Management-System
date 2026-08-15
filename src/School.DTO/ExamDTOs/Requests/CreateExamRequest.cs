namespace School.DTO.ExamDTOs.Requests
{
    public class CreateExamRequest
    {
        public int ClassSubjectID { get; set; }

        public int ExamTypeID { get; set; }

        public DateOnly ExamDate { get; set; }

        public decimal TotalMarks { get; set; }
    }
}
