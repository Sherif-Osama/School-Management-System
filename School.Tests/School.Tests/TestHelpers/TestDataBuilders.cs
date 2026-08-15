using School.BLL.Common;
using School.BLL.Enums;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;
using School.DTO.AttendanceDTOs.Requests;
using School.DTO.AttendanceDTOs.Responses;
using School.DTO.AuthDTOs;
using School.DTO.ClassesDTOs.Responses;
using School.DTO.ExamDTOs;
using School.DTO.ScheduleDTOs.Requests;
using School.DTO.ScheduleDTOs.Responses;
using School.DTO.StudentGradeDTOs.Requests;
using School.DTO.StudentsDTOs.Responses;

namespace School.Tests.TestHelpers
{
    public static class TestDataBuilders
    {
        #region Student

        public static StudentResponse ValidStudent(
            int studentId = 1,
            int classId = 10,
            int statusId = (int)StudentStatus.Active,
            string statusName = "Active",
            DateTime? enrollmentDate = null,
            DateTime? dateOfBirth = null) => new()
            {
                StudentID = studentId,
                PersonID = 100,
                ClassID = classId,
                GradeID = 1,
                GradeName = "Grade 1",
                ClassName = "A",
                AcademicYear = "2025-2026",
                EnrollmentDate = enrollmentDate ?? new DateTime(2025, 9, 1),
                StatusID = statusId,
                StatusName = statusName,
                NationalID = "12345678901234",
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                ThirdName = "Ali",
                LastName = "Hassan",
                DateOfBirth = dateOfBirth ?? new DateTime(2015, 1, 1),
                Gender = 1,
                Phone = "01000000000",
                CityID = 1
            };

        #endregion

        #region Exam
        public static ExamResponse ValidExam(
            int examId = 1,
            int classId = 10,
            decimal totalMarks = 100) => new()
            {
                ExamID = examId,
                GradeID = 1,
                GradeName = "Grade 1",
                ClassID = classId,
                ClassName = "A",
                SubjectID = 1,
                SubjectName = "Math",
                TeacherID = 1,
                FirstName = "Sara",
                SecondName = "Ibrahim",
                ThirdName = "Mostafa",
                ExamTypeID = 1,
                ExamTypeName = "Midterm",
                ExamDate = DateOnly.FromDateTime(DateTime.Today),
                TotalMarks = totalMarks
            };
        #endregion

        #region StudentGrade
        public static CreateStudentGradeRequest ValidCreateStudentGradeRequest(
            int studentId = 1,
            int examId = 1,
            decimal grade = 50,
            bool isAbsent = false) => new()
            {
                StudentID = studentId,
                ExamID = examId,
                Grade = grade,
                IsAbsent = isAbsent
            };

        public static UpdateStudentGradeRequest ValidUpdateStudentGradeRequest(
            decimal grade = 50,
            bool isAbsent = false) => new()
            {
                Grade = grade,
                IsAbsent = isAbsent
            };
        #endregion

        #region ClassSubject
        public static ClassSubjectResponse ValidClassSubject(
            int classSubjectId = 1,
            int classId = 10,
            int teacherId = 1) => new()
            {
                ClassSubjectID = classSubjectId,
                GradeID = 1,
                GradeName = "Grade 1",
                ClassID = classId,
                ClassName = "A",
                AcademicYear = "2025-2026",
                SubjectID = 1,
                SubjectName = "Math",
                TeacherID = teacherId,
                FirstName = "Sara",
                SecondName = "Ibrahim",
                ThirdName = "Mostafa"
            };
        #endregion

        #region Schedule
        public static CreateScheduleRequest ValidCreateScheduleRequest(
            int classSubjectId = 1,
            int classroomId = 1,
            byte dayOfWeek = 1,
            TimeOnly? startTime = null,
            TimeOnly? endTime = null) => new()
            {
                ClassSubjectID = classSubjectId,
                ClassroomID = classroomId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime ?? new TimeOnly(9, 0),
                EndTime = endTime ?? new TimeOnly(10, 0)
            };

        public static UpdateScheduleRequest ValidUpdateScheduleRequest(
            int classSubjectId = 1,
            int classroomId = 1,
            byte dayOfWeek = 1,
            TimeOnly? startTime = null,
            TimeOnly? endTime = null) => new()
            {
                ClassSubjectID = classSubjectId,
                ClassroomID = classroomId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime ?? new TimeOnly(9, 0),
                EndTime = endTime ?? new TimeOnly(10, 0)
            };

        public static ScheduleResponse ValidSchedule(int scheduleId = 1) => new()
        {
            ScheduleID = scheduleId,
            GradeID = 1,
            GradeName = "Grade 1",
            ClassID = 10,
            ClassName = "A",
            AcademicYear = "2025-2026",
            SubjectID = 1,
            SubjectName = "Math",
            TeacherID = 1,
            FirstName = "Sara",
            SecondName = "Ibrahim",
            ThirdName = "Mostafa",
            ClassroomID = 1,
            RoomName = "Room 101",
            DayOfWeek = 1,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(10, 0)
        };
        #endregion

        #region Attendance
        public static AttendanceResponse ValidAttendance(int attendanceId = 1) => new()
        {
            AttendanceID = attendanceId,
            StudentID = 1,
            PersonID = 100,
            FirstName = "Ahmed",
            SecondName = "Mohamed",
            ThirdName = "Ali",
            LastName = "Hassan",
            GradeID = 1,
            GradeName = "Grade 1",
            ClassID = 10,
            ClassName = "A",
            AcademicYear = "2025-2026",
            AttendanceDate = new DateOnly(2026, 3, 15),
            StatusID = 1,
            StatusName = "Present"
        };

        public static CreateAttendanceRequest ValidCreateAttendanceRequest(int studentId = 1,
            DateOnly? attendanceDate = null, int statusId = 1) => new()
            {
                StudentID = studentId,
                AttendanceDate = attendanceDate ?? new DateOnly(2026, 3, 15),
                StatusID = statusId
            };
        public static UpdateAttendanceRequest ValidUpdateAttendanceRequest(DateOnly? attendanceDate = null,
            int statusId = 1) => new()
            {
                AttendanceDate = attendanceDate ?? new DateOnly(2026, 3, 15),
                StatusID = statusId
            };
        #endregion

        #region Class
        public static ClassResponse ValidClass(int classId = 10, bool isActive = true) => new()
        {
            ClassID = classId,
            GradeID = 1,
            GradeName = "Grade 1",
            ClassName = "A",
            AcademicYear = "2025-2026",
            Capacity = 30,
            IsActive = isActive
        };
        #endregion

        #region UserAuth
        public static UserAuth MakeUserAuth(string username, string plainPassword, bool isActive = true) => new()
        {
            UserID = 1,
            PersonID = 100,
            Username = username,
            PasswordHash = PasswordHasher.Hash(plainPassword),
            IsActive = isActive,
            Roles = ["Admin"],
            Permissions = ["Students.View.All"]
        };
        #endregion
    }
}