namespace School.DTO.CityDTOs
{
    // Read-only lookup DTO.
    // No Create or Update requests are required for this entity.
    // Used to retrieve country reference data for selection and display.
    public class CityDTO
    {
        public int CityID { get; set; }

        public string CityName { get; set; } = string.Empty;

        public int CountryID { get; set; }

        public string CountryName { get; set; } = string.Empty;
    }
}
