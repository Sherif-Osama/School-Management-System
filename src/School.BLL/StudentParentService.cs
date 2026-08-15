using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.AssociationsDTOs.StudentParentDTOs;
using School.DTO.AssociationsDTOs.StudentParentDTOs.Requests;

namespace School.BLL
{
    public class StudentParentService : IStudentParentService
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
        private static void ValidateRelation(StudentParentRequest relation)
        {
            ArgumentNullException.ThrowIfNull(relation);

            ValidationHelper.ValidateId(relation.StudentID);
            ValidationHelper.ValidateId(relation.ParentID);
        }

        private async Task EnsureRelationDoesNotExistAsync(StudentParentRequest relation)
        {
            if (await _studentParentData.IsStudentParentExistAsync(relation))
                throw new InvalidOperationException("This student is already linked to this parent.");
        }

        private async Task EnsureRelationExistsAsync(StudentParentRequest relation)
        {
            if (!await _studentParentData.IsStudentParentExistAsync(relation))
                throw new KeyNotFoundException("The relationship does not exist.");
        }

        #endregion

        #region Public Methods

        public async Task<List<StudentParentResponse>> GetAllStudentParentsAsync()
        {
            return await _studentParentData.GetAllStudentParentsAsync();
        }

        public async Task<List<StudentParentResponse>> GetParentsByStudentIdAsync(int studentId)
        {
            ValidationHelper.ValidateId(studentId);

            await EnsureHelper.EnsureExistsAsync(_studentData.IsStudentExistAsync, studentId, "student");

            return await _studentParentData.GetParentsByStudentIdAsync(studentId);
        }

        public async Task<List<StudentParentResponse>> GetStudentsByParentIdAsync(int parentId)
        {
            ValidationHelper.ValidateId(parentId);

            await EnsureHelper.EnsureExistsAsync(_parentData.IsParentExistAsync, parentId, "parent");

            return await _studentParentData.GetStudentsByParentIdAsync(parentId);
        }

        public async Task<bool> AddStudentParentAsync(StudentParentRequest relation)
        {
            ValidateRelation(relation);

            await EnsureHelper.EnsureExistsAsync(_studentData.IsStudentExistAsync, relation.StudentID, "student");

            await EnsureHelper.EnsureExistsAsync(_parentData.IsParentExistAsync, relation.ParentID, "parent");

            await EnsureRelationDoesNotExistAsync(relation);

            bool isRelationAdded = await _studentParentData.AddStudentParentAsync(relation);

            if (!isRelationAdded)
                throw new InvalidOperationException("Failed to add the student-parent relationship.");

            return isRelationAdded;
        }

        public async Task<bool> DeleteStudentParentAsync(StudentParentRequest relation)
        {
            ValidateRelation(relation);
            await EnsureHelper.EnsureExistsAsync(_studentData.IsStudentExistAsync, relation.StudentID, "Student");
            await EnsureHelper.EnsureExistsAsync(_parentData.IsParentExistAsync, relation.ParentID, "Parent");
            await EnsureRelationExistsAsync(relation);

            bool isDeleted = await _studentParentData.DeleteStudentParentAsync(relation);

            if (!isDeleted)
                throw new InvalidOperationException("Failed to delete the student-parent relationship.");

            return isDeleted;
        }
        #endregion
    }
}