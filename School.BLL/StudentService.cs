using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.StudentsDTOs;

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
        private static void ValidateStudent(StudentDTO student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (student.PersonID <= 0)
                throw new ArgumentException("A valid Person ID is required.", nameof(student.PersonID));

            if (student.ClassID <= 0)
                throw new ArgumentException("A valid Class ID is required.", nameof(student.ClassID));

            if (student.StatusID <= 0)
                throw new ArgumentException("A valid Status ID is required.", nameof(student.StatusID));

            if (student.EnrollmentDate == default || student.EnrollmentDate > DateTime.Now)
                throw new ArgumentException("Enrollment date is invalid.", nameof(student.EnrollmentDate));
        }

        private static void ValidateStudentId(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("Student ID must be greater than zero.", nameof(studentId));
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new KeyNotFoundException($"Person with ID {personId} does not exist.");
        }

        private async Task EnsureClassExistsAsync(int classId)
        {
            if (!await _classData.IsClassExistAsync(classId))
                throw new KeyNotFoundException($"Class with ID {classId} does not exist.");
        }

        private async Task EnsurePersonIsNotStudentAsync(int personId, int? currentStudentId = null)
        {
            StudentDetailsDTO? student = await _studentData.GetStudentByPersonIdAsync(personId);

            if (student == null)
                return;

            if (currentStudentId.HasValue && student.StudentID == currentStudentId.Value)
                return;

            throw new InvalidOperationException($"Person ID {personId} is already linked to another student.");
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

        private async Task EnsureStudentExistsAsync(int studentId)
        {
            if (!await _studentData.IsStudentExistAsync(studentId))
                throw new KeyNotFoundException($"Student with ID {studentId} does not exist.");
        }

        private async Task EnsureClassHasAvailableCapacityAsync(int classID)
        {
            if (!await _classData.HasClassAvailableCapacityAsync(classID))
                throw new InvalidOperationException("The class has reached its maximum capacity.");
        }

        #endregion

        #region Public Methods
        public async Task<List<StudentDetailsDTO>> GetAllStudentsAsync()
        {
            return await _studentData.GetAllStudentsAsync();
        }

        public async Task<StudentDetailsDTO?> GetStudentByIdAsync(int studentId)
        {
            ValidateStudentId(studentId);

            StudentDetailsDTO? studentDetailsDTO = await _studentData.GetStudentByIdAsync(studentId);

            if (studentDetailsDTO == null)
                throw new KeyNotFoundException($"Student with ID {studentId} does not exist.");

            return studentDetailsDTO;
        }

        public async Task<StudentDetailsDTO?> GetStudentByPersonIdAsync(int personId)
        {
            if (personId <= 0)
                throw new ArgumentException("Person ID must be greater than zero.", nameof(personId));

            StudentDetailsDTO? studentDetailsDTO = await _studentData.GetStudentByPersonIdAsync(personId);

            if (studentDetailsDTO == null)
                throw new KeyNotFoundException($"Student with person ID {personId} does not exist.");

            return studentDetailsDTO;
        }

        public async Task<int> AddStudentAsync(StudentDTO student)
        {
            ValidateStudent(student);

            await EnsurePersonExistsAsync(student.PersonID);

            await EnsureClassExistsAsync(student.ClassID);

            await EnsurePersonIsNotStudentAsync(student.PersonID);

            await EnsurePersonIsNotTeacherAsync(student.PersonID);

            await EnsurePersonIsNotParentAsync(student.PersonID);

            await EnsureClassHasAvailableCapacityAsync(student.ClassID);

            int newStudentId = await _studentData.AddStudentAsync(student);

            if (newStudentId <= 0)
                throw new InvalidOperationException("Failed to add student.");

            return newStudentId;
        }

        public async Task<bool> UpdateStudentAsync(StudentDTO student)
        {
            ValidateStudent(student);

            ValidateStudentId(student.StudentID);

            await EnsureStudentExistsAsync(student.StudentID);

            await EnsurePersonExistsAsync(student.PersonID);

            await EnsureClassExistsAsync(student.ClassID);

            await EnsureClassHasAvailableCapacityAsync(student.ClassID);

            await EnsurePersonIsNotStudentAsync(student.PersonID, student.StudentID);

            await EnsurePersonIsNotTeacherAsync(student.PersonID);

            await EnsurePersonIsNotParentAsync(student.PersonID);

            bool isUpdated = await _studentData.UpdateStudentAsync(student);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update student with ID {student.StudentID}");

            return isUpdated;
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            ValidateStudentId(studentId);

            await EnsureStudentExistsAsync(studentId);

            bool isDeleted = await _studentData.DeleteStudentAsync(studentId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete student with ID {studentId}");

            return isDeleted;
        }
        #endregion
    }
}