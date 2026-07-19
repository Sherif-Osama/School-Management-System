using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.ClassesDTOs;
using System.Data;

namespace School.DAL
{
    public class ClassData : BaseData, IClassData
    {
        public ClassData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static ClassDetailsDTO MapClassDetails(SqlDataReader reader)
        {
            return new ClassDetailsDTO
            {
                ClassID = reader.GetInt32(reader.GetOrdinal("ClassID")),
                GradeID = reader.GetByte(reader.GetOrdinal("GradeID")),
                GradeName = reader.GetString(reader.GetOrdinal("GradeName")),
                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
                AcademicYear = reader.GetString(reader.GetOrdinal("AcademicYear")),
                Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        private static void AddParameters(SqlCommand command, ClassDTO schoolClass)
        {
            command.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = schoolClass.GradeID;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar).Value = schoolClass.ClassName;
            command.Parameters.Add("@AcademicYear", SqlDbType.NVarChar).Value = schoolClass.AcademicYear;
            command.Parameters.Add("@Capacity", SqlDbType.Int).Value = schoolClass.Capacity;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = schoolClass.IsActive;
        }

        #endregion

        #region Public Methods

        public Task<List<ClassDetailsDTO>> GetAllClassesAsync() =>
            QueryListAsync("SP_GetAllClasses", null, MapClassDetails);

        public Task<ClassDetailsDTO?> GetClassByIdAsync(int classId) =>
            QuerySingleAsync("SP_GetClassByID", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId,
                MapClassDetails);

        public Task<ClassDetailsDTO?> GetClassByDetailsAsync(byte gradeId, string className, string academicYear) =>
            QuerySingleAsync("SP_GetClassByName",
                cmd =>
                {
                    cmd.Parameters.Add("@GradeID", SqlDbType.TinyInt).Value = gradeId;
                    cmd.Parameters.Add("@ClassName", SqlDbType.NVarChar).Value = className;
                    cmd.Parameters.Add("@AcademicYear", SqlDbType.NVarChar).Value = academicYear;
                },
                MapClassDetails);

        public Task<int> AddClassAsync(ClassDTO schoolClass) =>
            InsertAsync<int>("SP_AddClass", cmd => AddParameters(cmd, schoolClass), "@ClassID", SqlDbType.Int);

        public Task<bool> UpdateClassAsync(ClassDTO schoolClass) =>
            ExecuteNonQueryAsync("SP_UpdateClass",
                cmd =>
                {
                    cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = schoolClass.ClassID;
                    AddParameters(cmd, schoolClass);
                });

        public Task<bool> DeleteClassAsync(int classId) =>
            ExecuteNonQueryAsync("SP_DeleteClass", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId);

        public Task<bool> IsClassExistAsync(int classId) =>
            ExecuteExistsAsync("SP_IsClassExists", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classId);

        public Task<bool> HasClassAvailableCapacityAsync(int classID) =>
            ExecuteExistsAsync("SP_HasClassAvailableCapacity", cmd => cmd.Parameters.Add("@ClassID", SqlDbType.Int).Value = classID);

        #endregion
    }
}