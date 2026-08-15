namespace School.DTO.PersonDTOs.Requests
{
    public class CreatePersonRequest
    {
        public required string NationalID { get; set; }

        public required string FirstName { get; set; }

        public required string SecondName { get; set; }

        public required string ThirdName { get; set; }

        public string? LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public byte Gender { get; set; }

        public string? Address { get; set; }

        public required string Phone { get; set; }

        public string? Email { get; set; }

        public string? ImagePath { get; set; }

        public int CityID { get; set; }
    }
}
