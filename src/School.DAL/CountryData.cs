using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.CountriesDTOs;
using System.Data;

namespace School.DAL
{
    public class CountryData : BaseData, ICountryData
    {
        public CountryData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static CountryDTO MapCountry(SqlDataReader reader)
        {
            return new CountryDTO
            {
                CountryID = reader.GetInt32(reader.GetOrdinal("CountryID")),
                CountryName = reader.GetString(reader.GetOrdinal("CountryName"))
            };
        }

        #endregion Helper Methods

        #region Public Methods

        public Task<List<CountryDTO>> GetAllCountriesAsync() => QueryListAsync("SP_GetAllCountries", null, MapCountry);

        public Task<CountryDTO?> GetCountryByIdAsync(int countryId) => QuerySingleAsync("SP_GetCountryByID", cmd => cmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryId,
                MapCountry);

        public Task<CountryDTO?> GetCountryByNameAsync(string countryName) => QuerySingleAsync("SP_GetCountryByName", cmd => cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar, 100).Value = countryName.Trim(),
                MapCountry);

        #endregion
    }
}