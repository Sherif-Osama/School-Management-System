namespace School.DTO.AssociationsDTOs.ClassSubjectDTOs.Requests
{
    public class CreateClassSubjectRequest
    {
        public int ClassID { get; set; }

        public int SubjectID { get; set; }

        public int TeacherID { get; set; }
    }
}