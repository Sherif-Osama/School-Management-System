using School.DTO.ExamDTOs;
using School.DTO.ExamDTOs.Requests;

namespace School.DAL.Interfaces
{
    public interface IExamData
    {
        Task<int> AddExamAsync(CreateExamRequest exam);
        Task<bool> DeleteExamAsync(int examId);
        Task<List<ExamResponse>> GetAllExamsAsync();
        Task<ExamResponse?> GetExamByIdAsync(int examId);
        Task<List<ExamResponse>> GetExamsByClassIdAsync(int classId);
        Task<List<ExamResponse>> GetExamsBySubjectIdAsync(int subjectId);
        Task<List<ExamResponse>> GetExamsByTeacherIdAsync(int teacherId);
        Task<bool> IsExamExistAsync(int examId);
        Task<bool> UpdateExamAsync(int examId, UpdateExamRequest exam);
        Task<bool> IsExamDuplicate(int classSubjectId, int examTypeId, int? examId = null);
    }
}