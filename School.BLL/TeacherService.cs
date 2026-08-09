using School.BLL.Common;
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
        private static int MinnationalIdLength => 14;
        private static int MaxnationalIdLength => 20;
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

            ValidationHelper.ValidateId(teacher.PersonID);

            if (teacher.HireDate == default || teacher.HireDate > DateTime.Today)
                throw new ArgumentException("Hire date is invalid.", nameof(teacher.HireDate));

            if (teacher.Salary <= 0)
                throw new ArgumentException("Salary must be greater than zero.", nameof(teacher.Salary));
        }

        private async Task EnsureTeacherExistsAsync(int teacherId)
        {
            if (!await _teacherData.IsTeacherExistAsync(teacherId))
                throw new KeyNotFoundException($"Teacher with ID {teacherId} does not exist.");
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new KeyNotFoundException($"Person with ID {personId} does not exist.");
        }

        private async Task EnsurePersonIsNotTeacherAsync(int personId, int? currentTeacherId = null)
        {
            TeacherDetailsDTO? teacher = await _teacherData.GetTeacherByPersonIdAsync(personId);

            if (teacher == null)
                return;

            if (currentTeacherId.HasValue &&
                teacher.TeacherID == currentTeacherId.Value)
                return;

            throw new InvalidOperationException($"Person ID {personId} is already linked to another teacher.");
        }

        private async Task EnsurePersonIsNotStudentAsync(int personId)
        {
            if (await _studentData.GetStudentByPersonIdAsync(personId) != null)
                throw new InvalidOperationException($"Person ID {personId} is already registered as a student.");
        }
        #endregion

        #region Public Methods

        public async Task<List<TeacherDetailsDTO>> GetAllTeachersAsync()
        {
            return await _teacherData.GetAllTeachersAsync();
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByIdAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            TeacherDetailsDTO? teacher = await _teacherData.GetTeacherByIdAsync(teacherId);

            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with ID {teacherId} does not exist.");

            return teacher;
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByPersonIdAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            TeacherDetailsDTO? teacher = await _teacherData.GetTeacherByPersonIdAsync(personId);

            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with Person ID {personId} does not exist.");

            return teacher;
        }

        public async Task<TeacherDetailsDTO?> GetTeacherByNationalIdAsync(string nationalId)
        {
            nationalId = ValidationHelper.ValidateString(nationalId, nameof(nationalId), MinnationalIdLength, MaxnationalIdLength);

            TeacherDetailsDTO? teacher = await _teacherData.GetTeacherByNationalIdAsync(nationalId);

            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with National ID '{nationalId}' does not exist.");

            return teacher;
        }

        public async Task<int> AddTeacherAsync(TeacherDTO teacher)
        {
            ValidateTeacher(teacher);

            await EnsurePersonExistsAsync(teacher.PersonID);
            await EnsurePersonIsNotTeacherAsync(teacher.PersonID);
            await EnsurePersonIsNotStudentAsync(teacher.PersonID);

            int newTeacherId = await _teacherData.AddTeacherAsync(teacher);

            if (newTeacherId <= 0)
                throw new InvalidOperationException("Failed to add teacher.");

            return newTeacherId;
        }

        public async Task<bool> UpdateTeacherAsync(TeacherDTO teacher)
        {
            ValidateTeacher(teacher);
            ValidationHelper.ValidateId(teacher.TeacherID);

            await EnsureTeacherExistsAsync(teacher.TeacherID);
            await EnsurePersonExistsAsync(teacher.PersonID);
            await EnsurePersonIsNotTeacherAsync(teacher.PersonID, teacher.TeacherID);
            await EnsurePersonIsNotStudentAsync(teacher.PersonID);

            bool isUpdated = await _teacherData.UpdateTeacherAsync(teacher);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update teacher with ID {teacher.TeacherID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteTeacherAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            await EnsureTeacherExistsAsync(teacherId);

            bool isDeleted = await _teacherData.DeleteTeacherAsync(teacherId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete teacher with ID {teacherId}.");

            return isDeleted;
        }

        public async Task<bool> IsTeacherExistAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            return await _teacherData.IsTeacherExistAsync(teacherId);
        }
        #endregion
    }
}