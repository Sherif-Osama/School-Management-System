using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ParentsDTOs;

namespace School.BLL
{
    public class ParentService : IParentService
    {
        private readonly IParentData _parentData;
        private readonly IPersonData _personData;
        private readonly IStudentData _studentData;

        public ParentService(IParentData parentData, IPersonData personData, IStudentData studentData)
        {
            _parentData = parentData;
            _personData = personData;
            _studentData = studentData;
        }

        #region Private Helpers

        private static void ValidateParent(ParentDTO parent)
        {
            ArgumentNullException.ThrowIfNull(parent);

            ValidatePersonId(parent.PersonID);
        }

        private static void ValidateParentId(int parentId)
        {
            if (parentId <= 0)
                throw new ArgumentException("Parent ID must be greater than zero.", nameof(parentId));
        }

        private static void ValidatePersonId(int personId)
        {
            if (personId <= 0)
                throw new ArgumentException("Person ID must be greater than zero.", nameof(personId));
        }

        private async Task EnsureParentExistsAsync(int parentId)
        {
            if (!await _parentData.IsParentExistAsync(parentId))
                throw new KeyNotFoundException(
                    $"Parent with ID {parentId} does not exist.");
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new KeyNotFoundException($"Person with ID {personId} does not exist.");
        }

        private async Task EnsurePersonIsNotParentAsync(int personId, int? currentParentId = null)
        {
            ParentDetailsDTO? parent = await _parentData.GetParentByPersonIdAsync(personId);

            if (parent == null)
                return;

            if (currentParentId.HasValue && parent.ParentID == currentParentId.Value)
                return;

            throw new InvalidOperationException(
                $"Person ID {personId} is already linked to another parent.");
        }

        private async Task EnsurePersonIsNotStudentAsync(int personId)
        {
            if (await _studentData.GetStudentByPersonIdAsync(personId) != null)
                throw new InvalidOperationException($"Person ID {personId} is already registered as a student.");
        }

        #endregion

        #region Public Methods

        public async Task<List<ParentDetailsDTO>> GetAllParentsAsync()
        {
            return await _parentData.GetAllParentsAsync();
        }

        public async Task<ParentDetailsDTO?> GetParentByIdAsync(int parentId)
        {
            ValidateParentId(parentId);
            ParentDetailsDTO? parentDetails = await _parentData.GetParentByIdAsync(parentId);

            if (parentDetails == null)
                throw new KeyNotFoundException($"Parent with ID {parentId} does not exist.");

            return parentDetails;
        }

        public async Task<ParentDetailsDTO?> GetParentByPersonIdAsync(int personId)
        {
            ValidatePersonId(personId);

            ParentDetailsDTO? parentDetails = await _parentData.GetParentByPersonIdAsync(personId);

            if (parentDetails == null)
                throw new KeyNotFoundException($"Parent with Person ID {personId} does not exist.");

            return parentDetails;
        }

        public async Task<ParentDetailsDTO?> GetParentByNationalIdAsync(string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException("National ID is required.", nameof(nationalId));

            nationalId = nationalId.Trim();

            ParentDetailsDTO? parentDetails = await _parentData.GetParentByNationalIdAsync(nationalId);

            if (parentDetails == null)
                throw new KeyNotFoundException($"Parent with National ID {nationalId} does not exist.");

            return parentDetails;
        }

        public async Task<int> AddParentAsync(ParentDTO parent)
        {
            ValidateParent(parent);

            await EnsurePersonExistsAsync(parent.PersonID);

            await EnsurePersonIsNotParentAsync(parent.PersonID);

            await EnsurePersonIsNotStudentAsync(parent.PersonID);

            int newParentId = await _parentData.AddParentAsync(parent);

            if (newParentId <= 0)
                throw new InvalidOperationException("Failed to add parent.");

            return newParentId;
        }

        public async Task<bool> UpdateParentAsync(ParentDTO parent)
        {
            ValidateParent(parent);

            ValidateParentId(parent.ParentID);

            await EnsureParentExistsAsync(parent.ParentID);

            await EnsurePersonExistsAsync(parent.PersonID);

            await EnsurePersonIsNotParentAsync(parent.PersonID, parent.ParentID);

            await EnsurePersonIsNotStudentAsync(parent.PersonID);

            bool isUpdated = await _parentData.UpdateParentAsync(parent);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update parent with ID {parent.ParentID}");

            return isUpdated;
        }

        public async Task<bool> DeleteParentAsync(int parentId)
        {
            ValidateParentId(parentId);

            await EnsureParentExistsAsync(parentId);
            bool isDeleted = await _parentData.DeleteParentAsync(parentId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete parent with ID {parentId}");

            return isDeleted;
        }

        public async Task<bool> IsParentExistAsync(int parentId)
        {
            ValidateParentId(parentId);

            return await _parentData.IsParentExistAsync(parentId);
        }

        #endregion
    }
}