namespace School.DTO.CountriesDTOs
{
    // No Create or Update requests are required for this entity.
    // Read-only lookup DTO.
    // Used to retrieve country reference data for selection and display.
    public class CountryDTO
    {
        public int CountryID { get; set; }

        public string CountryName { get; set; } = string.Empty;
    }
}