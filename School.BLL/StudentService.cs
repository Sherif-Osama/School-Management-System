using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.StudentsDTOs.Requests;
using School.DTO.StudentsDTOs.Responses;

namespace School.BLL
{
    public class StudentService : IStudentService
    {
        private readonly IStudentData _studentData;
        private readonly IPersonData _personData;
        private readonly IClassData _classData;
        private readonly ITeacherData _teacherData;
        private readonly IParentData _parentData;

        public StudentService(IStudentData studentData, IPersonData personData, IClassData classData, ITeacherData teacherData, IParentData parentData)
        {
            _studentData = studentData;
            _personData = personData;
            _classData = classData;
            _teacherData = teacherData;
            _parentData = parentData;
        }

        #region Helpers Methods
        private static void ValidateEnrollmentDate(DateTime enrollmentDate)
        {
            if (enrollmentDate == default || enrollmentDate > DateTime.Today)
                throw new ArgumentException("Enrollment date is invalid.", nameof(enrollmentDate));
        }

        private static void ValidateStudent(CreateStudentRequest student)
        {
            ArgumentNullException.ThrowIfNull(student);
            ValidationHelper.ValidateId(student.PersonID);
            ValidationHelper.ValidateId(student.ClassID);
            ValidationHelper.ValidateId(student.StatusID);
            ValidateEnrollmentDate(student.EnrollmentDate);

        }

        private static void ValidateStudent(UpdateStudentRequest student)
        {
            ArgumentNullException.ThrowIfNull(student);
            ValidationHelper.ValidateId(student.ClassID);
            ValidationHelper.ValidateId(student.StatusID);
            ValidateEnrollmentDate(student.EnrollmentDate);
        }
        // A person who is already a Teacher cannot also be registered as a Student.
        private async Task EnsurePersonIsNotTeacherAsync(int personId)
        {
            if (await _teacherData.GetTeacherByPersonIdAsync(personId) != null)
                throw new InvalidOperationException($"Person ID {personId} is already registered as a teacher.");
        }

        // A person who is already a Parent cannot also be registered as a Student.
        private async Task EnsurePersonIsNotParentAsync(int personId)
        {
            if (await _parentData.GetParentByPersonIdAsync(personId) != null)
                throw new InvalidOperationException($"Person ID {personId} is already registered as a parent.");
        }

        private async Task<StudentResponse> EnsureStudentExistsAsync(int studentId)
        {
            var student = await _studentData.GetStudentByIdAsync(studentId);

            if (student == null)
                throw new KeyNotFoundException($"Student with ID {studentId} does not exist.");

            return student;
        }

        private async Task EnsureClassHasAvailableCapacityAsync(int classID)
        {
            if (!await _classData.HasClassAvailableCapacityAsync(classID))
                throw new InvalidOperationException("The class has reached its maximum capacity.");
        }

        #endregion

        #region Public Methods
        public async Task<List<StudentResponse>> GetAllStudentsAsync()
        {
            return await _studentData.GetAllStudentsAsync();
        }

        public async Task<StudentResponse?> GetStudentByIdAsync(int studentId)
        {
            ValidationHelper.ValidateId(studentId);

            StudentResponse? student = await _studentData.GetStudentByIdAsync(studentId);

            if (student == null)
                throw new KeyNotFoundException($"Student with ID {studentId} does not exist.");

            return student;
        }

        public async Task<StudentResponse?> GetStudentByPersonIdAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            StudentResponse? student = await _studentData.GetStudentByPersonIdAsync(personId);

            if (student == null)
                throw new KeyNotFoundException($"Student with person ID {personId} does not exist.");

            return student;
        }

        public async Task<int> AddStudentAsync(CreateStudentRequest student)
        {
            ValidateStudent(student);

            await EnsureHelper.EnsureExistsAsync(_personData.IsPersonExistAsync, student.PersonID, "Person");

            await EnsureHelper.EnsureExistsAsync(_classData.IsClassExistAsync, student.ClassID, "Class");

            await EnsureHelper.EnsureUniqueAsync(_studentData.GetStudentByPersonIdAsync, student.PersonID);

            await EnsurePersonIsNotTeacherAsync(student.PersonID);

            await EnsurePersonIsNotParentAsync(student.PersonID);

            await EnsureClassHasAvailableCapacityAsync(student.ClassID);

            int newStudentId = await _studentData.AddStudentAsync(student);

            if (newStudentId <= 0)
                throw new InvalidOperationException("Failed to add student.");

            return newStudentId;
        }

        public async Task<bool> UpdateStudentAsync(int studentId, UpdateStudentRequest student)
        {
            ValidateStudent(student);

            ValidationHelper.ValidateId(studentId);

            var currentStudent = await EnsureStudentExistsAsync(studentId);

            await EnsureHelper.EnsureExistsAsync(_classData.IsClassExistAsync, student.ClassID, "Class");

            if (student.ClassID != currentStudent.ClassID)
            {
                await EnsureClassHasAvailableCapacityAsync(student.ClassID);
            }

            bool isUpdated = await _studentData.UpdateStudentAsync(studentId, student);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update student with ID {studentId}");

            return isUpdated;
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            ValidationHelper.ValidateId(studentId);

            await EnsureStudentExistsAsync(studentId);

            bool isDeleted = await _studentData.DeleteStudentAsync(studentId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete student with ID {studentId}");

            return isDeleted;
        }
        #endregion
    }
}