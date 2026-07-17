using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DTO.ParentsDTOs;
using System.Data;
namespace School.DAL
{
    public class ParentData : BaseData
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

        private static async Task<List<ParentDetailsDTO>> ReadParentDetailsAsync(SqlCommand command)
        {
            List<ParentDetailsDTO> parents = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                parents.Add(MapParentDetails(reader));

            return parents;
        }

        #endregion

        #region Public Methods

        public async Task<List<ParentDetailsDTO>> GetAllParentsAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetAllParents");
            return await ReadParentDetailsAsync(command);
        }

        public async Task<ParentDetailsDTO?> GetParentByIdAsync(int parentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetParentByID");
            command.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId;

            return (await ReadParentDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<ParentDetailsDTO?> GetParentByPersonIdAsync(int personId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetParentByPersonID");
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
            return (await ReadParentDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<ParentDetailsDTO?> GetParentByNationalIdAsync(string nationalId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_GetParentByNationalID");
            command.Parameters.Add("@NationalID", SqlDbType.NVarChar, 14).Value = nationalId;

            return (await ReadParentDetailsAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddParentAsync(ParentDTO parent)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_AddParent");
            AddParameters(command, parent);

            SqlParameter outputParentId = new("@ParentID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputParentId);

            await command.ExecuteNonQueryAsync();

            return (int)outputParentId.Value;
        }

        public async Task<bool> UpdateParentAsync(ParentDTO parent)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = CreateStoredProcedure(connection, "SP_UpdateParent");
            command.Parameters.Add("@ParentID", SqlDbType.Int).Value = parent.ParentID;
            AddParameters(command, parent);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteParentAsync(int parentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_DeleteParent");
            command.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId;

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsParentExistAsync(int parentId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = CreateStoredProcedure(connection, "SP_IsParentExists");

            command.Parameters.Add("@ParentID", SqlDbType.Int).Value = parentId;

            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion
    }
}