using School.DTO.StudentGradeDTOs.Requests;
using School.DTO.StudentGradeDTOs.Responses;

namespace School.BLL.Interfaces
{
    public interface IStudentGradeService
    {
        Task<int> AddStudentGradeAsync(CreateStudentGradeRequest studentGrade);
        Task<bool> DeleteStudentGradeAsync(int studentGradeId);
        Task<List<StudentGradeResponse>> GetAllStudentGradesAsync();
        Task<StudentGradeResponse> GetStudentGradeByIdAsync(int studentGradeId);
        Task<List<StudentGradeResponse>> GetStudentGradesByClassIdAsync(int classId);
        Task<List<StudentGradeResponse>> GetStudentGradesByExamIdAsync(int examId);
        Task<List<StudentGradeResponse>> GetStudentGradesByStudentIdAsync(int studentId);
        Task<List<StudentGradeResponse>> GetStudentGradesBySubjectIdAsync(int subjectId);
        Task<bool> UpdateStudentGradeAsync(int studentGradeId, UpdateStudentGradeRequest studentGrade);
    }
}