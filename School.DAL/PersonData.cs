using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.PersonDTOs.Requests;
using School.DTO.PersonDTOs.Responses;
using System.Data;

namespace School.DAL
{
    public class PersonData : BaseData, IPersonData
    {
        public PersonData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static PersonResponse MapPerson(SqlDataReader reader)
        {
            return new PersonResponse
            {
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

        private static void AddParameters(SqlCommand command, CreatePersonRequest person)
        {
            command.Parameters.Add("@NationalID", SqlDbType.NVarChar, 50).Value = person.NationalID;
            command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = person.FirstName;
            command.Parameters.Add("@SecondName", SqlDbType.NVarChar, 50).Value = person.SecondName;
            command.Parameters.Add("@ThirdName", SqlDbType.NVarChar, 50).Value = person.ThirdName;
            command.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = person.LastName ?? (object)DBNull.Value;
            command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = person.DateOfBirth;
            command.Parameters.Add("@Gender", SqlDbType.TinyInt).Value = person.Gender;
            command.Parameters.Add("@Address", SqlDbType.NVarChar, 250).Value = person.Address ?? (object)DBNull.Value;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = person.Phone;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = person.Email ?? (object)DBNull.Value;
            command.Parameters.Add("@ImagePath", SqlDbType.NVarChar, 250).Value = person.ImagePath ?? (object)DBNull.Value;
            command.Parameters.Add("@CityID", SqlDbType.Int).Value = person.CityID;
        }

        #endregion

        #region Public Methods

        public Task<List<PersonResponse>> GetAllPeopleAsync() => QueryListAsync("SP_GetAllPeople", null, MapPerson);

        public Task<PersonResponse?> GetPersonByIdAsync(int personId) => QuerySingleAsync("SP_GetPersonByID", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId,
                MapPerson);

        public Task<PersonResponse?> GetPersonByNationalIDAsync(string nationalId) =>
            QuerySingleAsync("SP_GetPersonByNationalID", cmd => cmd.Parameters.Add("@NationalID", SqlDbType.NVarChar, 50).Value = nationalId,
                MapPerson);

        public Task<int> AddPersonAsync(CreatePersonRequest person) =>
            InsertAsync<int>("SP_AddPerson", cmd => AddParameters(cmd, person), "@PersonID",
                SqlDbType.Int);

        public Task<bool> UpdatePersonAsync(int personId, UpdatePersonRequest person) =>
            ExecuteNonQueryAsync("SP_UpdatePerson",
                cmd =>
                {
                    cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
                    cmd.Parameters.Add("@NationalID", SqlDbType.NVarChar, 50).Value = person.NationalID;
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = person.FirstName;
                    cmd.Parameters.Add("@SecondName", SqlDbType.NVarChar, 50).Value = person.SecondName;
                    cmd.Parameters.Add("@ThirdName", SqlDbType.NVarChar, 50).Value = person.ThirdName;
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = person.LastName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = person.DateOfBirth;
                    cmd.Parameters.Add("@Gender", SqlDbType.TinyInt).Value = person.Gender;
                    cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 250).Value = person.Address ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = person.Phone;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = person.Email ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@ImagePath", SqlDbType.NVarChar, 250).Value = person.ImagePath ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@CityID", SqlDbType.Int).Value = person.CityID;
                });

        public Task<bool> DeletePersonAsync(int personId) =>
            ExecuteNonQueryAsync("SP_DeletePerson", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId);

        public Task<bool> IsPersonExistAsync(int personId) =>
            ExecuteExistsAsync("SP_IsPersonExist", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId);

        #endregion
    }
}