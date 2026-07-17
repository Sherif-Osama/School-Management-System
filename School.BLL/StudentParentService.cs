using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.StudentParentDTOs;

namespace School.BLL
{
    public class StudentParentService
    {
        private readonly IStudentParentData _studentParentData;
        private readonly IStudentData _studentData;
        private readonly IParentData _parentData;

        public StudentParentService(IStudentParentData studentParentData, IStudentData studentData, IParentData parentData)
        {
            _studentParentData = studentParentData;
            _studentData = studentData;
            _parentData = parentData;
        }

        #region Private Helpers

        private static void ValidateRelation(StudentParentDTO relation)
        {
            ArgumentNullException.ThrowIfNull(relation);

            if (relation.StudentID <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(relation.StudentID),
                    "Student ID must be greater than zero.");

            if (relation.ParentID <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(relation.ParentID),
                    "Parent ID must be greater than zero.");
        }

        private async Task EnsureStudentExistsAsync(int studentId)
        {
            if (!await _studentData.IsStudentExistAsync(studentId))
                throw new InvalidOperationException(
                    $"Student with ID {studentId} does not exist.");
        }

        private async Task EnsureParentExistsAsync(int parentId)
        {
            if (!await _parentData.IsParentExistAsync(parentId))
                throw new InvalidOperationException(
                    $"Parent with ID {parentId} does not exist.");
        }

        private async Task EnsureRelationDoesNotExistAsync(StudentParentDTO relation)
        {
            if (await _studentParentData.IsStudentParentExistAsync(relation))
                throw new InvalidOperationException(
                    "This student is already linked to this parent.");
        }

        private async Task EnsureRelationExistsAsync(StudentParentDTO relation)
        {
            if (!await _studentParentData.IsStudentParentExistAsync(relation))
                throw new InvalidOperationException(
                    "The relationship does not exist.");
        }

        #endregion

        #region Public Methods

        public async Task<List<StudentParentDetailsDTO>> GetAllStudentParentsAsync()
        {
            return await _studentParentData.GetAllStudentParentsAsync();
        }

        public async Task<List<StudentParentDetailsDTO>> GetParentsByStudentIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentOutOfRangeException(nameof(studentId));

            await EnsureStudentExistsAsync(studentId);
            return await _studentParentData.GetParentsByStudentIdAsync(studentId);
        }

        public async Task<List<StudentParentDetailsDTO>> GetStudentsByParentIdAsync(int parentId)
        {
            if (parentId <= 0)
                throw new ArgumentOutOfRangeException(nameof(parentId));

            await EnsureParentExistsAsync(parentId);

            return await _studentParentData.GetStudentsByParentIdAsync(parentId);
        }

        public async Task<bool> AddStudentParentAsync(StudentParentDTO relation)
        {
            ValidateRelation(relation);

            await EnsureStudentExistsAsync(relation.StudentID);

            await EnsureParentExistsAsync(relation.ParentID);

            await EnsureRelationDoesNotExistAsync(relation);

            return await _studentParentData.AddStudentParentAsync(relation);
        }

        public async Task<bool> DeleteStudentParentAsync(StudentParentDTO relation)
        {
            ValidateRelation(relation);
            await EnsureStudentExistsAsync(relation.StudentID);
            await EnsureParentExistsAsync(relation.ParentID);
            await EnsureRelationExistsAsync(relation);
            return await _studentParentData.DeleteStudentParentAsync(relation);
        }
        #endregion
    }
}