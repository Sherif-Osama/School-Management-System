using Microsoft.Extensions.DependencyInjection;

namespace School.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(this IServiceCollection services)
        {
            services.AddScoped<PersonData>();

            services.AddScoped<StudentData>();
            ;
            services.AddScoped<TeacherData>();

            services.AddScoped<ParentData>();

            services.AddScoped<StudentParentData>();

            services.AddScoped<SubjectData>();

            services.AddScoped<GradeData>();

            services.AddScoped<ClassData>();

            services.AddScoped<TeacherSubjectData>();

            services.AddScoped<ClassSubjectData>();

            services.AddScoped<UserData>();

            services.AddScoped<ClassroomData>();

            services.AddScoped<ScheduleData>();

            services.AddScoped<ExamData>();

            services.AddScoped<ExamTypeData>();

            services.AddScoped<StudentStatusData>();

            services.AddScoped<AttendanceStatusData>();

            services.AddScoped<AttendanceData>();

            services.AddScoped<StudentGradeData>();

            return services;
        }
    }
}