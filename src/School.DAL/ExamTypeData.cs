using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.ExamTypeDTOs.Requests;
using School.DTO.ExamTypeDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class ExamTypeData : BaseData, IExamTypeData
    {
        public ExamTypeData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static ExamTypeResponse MapExamType(SqlDataReader reader)
        {
            return new ExamTypeResponse
            {
                ExamTypeID = reader.GetInt32(reader.GetOrdinal("ExamTypeID")),
                ExamName = reader.GetString(reader.GetOrdinal("ExamName"))
            };
        }

        private static void AddParameters(SqlCommand command, CreateExamTypeRequest examType)
        {
            command.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value = examType.ExamName.Trim();
        }

        #endregion

        #region Public Methods

        public Task<List<ExamTypeResponse>> GetAllExamTypesAsync() =>
            QueryListAsync("SP_GetAllExamTypes", null, MapExamType);

        public Task<ExamTypeResponse?> GetExamTypeByIdAsync(int examTypeId) =>
            QuerySingleAsync("SP_GetExamTypeByID", cmd => cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId,
                MapExamType);

        public Task<ExamTypeResponse?> GetExamTypeByNameAsync(string examName) =>
            QuerySingleAsync("SP_GetExamTypeByName", cmd => cmd.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value = examName.Trim(),
                MapExamType);

        public Task<int> AddExamTypeAsync(CreateExamTypeRequest examType) =>
            InsertAsync<int>("SP_AddExamType", cmd => AddParameters(cmd, examType), "@ExamTypeID", SqlDbType.Int);

        public Task<bool> UpdateExamTypeAsync(int examTypeId, UpdateExamTypeRequest examType) =>
            ExecuteNonQueryAsync("SP_UpdateExamType",
                cmd =>
                {
                    cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId;
                    cmd.Parameters.Add("@ExamName", SqlDbType.NVarChar).Value = examType.ExamName.Trim();
                });

        public Task<bool> DeleteExamTypeAsync(int examTypeId) =>
            ExecuteNonQueryAsync("SP_DeleteExamType", cmd => cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId);

        public Task<bool> IsExamTypeExistAsync(int examTypeId) =>
            ExecuteExistsAsync("SP_IsExamTypeExists", cmd => cmd.Parameters.Add("@ExamTypeID", SqlDbType.Int).Value = examTypeId);
        #endregion
    }
}