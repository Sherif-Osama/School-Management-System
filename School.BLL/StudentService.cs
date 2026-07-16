using School.DAL;
using School.DTO.StudentsDTOs;

namespace School.BLL
{
    public class StudentService
    {
        private readonly StudentData _studentData;
        private readonly PersonData _personData;
        private readonly ClassData _classData;
        //Add ClassData if needed for additional validation or operations
        public StudentService(StudentData studentData, PersonData personData, ClassData classData)
        {
            _studentData = studentData;
            _personData = personData;
            _classData = classData;
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
                throw new ArgumentOutOfRangeException(nameof(studentId), "Student ID must be greater than zero.");
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new InvalidOperationException(
                    $"Person with ID {personId} does not exist.");
        }

        private async Task EnsureClassExistsAsync(int classId)
        {
            if (!await _classData.IsClassExistAsync(classId))
                throw new InvalidOperationException(
                    $"Class with ID {classId} does not exist.");
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

        private async Task EnsureStudentExistsAsync(int studentId)
        {
            if (!await _studentData.IsStudentExistAsync(studentId))
                throw new InvalidOperationException($"Student with ID {studentId} does not exist.");
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
            return await _studentData.GetStudentByIdAsync(studentId);
        }

        public async Task<StudentDetailsDTO?> GetStudentByPersonIdAsync(int personId)
        {
            if (personId <= 0)
                throw new ArgumentException("Person ID must be greater than zero.", nameof(personId));

            return await _studentData.GetStudentByPersonIdAsync(personId);
        }

        public async Task<int> AddStudentAsync(StudentDTO student)
        {
            ValidateStudent(student);

            await EnsurePersonExistsAsync(student.PersonID);

            await EnsureClassExistsAsync(student.ClassID);

            await EnsurePersonIsNotStudentAsync(student.PersonID);

            await EnsureClassHasAvailableCapacityAsync(student.ClassID);

            return await _studentData.AddStudentAsync(student);
        }

        public async Task<bool> UpdateStudentAsync(StudentDTO student)
        {
            ValidateStudent(student);

            ValidateStudentId(student.StudentID);

            await EnsureStudentExistsAsync(student.StudentID);

            await EnsurePersonExistsAsync(student.PersonID);

            await EnsureClassExistsAsync(student.ClassID);

            await EnsurePersonIsNotStudentAsync(
                student.PersonID,
                student.StudentID);

            return await _studentData.UpdateStudentAsync(student);
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            ValidateStudentId(studentId);

            await EnsureStudentExistsAsync(studentId);

            return await _studentData.DeleteStudentAsync(studentId);
        }
        #endregion
    }
}