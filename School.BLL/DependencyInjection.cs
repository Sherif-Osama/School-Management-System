using Microsoft.Extensions.DependencyInjection;
using School.DAL;

namespace School.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddDAL();

            services.AddScoped<PersonService>();
            services.AddScoped<StudentService>();
            services.AddScoped<TeacherService>();
            services.AddScoped<ParentService>();
            services.AddScoped<StudentParentService>();
            services.AddScoped<SubjectService>();
            services.AddScoped<GradeService>();
            services.AddScoped<ClassService>();
            services.AddScoped<TeacherSubjectService>();
            services.AddScoped<ClassSubjectService>();
            services.AddScoped<UserService>();
            services.AddScoped<ClassroomService>();
            services.AddScoped<ScheduleService>();
            services.AddScoped<ExamService>();
            services.AddScoped<ExamTypeService>();
            services.AddScoped<StudentStatusService>();
            services.AddScoped<AttendanceStatusService>();
            services.AddScoped<AttendanceService>();
            services.AddScoped<StudentGradeService>();

            return services;
        }
    }
}