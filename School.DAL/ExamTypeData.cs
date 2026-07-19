using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.ExamTypeDTOs;
using System.Data;

namespace School.DAL
{
    public class ExamTypeData : BaseData, IExamTypeData
    {
        public ExamTypeData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static ExamTypeDTO MapExamType(SqlDataReader reader)
        {
            return new ExamTypeDTO
            {
                ExamTypeID = reader.GetInt32(reader.GetOrdinal("ExamTypeID")),
                ExamName = reader.GetString(reader.GetOrdinal("ExamName"))
            };
        }

        private static void AddParameters(SqlCommand command, ExamTypeDTO examType)
        {
            command.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value = examType.ExamName.Trim();
        }

        #endregion

        #region Public Methods

        public Task<List<ExamTypeDTO>> GetAllExamTypesAsync() =>
            QueryListAsync("SP_GetAllExamTypes", null, MapExamType);

        public Task<ExamTypeDTO?> GetExamTypeByIdAsync(int examTypeId) =>
            QuerySingleAsync("SP_GetExamTypeByID", cmd => cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId,
                MapExamType);

        public Task<ExamTypeDTO?> GetExamTypeByNameAsync(string examName) =>
            QuerySingleAsync("SP_GetExamTypeByName", cmd => cmd.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value = examName.Trim(),
                MapExamType);

        public Task<int> AddExamTypeAsync(ExamTypeDTO examType) =>
            InsertAsync<int>("SP_AddExamType", cmd => AddParameters(cmd, examType), "@ExamTypeID", SqlDbType.Int);

        public Task<bool> UpdateExamTypeAsync(ExamTypeDTO examType) =>
            ExecuteNonQueryAsync("SP_UpdateExamType",
                cmd =>
                {
                    cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examType.ExamTypeID;
                    AddParameters(cmd, examType);
                });

        public Task<bool> DeleteExamTypeAsync(int examTypeId) =>
            ExecuteNonQueryAsync("SP_DeleteExamType", cmd => cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId);

        public Task<bool> IsExamTypeExistAsync(int examTypeId) =>
            ExecuteExistsAsync("SP_IsExamTypeExists", cmd => cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId);

        #endregion
    }
}