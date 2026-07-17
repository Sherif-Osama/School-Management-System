using Microsoft.Extensions.DependencyInjection;

namespace School.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<PersonData>(_ => new PersonData(connectionString));

            services.AddScoped<StudentData>(_ => new StudentData(connectionString))
                ;
            services.AddScoped<TeacherData>(_ => new TeacherData(connectionString));

            services.AddScoped<ParentData>(_ => new ParentData(connectionString));

            services.AddScoped<StudentParentData>(_ => new StudentParentData(connectionString));

            services.AddScoped<SubjectData>(_ => new SubjectData(connectionString));

            services.AddScoped<GradeData>(_ => new GradeData(connectionString));

            services.AddScoped<ClassData>(_ => new ClassData(connectionString));

            services.AddScoped<TeacherSubjectData>(_ => new TeacherSubjectData(connectionString));

            services.AddScoped<ClassSubjectData>(_ => new ClassSubjectData(connectionString));

            services.AddScoped<UserData>(_ => new UserData(connectionString));

            services.AddScoped<ClassroomData>(_ => new ClassroomData(connectionString));

            services.AddScoped<ScheduleData>(_ => new ScheduleData(connectionString));

            services.AddScoped<ExamData>(_ => new ExamData(connectionString));

            services.AddScoped<ExamTypeData>(_ => new ExamTypeData(connectionString));

            services.AddScoped<StudentStatusData>(_ => new StudentStatusData(connectionString));

            services.AddScoped<AttendanceStatusData>(_ => new AttendanceStatusData(connectionString));

            services.AddScoped<AttendanceData>(_ => new AttendanceData(connectionString));

            services.AddScoped<StudentGradeData>(_ => new StudentGradeData(connectionString));

            return services;
        }
    }
}