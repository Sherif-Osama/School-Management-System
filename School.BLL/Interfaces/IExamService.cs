using School.DTO.ExamDTOs;
using School.DTO.ExamDTOs.Requests;

namespace School.BLL.Interfaces
{
    public interface IExamService
    {
        Task<int> AddExamAsync(CreateExamRequest exam);
        Task<bool> DeleteExamAsync(int examId);
        Task<List<ExamResponse>> GetAllExamsAsync();
        Task<ExamResponse> GetExamByIdAsync(int examId);
        Task<List<ExamResponse>> GetExamsByClassIdAsync(int classId);
        Task<List<ExamResponse>> GetExamsBySubjectIdAsync(int subjectId);
        Task<List<ExamResponse>> GetExamsByTeacherIdAsync(int teacherId);
        Task<bool> UpdateExamAsync(int examId, UpdateExamRequest exam);
    }
}