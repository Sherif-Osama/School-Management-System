using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.GradesDTOs.Requests;
using School.DTO.GradesDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class GradeData : BaseData, IGradeData
    {
        public GradeData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static GradeResponse MapGrade(SqlDataReader reader)
        {
            return new GradeResponse
            {
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName"))
            };
        }

        private static void AddParameters(SqlCommand command, CreateGradeRequest grade)
        {
            command.Parameters.Add("@GradeName", SqlDbType.NVarChar).Value = grade.GradeName;
        }

        #endregion

        #region Public Methods

        public Task<List<GradeResponse>> GetAllGradesAsync() =>
            QueryListAsync("SP_GetAllGrades", null, MapGrade);

        public Task<GradeResponse?> GetGradeByIdAsync(byte gradeId) =>
            QuerySingleAsync("SP_GetGradeByID", cmd => cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId,
                MapGrade);

        public Task<GradeResponse?> GetGradeByNameAsync(string gradeName) =>
            QuerySingleAsync("SP_GetGradeByName", cmd => cmd.Parameters.Add("@GradeName", SqlDbType.NVarChar).Value = gradeName,
                MapGrade);

        public Task<int> AddGradeAsync(CreateGradeRequest grade) =>
            InsertAsync<int>("SP_AddGrade", cmd => AddParameters(cmd, grade), "@GradeID", SqlDbType.TinyInt);

        public Task<bool> UpdateGradeAsync(byte gradeId, UpdateGradeRequest grade) =>
            ExecuteNonQueryAsync("SP_UpdateGrade",
                cmd =>
                {
                    cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId;
                    cmd.Parameters.Add("@GradeName", SqlDbType.NVarChar).Value = grade.GradeName;
                });

        public Task<bool> DeleteGradeAsync(byte gradeId) =>
            ExecuteNonQueryAsync("SP_DeleteGrade", cmd => cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId);

        public Task<bool> IsGradeExistAsync(byte gradeId) =>
            ExecuteExistsAsync("SP_IsGradeExists", cmd => cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId);

        #endregion
    }
}