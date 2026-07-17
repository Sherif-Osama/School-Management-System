using School.DTO.ExamDTOs;

namespace School.BLL.Interfaces
{
    public interface IExamService
    {
        Task<int> AddExamAsync(ExamDTO exam);
        Task<bool> DeleteExamAsync(int examId);
        Task<List<ExamDetailsDTO>> GetAllExamsAsync();
        Task<ExamDetailsDTO?> GetExamByIdAsync(int examId);
        Task<List<ExamDetailsDTO>> GetExamsByClassIdAsync(int classId);
        Task<List<ExamDetailsDTO>> GetExamsBySubjectIdAsync(int subjectId);
        Task<List<ExamDetailsDTO>> GetExamsByTeacherIdAsync(int teacherId);
        Task<bool> UpdateExamAsync(ExamDTO exam);
    }
}