using School.BLL.Common;
using School.BLL.Enums;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;
using School.DTO.AssociationsDTOs.RolePermissionDTOs;
using School.DTO.AssociationsDTOs.StudentParentDTOs;
using School.DTO.AssociationsDTOs.StudentParentDTOs.Requests;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Responses;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Requests;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Responses;
using School.DTO.AttendanceDTOs.Requests;
using School.DTO.AttendanceDTOs.Responses;
using School.DTO.AttendanceStatusDTOs.Requests;
using School.DTO.AttendanceStatusDTOs.Responses;
using School.DTO.AuthDTOs;
using School.DTO.ClassesDTOs.Requests;
using School.DTO.ClassesDTOs.Responses;
using School.DTO.ExamDTOs;
using School.DTO.ExamDTOs.Requests;
using School.DTO.ExamTypeDTOs.Requests;
using School.DTO.ExamTypeDTOs.Responses;
using School.DTO.GradesDTOs.Requests;
using School.DTO.GradesDTOs.Responses;
using School.DTO.ParentsDTOs.Requests;
using School.DTO.ParentsDTOs.Responses;
using School.DTO.PermissionDTOs.Requests;
using School.DTO.PermissionDTOs.Responses;
using School.DTO.PersonDTOs.Requests;
using School.DTO.PersonDTOs.Responses;
using School.DTO.RoleDTOs.Requests;
using School.DTO.RoleDTOs.Responses;
using School.DTO.ScheduleDTOs.Requests;
using School.DTO.ScheduleDTOs.Responses;
using School.DTO.StudentGradeDTOs.Requests;
using School.DTO.StudentGradeDTOs.Responses;
using School.DTO.StudentsDTOs.Requests;
using School.DTO.StudentsDTOs.Responses;
using School.DTO.StudentStatusDTOs.Requests;
using School.DTO.StudentStatusDTOs.Responses;
using School.DTO.SubjectDTOs.Requests;
using School.DTO.SubjectDTOs.Responses;
using School.DTO.TeachersDTOs.Requests;
using School.DTO.TeachersDTOs.Responses;
using School.DTO.UserDTOs.Requests;
using School.DTO.UserDTOs.Responses;
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
        public static CreateStudentRequest ValidCreateStudentRequest(int personId = 100,
       int classId = 10, DateTime? enrollmentDate = null, int statusId = (int)StudentStatus.Active) => new()
       {
           PersonID = personId,
           ClassID = classId,
           EnrollmentDate = enrollmentDate ?? new DateTime(2025, 9, 1),
           StatusID = statusId
       };

        public static UpdateStudentRequest ValidUpdateStudentRequest(
        int classId = 10, DateTime? enrollmentDate = null, int statusId = (int)StudentStatus.Active) => new()
        {
            ClassID = classId,
            EnrollmentDate = enrollmentDate ?? new DateTime(2025, 9, 1),
            StatusID = statusId
        };
        #endregion
        #region ExamType
        public static ExamTypeResponse ValidExamType(int examTypeId = 1, string examName = "Midterm") => new()
        {
            ExamTypeID = examTypeId,
            ExamName = examName
        };

        public static CreateExamTypeRequest ValidCreateExamTypeRequest(string examName = "Midterm") => new()
        {
            ExamName = examName
        };

        public static UpdateExamTypeRequest ValidUpdateExamTypeRequest(string examName = "Midterm") => new()
        {
            ExamName = examName
        };
        #endregion
        #region Grade
        public static GradeResponse ValidGrade(
        byte gradeId = 1, string gradeName = "Grade 1") => new()
        {
            GradeID = gradeId,
            GradeName = gradeName
        };

        public static CreateGradeRequest ValidCreateGradeRequest(
        string gradeName = "Grade 1") => new()
        {
            GradeName = gradeName
        };

        public static UpdateGradeRequest ValidUpdateGradeRequest(
        string gradeName = "Grade 1") => new()
        {
            GradeName = gradeName
        };

        #endregion
        #region AttendanceStatus
        public static AttendanceStatusRequest ValidAttendanceStatusRequest(string statusName = "Present") => new()
        {
            StatusName = statusName
        };

        public static AttendanceStatusResponse ValidAttendanceStatusResponse(int statusId = 1, string statusName = "Present") => new()
        {
            StatusID = statusId,
            StatusName = statusName
        };
        #endregion
        #region Exam
        public static CreateExamRequest ValidCreateExamRequest(int classSubjectId = 1, int examTypeId = 1, DateOnly? examDate = null,
            decimal totalMarks = 100) => new()
            {
                ClassSubjectID = classSubjectId,
                ExamTypeID = examTypeId,
                ExamDate = examDate ?? new DateOnly(2026, 3, 15),
                TotalMarks = totalMarks
            };

        public static UpdateExamRequest ValidUpdateExamRequest(
            int classSubjectId = 1, int examTypeId = 1, DateOnly? examDate = null,
            decimal totalMarks = 100) => new()
            {
                ClassSubjectID = classSubjectId,
                ExamTypeID = examTypeId,
                ExamDate = examDate ?? new DateOnly(2026, 3, 15),
                TotalMarks = totalMarks
            };

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

        public static StudentGradeResponse ValidStudentGrade(int studentGradeId = 1, int studentId = 1, int examId = 1, decimal grade = 50,
            bool isAbsent = false) => new()
            {
                StudentGradeID = studentGradeId,
                StudentID = studentId,
                PersonID = 100,
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                ThirdName = "Ali",
                GradeID = 1,
                GradeName = "Grade 1",
                ClassID = 10,
                ClassName = "A",
                AcademicYear = "2025/2026",
                SubjectID = 1,
                SubjectName = "Math",
                ExamID = examId,
                ExamTypeID = 1,
                ExamName = "Midterm",
                ExamDate = DateTime.Today,
                TotalMarks = 100,
                Grade = grade,
                IsAbsent = isAbsent
            };
        #endregion
        #region ClassSubject
        public static CreateClassSubjectRequest ValidCreateClassSubjectRequest(int classId = 10, int teacherId = 1, int subjectId = 1) => new()
        {
            ClassID = classId,
            TeacherID = teacherId,
            SubjectID = subjectId
        };

        public static UpdateClassSubjectRequest ValidUpdateClassSubjectRequest(int teacherId = 1) => new()
        {
            TeacherID = teacherId
        };

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

        public static ClassResponse ValidClass(
            int classId = 10,
            bool isActive = true) => new()
            {
                ClassID = classId,
                GradeID = 1,
                GradeName = "Grade 1",
                ClassName = "Class A",
                AcademicYear = "2025-2026",
                Capacity = 30,
                IsActive = isActive
            };

        public static CreateClassRequest ValidCreateClassRequest(
            byte gradeId = 1,
            string className = "Class A",
            string academicYear = "2025-2026",
            int capacity = 30,
            bool isActive = true) => new()
            {
                GradeID = gradeId,
                ClassName = className,
                AcademicYear = academicYear,
                Capacity = capacity,
                IsActive = isActive
            };

        public static UpdateClassRequest ValidUpdateClassRequest(
            byte gradeId = 1,
            string className = "Class A",
            string academicYear = "2025-2026",
            int capacity = 30,
            bool isActive = true) => new()
            {
                GradeID = gradeId,
                ClassName = className,
                AcademicYear = academicYear,
                Capacity = capacity,
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
        #region Parent
        public static ParentResponse ValidParent(
            int parentId = 1, int personId = 100) => new()
            {
                ParentID = parentId,
                PersonID = personId,
                NationalID = "12345678901234",
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                ThirdName = "Ali",
                LastName = "Hassan",
                DateOfBirth = new DateTime(1980, 1, 1),
                Gender = 1,
                Address = "Alexandria",
                Phone = "01000000000",
                Email = "ahmed@example.com",
                ImagePath = null,
                CityID = 1
            };

        public static CreateParentRequest ValidCreateParentRequest(int personId = 1) => new()
        {
            PersonID = personId
        };
        #endregion
        #region Permission

        public static PermissionResponse ValidPermission(
            int permissionId = 1,
            string permissionName = "Students.View",
            string? description = "View students") => new()
            {
                PermissionID = permissionId,
                PermissionName = permissionName,
                Description = description
            };

        public static CreatePermissionRequest ValidCreatePermissionRequest(
            string permissionName = "Students.View",
            string? description = "View students") => new()
            {
                PermissionName = permissionName,
                Description = description
            };

        public static UpdatePermissionRequest ValidUpdatePermissionRequest(
            string permissionName = "Students.View",
            string? description = "View students") => new()
            {
                PermissionName = permissionName,
                Description = description
            };
        #endregion
        #region Person
        public static PersonResponse ValidPerson(
            int personId = 1,
            string nationalId = "12345678901234") => new()
            {
                PersonID = personId,
                NationalID = nationalId,
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                ThirdName = "Ali",
                LastName = "Hassan",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = 1,
                Phone = "01000000000",
                Email = "ahmed@example.com",
                Address = "Alexandria",
                CityID = 1,
                ImagePath = null
            };

        public static CreatePersonRequest ValidCreatePersonRequest(
            string nationalId = "12345678901234",
            DateTime? dateOfBirth = null,
            string? email = "ahmed@example.com") => new()
            {
                NationalID = nationalId,
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                ThirdName = "Ali",
                LastName = "Hassan",
                DateOfBirth = dateOfBirth ?? new DateTime(1990, 1, 1),
                Gender = 1,
                Phone = "01000000000",
                Email = email,
                Address = "Alexandria",
                CityID = 1,
                ImagePath = null
            };

        public static UpdatePersonRequest ValidUpdatePersonRequest(string nationalId = "12345678901234", DateTime? dateOfBirth = null, string? email = "ahmed@example.com") => new()
        {
            NationalID = nationalId,
            FirstName = "Ahmed",
            SecondName = "Mohamed",
            ThirdName = "Ali",
            LastName = "Hassan",
            DateOfBirth = dateOfBirth ?? new DateTime(1990, 1, 1),
            Gender = 1,
            Phone = "01000000000",
            Email = email,
            Address = "Alexandria",
            CityID = 1,
            ImagePath = null
        };
        #endregion
        #region User
        public static CreateUserRequest ValidCreateUserRequest(
            int personId = 1,
            string username = "ahmed123",
            string password = "P@ssw0rd",
            bool isActive = true) => new()
            {
                PersonID = personId,
                Username = username,
                Password = password,
                IsActive = isActive
            };

        public static UpdateUserRequest ValidUpdateUserRequest(
            string username = "ahmed123",
            bool isActive = true) => new()
            {
                Username = username,
                IsActive = isActive
            };

        public static UpdatePasswordRequest ValidUpdatePasswordRequest(
            string currentPassword = "OldP@ss1",
            string newPassword = "NewP@ss1",
            string confirmPassword = "NewP@ss1") => new()
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

        public static UserResponse ValidUser(
            int userId = 1,
            int personId = 1,
            string username = "ahmed123") => new()
            {
                UserID = userId,
                PersonID = personId,
                NationalID = "12345678901234",
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                ThirdName = "Ali",
                LastName = "Hassan",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = 1,
                Phone = "01000000000",
                Username = username,
                IsActive = true
            };
        #endregion
        #region UserRole
        public static UserRoleRequest ValidUserRoleRequest(
            int userId = 1,
            int roleId = 1) => new()
            {
                UserID = userId,
                RoleID = roleId
            };

        public static UserRoleResponse ValidUserRole(
            int userId = 1,
            int roleId = 1,
            string username = "ahmed123",
            string roleName = "Admin",
            bool isUserActive = true,
            bool isRoleActive = true) => new()
            {
                UserID = userId,
                Username = username,
                IsUserActive = isUserActive,
                RoleID = roleId,
                RoleName = roleName,
                RoleDescription = null,
                IsRoleActive = isRoleActive
            };
        #endregion
        #region TeacherSubject
        public static TeacherSubjectRequest ValidTeacherSubjectRequest(
            int teacherId = 1,
            int subjectId = 1) => new()
            {
                TeacherID = teacherId,
                SubjectID = subjectId
            };

        public static TeacherSubjectResponse ValidTeacherSubject(
            int teacherId = 1,
            int subjectId = 1,
            string subjectName = "Math") => new()
            {
                TeacherID = teacherId,
                FirstName = "Sara",
                SecondName = "Ibrahim",
                ThirdName = "Mostafa",
                LastName = "Ali",
                SubjectID = subjectId,
                SubjectName = subjectName
            };
        #endregion
        #region Teacher
        public static CreateTeacherRequest ValidCreateTeacherRequest(
            int personId = 1,
            DateTime? hireDate = null,
            decimal salary = 5000,
            bool isActive = true) => new()
            {
                PersonID = personId,
                HireDate = hireDate ?? new DateTime(2020, 9, 1),
                Salary = salary,
                IsActive = isActive
            };

        public static UpdateTeacherRequest ValidUpdateTeacherRequest(
            DateTime? hireDate = null,
            decimal salary = 5000,
            bool isActive = true) => new()
            {
                HireDate = hireDate ?? new DateTime(2020, 9, 1),
                Salary = salary,
                IsActive = isActive
            };

        public static TeacherResponse ValidTeacher(
            int teacherId = 1,
            int personId = 100,
            string nationalId = "12345678901234",
            DateTime? hireDate = null,
            decimal salary = 5000,
            bool isActive = true) => new()
            {
                TeacherID = teacherId,
                PersonID = personId,
                NationalID = nationalId,
                FullName = "Sara Ibrahim Mostafa",
                FirstName = "Sara",
                SecondName = "Ibrahim",
                ThirdName = "Mostafa",
                LastName = "Ali",
                DateOfBirth = new DateTime(1985, 1, 1),
                Gender = 2,
                Phone = "01000000000",
                CityID = 1,
                HireDate = hireDate ?? new DateTime(2020, 9, 1),
                Salary = salary,
                IsActive = isActive
            };
        #endregion
        #region Subject
        public static CreateSubjectRequest ValidCreateSubjectRequest(string subjectName = "Mathematics",
            bool isActive = true) => new()
            {
                SubjectName = subjectName,
                IsActive = isActive
            };

        public static UpdateSubjectRequest ValidUpdateSubjectRequest(string subjectName = "Mathematics", bool isActive = true) => new()
        {
            SubjectName = subjectName,
            IsActive = isActive
        };

        public static SubjectResponse ValidSubject(int subjectId = 1, string subjectName = "Mathematics", bool isActive = true) => new()
        {
            SubjectID = subjectId,
            SubjectName = subjectName,
            IsActive = isActive
        };
        #endregion
        #region StudentStatus
        public static CreateStudentStatusRequest ValidCreateStudentStatusRequest(string statusName = "Active",
            bool isActive = true) => new()
            {
                StatusName = statusName,
                IsActive = isActive
            };

        public static UpdateStudentStatusRequest ValidUpdateStudentStatusRequest(
            string statusName = "Inactive", bool isActive = true) => new()
            {
                StatusName = statusName,
                IsActive = isActive
            };

        public static StudentStatusResponse ValidStudentStatus(
            int statusId = 1, string statusName = "Active", bool isActive = true) => new()
            {
                StatusID = statusId,
                StatusName = statusName,
                IsActive = isActive
            };
        #endregion
        #region Role
        public static CreateRoleRequest ValidCreateRoleRequest(
            string roleName = "Admin",
            string? description = "System Administrator",
            bool isActive = true) => new()
            {
                RoleName = roleName,
                Description = description,
                IsActive = isActive
            };

        public static UpdateRoleRequest ValidUpdateRoleRequest(
            string roleName = "Admin", string? description = "System Administrator", bool isActive = true) => new()
            {
                RoleName = roleName,
                Description = description,
                IsActive = isActive
            };

        public static RoleResponse ValidRole(int roleId = 1, string roleName = "Admin",
            string? description = "System Administrator", bool isActive = true) => new()
            {
                RoleID = roleId,
                RoleName = roleName,
                Description = description,
                IsActive = isActive
            };
        #endregion
        #region RolePermission
        public static RolePermissionRequest ValidRolePermissionRequest(int roleId = 1, int permissionId = 1) => new()
        {
            RoleID = roleId,
            PermissionID = permissionId
        };

        public static RolePermissionResponse ValidRolePermission(
        int roleId = 1, int permissionId = 1, string roleName = "Admin", string permissionName = "Students.View",
        string? description = "View students",
        bool isRoleActive = true,
        bool isPermissionActive = true) => new()
        {
            RoleID = roleId,
            RoleName = roleName,
            PermissionID = permissionId,
            PermissionName = permissionName,
            Description = description,
            IsRoleActive = isRoleActive,
            IsPermissionActive = isPermissionActive
        };
        #endregion
        #region StudentParent
        public static StudentParentRequest ValidStudentParentRequest(int studentId = 1, int parentId = 1) => new()
        {
            StudentID = studentId,
            ParentID = parentId
        };

        public static StudentParentResponse ValidStudentParent(int studentId = 1, int parentId = 1,
        string studentName = "Ahmed Mohamed Ali Hassan", string parentName = "Mohamed Ali Hassan") => new()
        {
            StudentID = studentId,
            StudentName = studentName,
            ParentID = parentId,
            ParentName = parentName
        };
        #endregion
    }
}