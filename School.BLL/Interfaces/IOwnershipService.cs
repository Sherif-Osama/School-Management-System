namespace School.BLL.Interfaces
{
    public interface IOwnershipService
    {
        Task<bool> IsOwnStudentAsync(int studentId, int currentPersonId);
        Task<bool> IsOwnParentRecordAsync(int parentId, int currentPersonId);
    }
}