using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.TeachersDTOs;

namespace School.BLL
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherData _teacherData;
        private readonly IPersonData _personData;
        private readonly IStudentData _studentData;
        public TeacherService(ITeacherData teacherData, IPersonData personData, IStudentData studentData)
        {
            _studentData = studentData;
            _teacherData = teacherData;
            _personData = personData;
        }

        #region Private Helpers

        private static void ValidateTeacher(TeacherDTO teacher)
        {
            ArgumentNullException.ThrowIfNull(teacher);

            if (teacher.PersonID <= 0)
                throw new ArgumentException("A valid Person ID is required.", nameof(teacher.PersonID));

            if (teacher.HireDate == default || teacher.HireDate > DateTime.Today)
                throw new ArgumentException("Hire date is invalid.", nameof(teacher.HireDate));

            if (teacher.Salary <= 0)
                throw new ArgumentException("Salary must be greater than zero.", nameof(teacher.Salary));
        }

        private static void ValidateTeacherId(int teacherId)
        {
            if (teacherId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(teacherId),
                    "Teacher ID must be greater than zero.");
        }

        private static void ValidatePersonId(int personId)
        {
            if (personId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(personId),
                    "Person ID must be greater than zero.");
        }

        private async Task EnsureTeacherExistsAsync(int teacherId)
        {
            if (!await _teacherData.IsTeacherExistAsync(teacherId))
                throw new InvalidOperationException(
                    $"Teacher with ID {teacherId} does not exist.");
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new InvalidOperationException(
                    $"Person with ID {personId} does not exist.");
        }

        private async Task EnsurePersonIsNotTeacherAsync(int personId, int? currentTeacherId = null)
        {
            TeacherDetailsDTO? teacher =
                await _teacherData.GetTeacherByPersonIdAsync(personId);

            if (teacher == null)
                return;

            if (currentTeacherId.HasValue &&
                teacher.TeacherID == currentTeacherId.Value)
                return;

            throw new InvalidOperationException(
                $"Person ID {personId} is already linked to another teacher.");
        }

        private async Task EnsurePersonIsNotStudentAsync(int personId)
        {
            if (await _studentData.GetStudentByPersonIdAsync(personId) != null)
                throw new InvalidOperationException(
                    $"Person ID {personId} is already registered as a student.");
        }
        #endregion

        #region Public Methods

        public async Task<List<TeacherDetailsDTO>> GetAllTeachersAsync()
        {
            return await _teacherData.GetAllTeachersAsync();
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByIdAsync(int teacherId)
        {
            ValidateTeacherId(teacherId);

            return await _teacherData.GetTeacherByIdAsync(teacherId);
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByPersonIdAsync(int personId)
        {
            ValidatePersonId(personId);

            return await _teacherData.GetTeacherByPersonIdAsync(personId);
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByNationalIdAsync(string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException("National ID is required.", nameof(nationalId));

            return await _teacherData.GetTeacherByNationalIdAsync(nationalId);
        }

        public async Task<int> AddTeacherAsync(TeacherDTO teacher)
        {
            ValidateTeacher(teacher);

            await EnsurePersonExistsAsync(teacher.PersonID);

            await EnsurePersonIsNotTeacherAsync(teacher.PersonID);

            await EnsurePersonIsNotStudentAsync(teacher.PersonID);

            return await _teacherData.AddTeacherAsync(teacher);
        }

        public async Task<bool> UpdateTeacherAsync(TeacherDTO teacher)
        {
            ValidateTeacher(teacher);

            ValidateTeacherId(teacher.TeacherID);

            await EnsureTeacherExistsAsync(teacher.TeacherID);

            await EnsurePersonExistsAsync(teacher.PersonID);

            await EnsurePersonIsNotTeacherAsync(
                teacher.PersonID,
                teacher.TeacherID);

            await EnsurePersonIsNotStudentAsync(teacher.PersonID);

            return await _teacherData.UpdateTeacherAsync(teacher);
        }

        public async Task<bool> DeleteTeacherAsync(int teacherId)
        {
            ValidateTeacherId(teacherId);

            await EnsureTeacherExistsAsync(teacherId);

            return await _teacherData.DeleteTeacherAsync(teacherId);
        }

        public async Task<bool> IsTeacherExistAsync(int teacherId)
        {
            ValidateTeacherId(teacherId);

            return await _teacherData.IsTeacherExistAsync(teacherId);
        }

        #endregion
    }
}