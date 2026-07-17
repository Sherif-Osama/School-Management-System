using School.DTO.StudentGradeDetailsDTOs;
using School.DTO.StudentGradeDTOs;

namespace School.DAL.Interfaces
{
    public interface IStudentGradeData
    {
        Task<int> AddStudentGradeAsync(StudentGradeDTO studentGrade);
        Task<bool> DeleteStudentGradeAsync(int studentGradeId);
        Task<List<StudentGradeDetailsDTO>> GetAllStudentGradesAsync();
        Task<StudentGradeDetailsDTO?> GetStudentGradeByIdAsync(int studentGradeId);
        Task<List<StudentGradeDetailsDTO>> GetStudentGradesByClassIdAsync(int classId);
        Task<List<StudentGradeDetailsDTO>> GetStudentGradesByExamIdAsync(int examId);
        Task<List<StudentGradeDetailsDTO>> GetStudentGradesByStudentIdAsync(int studentId);
        Task<List<StudentGradeDetailsDTO>> GetStudentGradesBySubjectIdAsync(int subjectId);
        Task<bool> IsStudentGradeExistAsync(int studentGradeId);
        Task<bool> UpdateStudentGradeAsync(StudentGradeDTO studentGrade);
    }
}