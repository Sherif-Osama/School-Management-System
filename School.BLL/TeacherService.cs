using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.TeachersDTOs.Requests;
using School.DTO.TeachersDTOs.Responses;

namespace School.BLL
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherData _teacherData;
        private readonly IPersonData _personData;
        private readonly IStudentData _studentData;
        private static int MinNationalIdLength => 14;
        private static int MaxNationalIdLength => 20;
        public TeacherService(ITeacherData teacherData, IPersonData personData, IStudentData studentData)
        {
            _studentData = studentData;
            _teacherData = teacherData;
            _personData = personData;
        }

        #region Private Helpers
        private static void ValidateTeacherData(DateTime hireDate, decimal salary)
        {
            if (hireDate == default || hireDate > DateTime.Today)
                throw new ArgumentException("Hire date is invalid.", nameof(hireDate));

            if (salary <= 0)
                throw new ArgumentException("Salary must be greater than zero.", nameof(salary));
        }

        private static void ValidateTeacher(CreateTeacherRequest teacher)
        {
            ArgumentNullException.ThrowIfNull(teacher);

            ValidationHelper.ValidateId(teacher.PersonID);

            ValidateTeacherData(teacher.HireDate, teacher.Salary);
        }

        private static void ValidateTeacher(UpdateTeacherRequest teacher)
        {
            ArgumentNullException.ThrowIfNull(teacher);

            ValidateTeacherData(teacher.HireDate, teacher.Salary);
        }

        private async Task EnsurePersonIsNotStudentAsync(int personId)
        {
            if (await _studentData.GetStudentByPersonIdAsync(personId) != null)
                throw new InvalidOperationException($"Person ID {personId} is already registered as a student.");
        }
        #endregion

        #region Public Methods

        public async Task<List<TeacherResponse>> GetAllTeachersAsync()
        {
            return await _teacherData.GetAllTeachersAsync();
        }

        public async Task<TeacherResponse?> GetTeacherByIdAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            TeacherResponse? teacher = await _teacherData.GetTeacherByIdAsync(teacherId);

            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with ID {teacherId} does not exist.");

            return teacher;
        }

        public async Task<TeacherResponse?> GetTeacherByPersonIdAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            TeacherResponse? teacher = await _teacherData.GetTeacherByPersonIdAsync(personId);

            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with Person ID {personId} does not exist.");

            return teacher;
        }

        public async Task<TeacherResponse?> GetTeacherByNationalIdAsync(string nationalId)
        {
            nationalId = ValidationHelper.ValidateString(nationalId, nameof(nationalId), MinNationalIdLength, MaxNationalIdLength);

            TeacherResponse? teacher = await _teacherData.GetTeacherByNationalIdAsync(nationalId);

            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with National ID '{nationalId}' does not exist.");

            return teacher;
        }

        public async Task<int> AddTeacherAsync(CreateTeacherRequest teacher)
        {
            ValidateTeacher(teacher);

            await EnsureHelper.EnsureExistsAsync(_personData.IsPersonExistAsync, teacher.PersonID, "Person");
            await EnsureHelper.EnsureUniqueAsync(_teacherData.GetTeacherByPersonIdAsync, teacher.PersonID);
            await EnsurePersonIsNotStudentAsync(teacher.PersonID);

            int newTeacherId = await _teacherData.AddTeacherAsync(teacher);

            if (newTeacherId <= 0)
                throw new InvalidOperationException("Failed to add teacher.");

            return newTeacherId;
        }

        public async Task<bool> UpdateTeacherAsync(int teacherId, UpdateTeacherRequest teacher)
        {
            ValidateTeacher(teacher);
            ValidationHelper.ValidateId(teacherId);

            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, teacherId, "Teacher");

            bool isUpdated = await _teacherData.UpdateTeacherAsync(teacherId, teacher);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update teacher with ID {teacherId}.");

            return isUpdated;
        }

        public async Task<bool> DeleteTeacherAsync(int teacherId)
        {
            ValidationHelper.ValidateId(teacherId);

            await EnsureHelper.EnsureExistsAsync(_teacherData.IsTeacherExistAsync, teacherId, "Teacher");

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