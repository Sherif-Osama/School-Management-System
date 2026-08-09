using School.BLL.Common;
using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ClassroomDTOs;

namespace School.BLL
{
    public class ClassroomService : IClassroomService
    {
        private readonly IClassroomData _classroomData;
        private static int minRoomNameLength => 2;
        private static int maxRoomNameLength => 20;
        public ClassroomService(IClassroomData classroomData)
        {
            _classroomData = classroomData;
        }

        #region Validation
        private static void ValidateClassroom(ClassroomDTO classroom)
        {
            ArgumentNullException.ThrowIfNull(classroom);

            classroom.RoomName = ValidationHelper.ValidateString(classroom.RoomName, nameof(classroom.RoomName), minRoomNameLength, maxRoomNameLength);

            ValidateCapacity(classroom.Capacity);
        }

        private static void ValidateCapacity(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity),
                    "Capacity must be greater than zero.");

            if (capacity > 100)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot exceed 100 students.");
        }

        #endregion

        #region Ensure
        private async Task EnsureClassroomExistsAsync(int classroomId)
        {
            if (!await _classroomData.IsClassroomExistAsync(classroomId))
                throw new KeyNotFoundException(
                    $"Classroom with ID {classroomId} does not exist.");
        }

        private async Task EnsureRoomNumberUniqueAsync(string roomName, int? currentClassroomId = null)
        {
            ClassroomDTO? classroom = await _classroomData.GetClassroomByRoomNameAsync(roomName);

            if (classroom == null)
                return;

            if (currentClassroomId.HasValue && classroom.ClassroomID == currentClassroomId.Value)
                return;

            throw new InvalidOperationException($"Room with name {roomName} already exists.");
        }

        #endregion

        #region Public Methods

        public Task<List<ClassroomDTO>> GetAllClassroomsAsync()
        {
            return _classroomData.GetAllClassroomsAsync();
        }

        public async Task<ClassroomDTO?> GetClassroomByIdAsync(int classroomId)
        {
            ValidationHelper.ValidateId(classroomId);

            ClassroomDTO? classroom = await _classroomData.GetClassroomByIdAsync(classroomId);

            if (classroom == null)
                throw new KeyNotFoundException($"Classroom with ID {classroomId} does not exist.");

            return classroom;
        }

        public async Task<ClassroomDTO?> GetClassroomByRoomNameAsync(string roomName)
        {
            roomName = ValidationHelper.ValidateString(roomName, nameof(roomName), minRoomNameLength, maxRoomNameLength);

            ClassroomDTO? classroom = await _classroomData.GetClassroomByRoomNameAsync(roomName);

            if (classroom == null)
                throw new KeyNotFoundException($"Classroom with room name {roomName} does not exist.");

            return classroom;
        }

        public async Task<int> AddClassroomAsync(ClassroomDTO classroom)
        {
            ValidateClassroom(classroom);

            await EnsureRoomNumberUniqueAsync(classroom.RoomName);

            int newClassroomId = await _classroomData.AddClassroomAsync(classroom);

            if (newClassroomId <= 0)
                throw new InvalidOperationException("Failed to add classroom.");

            return newClassroomId;
        }

        public async Task<bool> UpdateClassroomAsync(ClassroomDTO classroom)
        {
            ValidateClassroom(classroom);

            ValidationHelper.ValidateId(classroom.ClassroomID);

            await EnsureClassroomExistsAsync(classroom.ClassroomID);

            await EnsureRoomNumberUniqueAsync(classroom.RoomName, classroom.ClassroomID);

            bool isUpdated = await _classroomData.UpdateClassroomAsync(classroom);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update classroom with ID {classroom.ClassroomID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteClassroomAsync(int classroomId)
        {
            ValidationHelper.ValidateId(classroomId);

            await EnsureClassroomExistsAsync(classroomId);

            bool isDeleted = await _classroomData.DeleteClassroomAsync(classroomId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete classroom with ID {classroomId}.");

            return isDeleted;
        }

        public async Task<bool> IsClassroomExistAsync(int classroomId)
        {
            ValidationHelper.ValidateId(classroomId);

            return await _classroomData.IsClassroomExistAsync(classroomId);
        }
        #endregion
    }
}