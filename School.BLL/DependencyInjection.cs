using Microsoft.Extensions.DependencyInjection;
using School.DAL;

namespace School.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(this IServiceCollection services, string connectionString)
        {
            services.AddDAL(connectionString);

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

            return services;
        }
    }
}