using Microsoft.Data.SqlClient;
using School.DTO.PersonDTOs;
using System.Data;

namespace School.DAL
{

    public class PersonData
    {
        private readonly string _connectionString;

        public PersonData(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Helper Methods
        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private static async Task<List<PersonDTO>> ReadPeopleAsync(SqlCommand command)
        {
            List<PersonDTO> people = [];

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                people.Add(MapPerson(reader));
            }

            return people;
        }

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
        public async Task<List<PersonDTO>> GetAllPeopleAsync()
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_GetAllPeople", connection);
            command.CommandType = CommandType.StoredProcedure;

            return await ReadPeopleAsync(command);
        }

        public async Task<PersonDTO?> GetPersonByIdAsync(int personId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetPersonByID", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;

            return (await ReadPeopleAsync(command)).FirstOrDefault();
        }

        public async Task<PersonDTO?> GetPersonByNationalIDAsync(string nationalId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_GetPersonByNationalID", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@NationalID", SqlDbType.NVarChar, 50).Value = nationalId;

            return (await ReadPeopleAsync(command)).FirstOrDefault();
        }

        public async Task<int> AddPersonAsync(PersonDTO person)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_AddPerson", connection);
            command.CommandType = CommandType.StoredProcedure;

            AddParameters(command, person);

            var outputIdParam = new SqlParameter("@PersonID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputIdParam);
            await command.ExecuteNonQueryAsync();

            return (int)outputIdParam.Value;
        }

        public async Task<bool> UpdatePersonAsync(PersonDTO person)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();

            using SqlCommand command = new("SP_UpdatePerson", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = person.PersonID;
            AddParameters(command, person);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeletePersonAsync(int personId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_DeletePerson", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsPersonExistAsync(int personId)
        {
            using SqlConnection connection = await GetOpenConnectionAsync();
            using SqlCommand command = new("SP_IsPersonExist", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        #endregion

    }
}