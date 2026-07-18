using School.BLL.Interfaces;
using School.DAL.Interfaces;
using School.DTO.ClassroomDTOs;

namespace School.BLL
{
    public class ClassroomService : IClassroomService
    {
        private readonly IClassroomData _classroomData;

        public ClassroomService(IClassroomData classroomData)
        {
            _classroomData = classroomData;
        }

        #region Validation
        private static void ValidateClassroom(ClassroomDTO classroom)
        {
            ArgumentNullException.ThrowIfNull(classroom);

            classroom.RoomName = ValidateRoomNumber(classroom.RoomName);

            ValidateCapacity(classroom.Capacity);
        }

        private static void ValidateClassroomId(int classroomId)
        {
            if (classroomId <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(classroomId),
                    "Classroom ID must be greater than zero.");
        }

        private static string ValidateRoomNumber(string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName))
                throw new ArgumentException("Room name is required.", nameof(roomName));

            roomName = roomName.Trim();

            if (roomName.Length > 20)
                throw new ArgumentException("Room name cannot exceed 20 characters.", nameof(roomName));

            return roomName;
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
            ValidateClassroomId(classroomId);

            ClassroomDTO? classroom = await _classroomData.GetClassroomByIdAsync(classroomId);

            if (classroom == null)
                throw new KeyNotFoundException($"Classroom with ID {classroomId} does not exist.");

            return classroom;
        }

        public async Task<ClassroomDTO?> GetClassroomByRoomNameAsync(string roomName)
        {
            roomName = ValidateRoomNumber(roomName);

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

            ValidateClassroomId(classroom.ClassroomID);

            await EnsureClassroomExistsAsync(classroom.ClassroomID);

            await EnsureRoomNumberUniqueAsync(classroom.RoomName, classroom.ClassroomID);

            bool isUpdated = await _classroomData.UpdateClassroomAsync(classroom);

            if (!isUpdated)
                throw new InvalidOperationException($"Failed to update classroom with ID {classroom.ClassroomID}.");

            return isUpdated;
        }

        public async Task<bool> DeleteClassroomAsync(int classroomId)
        {
            ValidateClassroomId(classroomId);

            await EnsureClassroomExistsAsync(classroomId);

            bool isDeleted = await _classroomData.DeleteClassroomAsync(classroomId);

            if (!isDeleted)
                throw new InvalidOperationException($"Failed to delete classroom with ID {classroomId}.");

            return isDeleted;
        }

        public async Task<bool> IsClassroomExistAsync(int classroomId)
        {
            ValidateClassroomId(classroomId);

            return await _classroomData.IsClassroomExistAsync(classroomId);
        }
        #endregion
    }
}