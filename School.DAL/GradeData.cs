using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.GradesDTOs;
using System.Data;

namespace School.DAL
{
    public class GradeData : BaseData, IGradeData
    {
        public GradeData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static GradeDTO MapGrade(SqlDataReader reader)
        {
            return new GradeDTO
            {
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName"))
            };
        }

        private static void AddParameters(SqlCommand command, GradeDTO grade)
        {
            command.Parameters.Add("@GradeName", SqlDbType.NVarChar).Value = grade.GradeName;
        }

        #endregion

        #region Public Methods

        public Task<List<GradeDTO>> GetAllGradesAsync() =>
            QueryListAsync("SP_GetAllGrades", null, MapGrade);

        public Task<GradeDTO?> GetGradeByIdAsync(byte gradeId) =>
            QuerySingleAsync("SP_GetGradeByID", cmd => cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId,
                MapGrade);

        public Task<GradeDTO?> GetGradeByNameAsync(string gradeName) =>
            QuerySingleAsync("SP_GetGradeByName", cmd => cmd.Parameters.Add("@GradeName", SqlDbType.NVarChar).Value = gradeName,
                MapGrade);

        public Task<int> AddGradeAsync(GradeDTO grade) =>
            InsertAsync<int>("SP_AddGrade", cmd => AddParameters(cmd, grade), "@GradeID", SqlDbType.TinyInt);

        public Task<bool> UpdateGradeAsync(GradeDTO grade) =>
            ExecuteNonQueryAsync("SP_UpdateGrade",
                cmd =>
                {
                    cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = grade.GradeID;
                    AddParameters(cmd, grade);
                });

        public Task<bool> DeleteGradeAsync(byte gradeId) =>
            ExecuteNonQueryAsync("SP_DeleteGrade", cmd => cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId);

        public Task<bool> IsGradeExistAsync(byte gradeId) =>
            ExecuteExistsAsync("SP_IsGradeExists", cmd => cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId);

        #endregion
    }
}