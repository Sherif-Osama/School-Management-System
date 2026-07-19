using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.ParentsDTOs;
using System.Data;

namespace School.DAL
{
    public class ParentData : BaseData, IParentData
    {
        public ParentData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static ParentDetailsDTO MapParentDetails(SqlDataReader reader)
        {
            return new ParentDetailsDTO
            {
                ParentID = reader.GetInt32(reader.GetOrdinal("ParentID")),
                PersonID = reader.GetInt32(reader.GetOrdinal("PersonID")),
                NationalID = reader.GetString(reader.GetOrdinal("NationalID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                SecondName = reader.GetString(reader.GetOrdinal("SecondName")),
                ThirdName = reader.GetString(reader.GetOrdinal("ThirdName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                Gender = reader.GetByte(reader.GetOrdinal("Gender")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? null : reader.GetString(reader.GetOrdinal("ImagePath")),
                CityID = reader.GetInt32(reader.GetOrdinal("CityID"))
            };
        }

        private static void AddParameters(SqlCommand command, ParentDTO parent)
        {
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = parent.PersonID;
        }

        #endregion

        #region Public Methods

        public Task<List<ParentDetailsDTO>> GetAllParentsAsync() =>
            QueryListAsync("SP_GetAllParents", null, MapParentDetails);

        public Task<ParentDetailsDTO?> GetParentByIdAsync(int parentId) =>
            QuerySingleAsync("SP_GetParentByID", cmd => cmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId,
                MapParentDetails);

        public Task<ParentDetailsDTO?> GetParentByPersonIdAsync(int personId) =>
            QuerySingleAsync("SP_GetParentByPersonID", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId,
                MapParentDetails);

        public Task<ParentDetailsDTO?> GetParentByNationalIdAsync(string nationalId) =>
            QuerySingleAsync("SP_GetParentByNationalID", cmd => cmd.Parameters.Add("@NationalID", SqlDbType.NVarChar, 14).Value = nationalId,
                MapParentDetails);

        public Task<int> AddParentAsync(ParentDTO parent) =>
            InsertAsync<int>("SP_AddParent", cmd => AddParameters(cmd, parent), "@ParentID",
                SqlDbType.Int);

        public Task<bool> UpdateParentAsync(ParentDTO parent) =>
            ExecuteNonQueryAsync("SP_UpdateParent",
                cmd =>
                {
                    cmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = parent.ParentID;
                    AddParameters(cmd, parent);
                });

        public Task<bool> DeleteParentAsync(int parentId) =>
            ExecuteNonQueryAsync("SP_DeleteParent", cmd => cmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId);

        public Task<bool> IsParentExistAsync(int parentId) =>
            ExecuteExistsAsync("SP_IsParentExists", cmd => cmd.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId);

        #endregion
    }
}