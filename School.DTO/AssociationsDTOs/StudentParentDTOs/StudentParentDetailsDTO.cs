namespace School.DTO.AssociationsDTOs.StudentParentDTOs
{
    public class StudentParentDetailsDTO
    {
        public int StudentID { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public int ParentID { get; set; }

        public string ParentName { get; set; } = string.Empty;
    }
}