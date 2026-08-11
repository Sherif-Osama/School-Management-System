using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ParentsDTOs.Requests;
using School.DTO.ParentsDTOs.Responses;

namespace School.BLL
{
    public class ParentService : IParentService
    {
        private readonly IParentData _parentData;
        private readonly IPersonData _personData;
        private readonly IStudentData _studentData;
        private static int MinNationalIdLength => 14;
        private static int MaxNationalIdLength => 20;
        public ParentService(IParentData parentData, IPersonData personData, IStudentData studentData)
        {
            _parentData = parentData;
            _personData = personData;
            _studentData = studentData;
        }

        #region Private Helpers

        private static void ValidateParent(CreateParentRequest parent)
        {
            ArgumentNullException.ThrowIfNull(parent);

            ValidationHelper.ValidateId(parent.PersonID);
        }

        private async Task EnsurePersonIsNotStudentAsync(int personId)
        {
            if (await _studentData.GetStudentByPersonIdAsync(personId) != null)
                throw new InvalidOperationException($"Person ID {personId} is already registered as a student.");
        }

        #endregion

        #region Public Methods

        public async Task<List<ParentResponse>> GetAllParentsAsync()
        {
            return await _parentData.GetAllParentsAsync();
        }

        public async Task<ParentResponse?> GetParentByIdAsync(int parentId)
        {
            ValidationHelper.ValidateId(parentId);
            ParentResponse? parentDetails = await _parentData.GetParentByIdAsync(parentId);

            if (parentDetails == null)
                throw new KeyNotFoundException($"Parent with ID {parentId} does not exist.");

            return parentDetails;
        }

        public async Task<ParentResponse?> GetParentByPersonIdAsync(int personId)
        {
            ValidationHelper.ValidateId(personId);

            ParentResponse? parentDetails = await _parentData.GetParentByPersonIdAsync(personId);

            if (parentDetails == null)
                throw new KeyNotFoundException($"Parent with Person ID {personId} does not exist.");

            return parentDetails;
        }

        public async Task<ParentResponse?> GetParentByNationalIdAsync(string nationalId)
        {
            nationalId = ValidationHelper.ValidateString(nationalId, nameof(nationalId), MinNationalIdLength, MaxNationalIdLength);

            ParentResponse? parentDetails = await _parentData.GetParentByNationalIdAsync(nationalId);

            if (parentDetails == null)
                throw new KeyNotFoundException($"Parent with National ID {nationalId} does not exist.");

            return parentDetails;
        }

        public async Task<int> AddParentAsync(CreateParentRequest parent)
        {
            ValidateParent(parent);

            await EnsureHelper.EnsureExistsAsync(_personData.IsPersonExistAsync, parent.PersonID, "Person");

            await EnsureHelper.EnsureUniqueAsync(_parentData.GetParentByPersonIdAsync, parent.PersonID);

            await EnsurePersonIsNotStudentAsync(parent.PersonID);

            int newParentId = await _parentData.AddParentAsync(parent);

            if (newParentId <= 0)
                throw new InvalidOperationException("Failed to add parent.");

            return newParentId;
        }

        public async Task<bool> DeleteParentAsync(int parentId)
        {
            ValidationHelper.ValidateId(parentId);

            await EnsureHelper.EnsureExistsAsync(_parentData.IsParentExistAsync, parentId, "Parent");

            bool isDeleted = await _parentData.DeleteParentAsync(parentId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete parent with ID {parentId}");

            return isDeleted;
        }

        public async Task<bool> IsParentExistAsync(int parentId)
        {
            ValidationHelper.ValidateId(parentId);

            return await _parentData.IsParentExistAsync(parentId);
        }
        #endregion
    }
}