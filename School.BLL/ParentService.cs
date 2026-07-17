using School.DAL.Interfaces;
using School.DTO.ParentsDTOs;

namespace School.BLL
{
    public class ParentService
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
                throw new ArgumentOutOfRangeException(
                    nameof(parentId),
                    "Parent ID must be greater than zero.");
        }

        private static void ValidatePersonId(int personId)
        {
            if (personId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(personId),
                    "Person ID must be greater than zero.");
        }

        private async Task EnsureParentExistsAsync(int parentId)
        {
            if (!await _parentData.IsParentExistAsync(parentId))
                throw new InvalidOperationException(
                    $"Parent with ID {parentId} does not exist.");
        }

        private async Task EnsurePersonExistsAsync(int personId)
        {
            if (!await _personData.IsPersonExistAsync(personId))
                throw new InvalidOperationException(
                    $"Person with ID {personId} does not exist.");
        }

        private async Task EnsurePersonIsNotParentAsync(int personId, int? currentParentId = null)
        {
            ParentDetailsDTO? parent =
                await _parentData.GetParentByPersonIdAsync(personId);

            if (parent == null)
                return;

            if (currentParentId.HasValue &&
                parent.ParentID == currentParentId.Value)
                return;

            throw new InvalidOperationException(
                $"Person ID {personId} is already linked to another parent.");
        }

        private async Task EnsurePersonIsNotStudentAsync(int personId)
        {
            if (await _studentData.GetStudentByPersonIdAsync(personId) != null)
                throw new InvalidOperationException(
                    $"Person ID {personId} is already registered as a student.");
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

            return await _parentData.GetParentByIdAsync(parentId);
        }

        public async Task<ParentDetailsDTO?> GetParentByPersonIdAsync(int personId)
        {
            ValidatePersonId(personId);

            return await _parentData.GetParentByPersonIdAsync(personId);
        }

        public async Task<ParentDetailsDTO?> GetParentByNationalIdAsync(string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException("National ID is required.", nameof(nationalId));

            return await _parentData.GetParentByNationalIdAsync(nationalId);
        }

        public async Task<int> AddParentAsync(ParentDTO parent)
        {
            ValidateParent(parent);

            await EnsurePersonExistsAsync(parent.PersonID);

            await EnsurePersonIsNotParentAsync(parent.PersonID);

            await EnsurePersonIsNotStudentAsync(parent.PersonID);

            return await _parentData.AddParentAsync(parent);
        }

        public async Task<bool> UpdateParentAsync(ParentDTO parent)
        {
            ValidateParent(parent);

            ValidateParentId(parent.ParentID);

            await EnsureParentExistsAsync(parent.ParentID);

            await EnsurePersonExistsAsync(parent.PersonID);

            await EnsurePersonIsNotParentAsync(
                parent.PersonID,
                parent.ParentID);

            await EnsurePersonIsNotStudentAsync(parent.PersonID);

            return await _parentData.UpdateParentAsync(parent);
        }

        public async Task<bool> DeleteParentAsync(int parentId)
        {
            ValidateParentId(parentId);

            await EnsureParentExistsAsync(parentId);

            return await _parentData.DeleteParentAsync(parentId);
        }

        public async Task<bool> IsParentExistAsync(int parentId)
        {
            ValidateParentId(parentId);

            return await _parentData.IsParentExistAsync(parentId);
        }

        #endregion
    }
}