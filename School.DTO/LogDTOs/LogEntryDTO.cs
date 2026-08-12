namespace School.DTO.LogDTOs
{
    public class LogEntryDTO
    {
        public required string Level { get; set; }

        public required string Category { get; set; }

        public required string Message { get; set; }

        public string? Exception { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}