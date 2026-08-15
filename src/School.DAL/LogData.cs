using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.LogDTOs;
using System.Data;

namespace School.DAL
{
    public class LogData : BaseData, ILogData
    {
        public LogData(IConfiguration configuration) : base(configuration) { }

        #region Public Methods

        public Task AddLogAsync(LogEntryDTO log) =>
            ExecuteNonQueryAsync("SP_AddLog",
                cmd =>
                {
                    cmd.Parameters.Add("@Level", SqlDbType.NVarChar).Value = log.Level;
                    cmd.Parameters.Add("@Category", SqlDbType.NVarChar).Value = log.Category;
                    cmd.Parameters.Add("@Message", SqlDbType.NVarChar).Value = log.Message;
                    cmd.Parameters.Add("@Exception", SqlDbType.NVarChar).Value = (object?)log.Exception ?? DBNull.Value;
                });

        #endregion
    }
}