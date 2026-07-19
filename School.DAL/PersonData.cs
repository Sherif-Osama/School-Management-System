using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.PersonDTOs;
using System.Data;

namespace School.DAL
{
    public class PersonData : BaseData, IPersonData
    {
        public PersonData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static PersonDTO MapPerson(SqlDataReader reader)
        {
            return new PersonDTO
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

        private static void AddParameters(SqlCommand command, PersonDTO person)
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

        public Task<List<PersonDTO>> GetAllPeopleAsync() =>
            QueryListAsync("SP_GetAllPeople", null, MapPerson);

        public Task<PersonDTO?> GetPersonByIdAsync(int personId) =>
            QuerySingleAsync("SP_GetPersonByID", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId,
                MapPerson);

        public Task<PersonDTO?> GetPersonByNationalIDAsync(string nationalId) =>
            QuerySingleAsync("SP_GetPersonByNationalID", cmd => cmd.Parameters.Add("@NationalID", SqlDbType.NVarChar, 50).Value = nationalId,
                MapPerson);

        public Task<int> AddPersonAsync(PersonDTO person) =>
            InsertAsync<int>("SP_AddPerson", cmd => AddParameters(cmd, person), "@PersonID",
                SqlDbType.Int);

        public Task<bool> UpdatePersonAsync(PersonDTO person) =>
            ExecuteNonQueryAsync("SP_UpdatePerson",
                cmd =>
                {
                    cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = person.PersonID;
                    AddParameters(cmd, person);
                });

        public Task<bool> DeletePersonAsync(int personId) =>
            ExecuteNonQueryAsync("SP_DeletePerson", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId);

        public Task<bool> IsPersonExistAsync(int personId) =>
            ExecuteExistsAsync("SP_IsPersonExist", cmd => cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId);

        #endregion
    }
}