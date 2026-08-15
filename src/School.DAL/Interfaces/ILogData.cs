using School.DTO.LogDTOs;

namespace School.DAL.Interfaces
{
    public interface ILogData
    {
        Task AddLogAsync(LogEntryDTO log);
    }
}