using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using School.DAL.Common;
using School.DAL.Interfaces;
using School.DTO.CityDTOs;
using System.Data;

namespace School.DAL
{
    public class CityData : BaseData, ICityData
    {
        public CityData(IConfiguration configuration) : base(configuration) { }

        #region Helper Methods

        private static CityDTO MapCity(SqlDataReader reader)
        {
            return new CityDTO
            {
                CityID = reader.GetInt32(reader.GetOrdinal("CityID")),
                CityName = reader.GetString(reader.GetOrdinal("CityName")),
                CountryID = reader.GetInt32(reader.GetOrdinal("CountryID")),
                CountryName = reader.GetString(reader.GetOrdinal("CountryName"))
            };
        }

        #endregion Helper Methods

        #region Public Methods
        public Task<List<CityDTO>> GetAllCitiesAsync() => QueryListAsync("SP_GetAllCities", null, MapCity);

        public Task<CityDTO?> GetCityByIdAsync(int cityId) => QuerySingleAsync("SP_GetCityByID", cmd => cmd.Parameters.Add("@CityID", SqlDbType.Int).Value = cityId,
                MapCity);

        public Task<CityDTO?> GetCityByNameAsync(string cityName) => QuerySingleAsync("SP_GetCityByName", cmd => cmd.Parameters.Add("@CityName", SqlDbType.NVarChar, 100).Value = cityName.Trim(),
                MapCity);
        #endregion
    }
}