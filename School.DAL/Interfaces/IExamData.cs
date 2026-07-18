using School.DTO.ExamDTOs;

namespace School.DAL.Interfaces
{
    public interface IExamData
    {
        Task<int> AddExamAsync(ExamDTO exam);
        Task<bool> DeleteExamAsync(int examId);
        Task<List<ExamDetailsDTO>> GetAllExamsAsync();
        Task<ExamDetailsDTO?> GetExamByIdAsync(int examId);
        Task<List<ExamDetailsDTO>> GetExamsByClassIdAsync(int classId);
        Task<List<ExamDetailsDTO>> GetExamsBySubjectIdAsync(int subjectId);
        Task<List<ExamDetailsDTO>> GetExamsByTeacherIdAsync(int teacherId);
        Task<bool> IsExamExistAsync(int examId);
        Task<bool> UpdateExamAsync(ExamDTO exam);
        Task<bool> IsExamDuplicate(int classSubjectId, int examTypeId, int? examId = null);
    }
}