using School.DAL;
using School.DTO.ClassroomDTOs;

namespace School.BLL
{
    public class ClassroomService
    {
        private readonly ClassroomData _classroomData;

        public ClassroomService(ClassroomData classroomData)
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
                throw new InvalidOperationException(
                    $"Classroom with ID {classroomId} does not exist.");
        }

        private async Task EnsureRoomNumberUniqueAsync(string roomName, int? currentClassroomId = null)
        {
            ClassroomDTO? classroom =
                await _classroomData.GetClassroomByRoomNameAsync(roomName);

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

            return await _classroomData.GetClassroomByIdAsync(classroomId);
        }

        public async Task<ClassroomDTO?> GetClassroomByRoomNameAsync(string roomName)
        {
            roomName = ValidateRoomNumber(roomName);

            return await _classroomData.GetClassroomByRoomNameAsync(roomName);
        }

        public async Task<int> AddClassroomAsync(ClassroomDTO classroom)
        {
            ValidateClassroom(classroom);

            await EnsureRoomNumberUniqueAsync(classroom.RoomName);

            return await _classroomData.AddClassroomAsync(classroom);
        }

        public async Task<bool> UpdateClassroomAsync(ClassroomDTO classroom)
        {
            ValidateClassroom(classroom);

            ValidateClassroomId(classroom.ClassroomID);

            await EnsureClassroomExistsAsync(classroom.ClassroomID);

            await EnsureRoomNumberUniqueAsync(classroom.RoomName, classroom.ClassroomID);

            return await _classroomData.UpdateClassroomAsync(classroom);
        }

        public async Task<bool> DeleteClassroomAsync(int classroomId)
        {
            ValidateClassroomId(classroomId);

            await EnsureClassroomExistsAsync(classroomId);

            return await _classroomData.DeleteClassroomAsync(classroomId);
        }

        public async Task<bool> IsClassroomExistAsync(int classroomId)
        {
            ValidateClassroomId(classroomId);

            return await _classroomData.IsClassroomExistAsync(classroomId);
        }
        #endregion
    }
}